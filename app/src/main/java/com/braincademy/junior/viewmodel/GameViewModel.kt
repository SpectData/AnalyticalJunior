package com.braincademy.junior.viewmodel

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.braincademy.junior.game.LogicGames
import com.braincademy.junior.game.MathGames
import com.braincademy.junior.model.Category
import com.braincademy.junior.model.Difficulty
import com.braincademy.junior.model.GameQuestion
import com.braincademy.junior.model.GameStats
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

class GameViewModel : ViewModel() {
    companion object {
        const val TOTAL_QUESTIONS = 5
    }

    // ── Persistent stats ──
    var stats by mutableStateOf(GameStats())
        private set

    // ── Current game state ──
    var category by mutableStateOf(Category.MATH)
        private set
    var difficulty by mutableStateOf(Difficulty.EASY)
        private set
    var currentQuestionIndex by mutableIntStateOf(0)
        private set
    var gameScore by mutableIntStateOf(0)
        private set
    var streak by mutableIntStateOf(0)
        private set
    var correctCount by mutableIntStateOf(0)
        private set
    var timeLeft by mutableIntStateOf(20)
        private set
    var answered by mutableStateOf(false)
        private set
    var currentQuestion by mutableStateOf<GameQuestion?>(null)
        private set

    // ── Memory grid state ──
    var memoryShowPhase by mutableStateOf(true)
        private set
    var memorySelectedCells by mutableStateOf<Set<Int>>(emptySet())
        private set
    var memoryCorrectPicks by mutableIntStateOf(0)
        private set
    var memoryDone by mutableStateOf(false)
        private set

    // ── Answer feedback ──
    var selectedAnswerIndex by mutableIntStateOf(-1)
        private set
    var showCorrect by mutableStateOf(false)
        private set

    // ── Feedback text ──
    var feedbackText by mutableStateOf("")
        private set
    var feedbackIsCorrect by mutableStateOf(true)
        private set
    var showFeedback by mutableStateOf(false)
        private set

    private var timerJob: Job? = null

    fun selectCategory(cat: Category) {
        category = cat
    }

    fun startGame(diff: Difficulty) {
        difficulty = diff
        currentQuestionIndex = 0
        gameScore = 0
        streak = 0
        correctCount = 0
        nextQuestion()
    }

    fun nextQuestion() {
        if (currentQuestionIndex >= TOTAL_QUESTIONS) return

        answered = false
        selectedAnswerIndex = -1
        showCorrect = false
        memoryShowPhase = true
        memorySelectedCells = emptySet()
        memoryCorrectPicks = 0
        memoryDone = false
        currentQuestionIndex++

        currentQuestion = generateQuestion()
        startTimer()

        // Handle memory show phase
        val q = currentQuestion
        if (q is GameQuestion.Memory) {
            viewModelScope.launch {
                delay(q.memoryQuestion.showTimeMs)
                memoryShowPhase = false
            }
        }
    }

    private fun generateQuestion(): GameQuestion {
        return if (category == Category.MATH) {
            val generators =
                listOf(
                    { MathGames.generateQuickCalc(difficulty) },
                    { MathGames.generateSequence(difficulty) },
                    { MathGames.generateCompare(difficulty) },
                )
            generators.random().invoke()
        } else {
            val generators: List<() -> GameQuestion> =
                listOf(
                    { LogicGames.generatePatternMatch(difficulty) },
                    { LogicGames.generateOddOneOut(difficulty) },
                    { LogicGames.generateMemoryGrid(difficulty) },
                )
            generators.random().invoke()
        }
    }

    private fun startTimer() {
        timerJob?.cancel()
        timeLeft = difficulty.timerSeconds
        timerJob =
            viewModelScope.launch {
                while (timeLeft > 0) {
                    delay(1000)
                    timeLeft--
                }
                if (!answered) {
                    handleTimeout()
                }
            }
    }

    private fun handleTimeout() {
        answered = true
        streak = 0
        showCorrect = true
        displayFeedback("Time's up!", false)
        scheduleNextQuestion()
    }

    fun selectAnswer(index: Int) {
        if (answered) return
        val q = (currentQuestion as? GameQuestion.Standard)?.question ?: return

        answered = true
        timerJob?.cancel()
        selectedAnswerIndex = index
        showCorrect = true

        val isCorrect = q.answers[index] == q.correctAnswer
        if (isCorrect) {
            streak++
            correctCount++
            val bonus = if (streak > 1) (difficulty.points * 0.5).toInt() else 0
            val pts = difficulty.points + bonus
            gameScore += pts
            displayFeedback(if (bonus > 0) "+$pts Streak!" else "+$pts", true)
        } else {
            streak = 0
            displayFeedback("Wrong!", false)
        }

        updateBestStreak()
        scheduleNextQuestion()
    }

    fun selectMemoryCell(index: Int) {
        val q = (currentQuestion as? GameQuestion.Memory)?.memoryQuestion ?: return
        if (answered || memoryShowPhase || index in memorySelectedCells) return
        if (memorySelectedCells.size >= q.highlighted.size) return

        memorySelectedCells = memorySelectedCells + index
        if (index in q.highlighted) {
            memoryCorrectPicks++
        }

        if (memorySelectedCells.size >= q.highlighted.size) {
            memoryDone = true
            answered = true
            timerJob?.cancel()

            if (memoryCorrectPicks == q.highlighted.size) {
                streak++
                correctCount++
                val bonus = if (streak > 1) (difficulty.points * 0.5).toInt() else 0
                val pts = difficulty.points + bonus
                gameScore += pts
                displayFeedback(if (bonus > 0) "+$pts Streak!" else "+$pts", true)
            } else {
                streak = 0
                displayFeedback("$memoryCorrectPicks/${q.highlighted.size}", false)
            }

            updateBestStreak()
            scheduleNextQuestion()
        }
    }

    val isGameOver: Boolean
        get() = currentQuestionIndex >= TOTAL_QUESTIONS && answered

    fun finalizeGame() {
        timerJob?.cancel()
        stats =
            stats.copy(
                totalScore = stats.totalScore + gameScore,
                gamesPlayed = stats.gamesPlayed + 1,
                bestStreak = maxOf(stats.bestStreak, streak),
            )
    }

    val accuracy: Int
        get() = if (TOTAL_QUESTIONS > 0) (correctCount * 100) / TOTAL_QUESTIONS else 0

    val starCount: Int
        get() =
            when {
                accuracy >= 100 -> 3
                accuracy >= 60 -> 2
                accuracy >= 20 -> 1
                else -> 0
            }

    private fun updateBestStreak() {
        if (streak > stats.bestStreak) {
            stats = stats.copy(bestStreak = streak)
        }
    }

    private fun displayFeedback(
        text: String,
        correct: Boolean,
    ) {
        feedbackText = text
        feedbackIsCorrect = correct
        showFeedback = true
        viewModelScope.launch {
            delay(800)
            showFeedback = false
        }
    }

    private fun scheduleNextQuestion() {
        viewModelScope.launch {
            delay(1200)
            if (currentQuestionIndex >= TOTAL_QUESTIONS) {
                finalizeGame()
            } else {
                nextQuestion()
            }
        }
    }
}
