package com.braincademy.junior.viewmodel

import android.app.Application
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import com.braincademy.junior.game.LogicGames
import com.braincademy.junior.game.MathGames
import com.braincademy.junior.game.QuestionBankLoader
import com.braincademy.junior.game.WaveGenerator
import com.braincademy.junior.model.BattlefieldState
import com.braincademy.junior.model.Difficulty
import com.braincademy.junior.model.GameQuestion
import com.braincademy.junior.model.GameStatus
import com.braincademy.junior.model.Snake
import com.braincademy.junior.model.SnakeSpellConstants
import com.braincademy.junior.model.Spell
import com.braincademy.junior.model.toSnakeSpellConfig
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

class SnakeSpellViewModel(application: Application) : AndroidViewModel(application) {
    private val questionBank = QuestionBankLoader(application)

    // ── Configuration ──
    var difficulty by mutableStateOf(Difficulty.EASY)
        private set
    var config by mutableStateOf(Difficulty.EASY.toSnakeSpellConfig())
        private set

    // ── Battlefield ──
    var battlefield by mutableStateOf(BattlefieldState())
        private set

    // ── Question state ──
    var currentQuestion by mutableStateOf<GameQuestion.Standard?>(null)
        private set
    var questionTimeLeft by mutableIntStateOf(20)
        private set
    var answered by mutableStateOf(false)
        private set
    var selectedAnswerIndex by mutableIntStateOf(-1)
        private set
    var showCorrect by mutableStateOf(false)
        private set

    // ── Feedback ──
    var feedbackText by mutableStateOf("")
        private set
    var feedbackIsCorrect by mutableStateOf(true)
        private set
    var showFeedback by mutableStateOf(false)
        private set

    // ── Internal ──
    private var nextSnakeId = 0
    private var nextSpellId = 0
    private var lastFrameTime = 0L
    private var spawnTimer = 0f
    private var snakesSpawnedThisWave = 0
    private var totalSnakesThisWave = 0
    private var waveTransitionCountdown = 0f
    private var gameLoopJob: Job? = null
    private var questionTimerJob: Job? = null

    fun startGame(diff: Difficulty) {
        difficulty = diff
        config = diff.toSnakeSpellConfig()
        nextSnakeId = 0
        nextSpellId = 0
        spawnTimer = 0f
        snakesSpawnedThisWave = 0
        waveTransitionCountdown = 0f
        questionBank.reset()

        battlefield =
            BattlefieldState(
                status = GameStatus.PLAYING,
                lives = config.startingLives,
            )

        startWave(1)
        generateNextQuestion()
        startGameLoop()
    }

    private fun startGameLoop() {
        gameLoopJob?.cancel()
        lastFrameTime = System.nanoTime()
        gameLoopJob =
            viewModelScope.launch {
                while (battlefield.status != GameStatus.GAME_OVER) {
                    val now = System.nanoTime()
                    val dt = (now - lastFrameTime) / 1_000_000_000f
                    lastFrameTime = now

                    when (battlefield.status) {
                        GameStatus.PLAYING -> updatePlaying(dt)
                        GameStatus.WAVE_TRANSITION -> updateWaveTransition(dt)
                        else -> {}
                    }

                    delay(33L)
                }
            }
    }

    private fun updatePlaying(dt: Float) {
        val movedSnakes =
            battlefield.snakes.map { snake ->
                snake.copy(xPosition = snake.xPosition - snake.speed * dt)
            }

        val (reached, alive) = movedSnakes.partition { it.xPosition <= SnakeSpellConstants.WIZARD_X }
        var newLives = battlefield.lives - reached.size

        val movedSpells =
            battlefield.spells.map { spell ->
                spell.copy(xPosition = spell.xPosition + spell.speed * dt)
            }

        val snakesMut = alive.toMutableList()
        val spellsToRemove = mutableSetOf<Int>()
        var newScore = battlefield.score
        var newKilled = battlefield.snakesKilled

        for (spell in movedSpells) {
            val target = snakesMut.find { it.id == spell.targetSnakeId && it.lane == spell.lane }
            if (target != null && spell.xPosition >= target.xPosition) {
                spellsToRemove.add(spell.id)
                val newHp = target.hp - 1
                if (newHp <= 0) {
                    snakesMut.remove(target)
                    newScore += config.pointsPerKill
                    newKilled++
                } else {
                    val idx = snakesMut.indexOf(target)
                    snakesMut[idx] = target.copy(hp = newHp)
                }
            }
            if (spell.xPosition > SnakeSpellConstants.FIELD_WIDTH) {
                spellsToRemove.add(spell.id)
            }
        }
        val remainingSpells = movedSpells.filter { it.id !in spellsToRemove }

        // Spawn snakes
        spawnTimer -= dt
        if (spawnTimer <= 0f && snakesSpawnedThisWave < totalSnakesThisWave) {
            val lane = WaveGenerator.pickLane()
            val type = WaveGenerator.pickSnakeType(battlefield.currentWave, config)
            val speed = WaveGenerator.getSnakeSpeed(type, battlefield.currentWave, config)
            snakesMut.add(
                Snake(
                    id = nextSnakeId++,
                    lane = lane,
                    type = type,
                    xPosition = SnakeSpellConstants.FIELD_WIDTH,
                    hp = type.baseHp,
                    speed = speed,
                ),
            )
            snakesSpawnedThisWave++
            spawnTimer = WaveGenerator.getSpawnInterval(battlefield.currentWave, config)
        }

        // Check wave complete
        val waveComplete = snakesSpawnedThisWave >= totalSnakesThisWave && snakesMut.isEmpty()
        val newStatus =
            when {
                newLives <= 0 -> GameStatus.GAME_OVER
                waveComplete -> {
                    waveTransitionCountdown = 3f
                    GameStatus.WAVE_TRANSITION
                }
                else -> GameStatus.PLAYING
            }

        battlefield =
            battlefield.copy(
                status = newStatus,
                snakes = snakesMut,
                spells = remainingSpells,
                lives = newLives.coerceAtLeast(0),
                score = newScore,
                snakesKilled = newKilled,
            )
    }

    private fun updateWaveTransition(dt: Float) {
        waveTransitionCountdown -= dt
        battlefield = battlefield.copy(waveTransitionTimer = waveTransitionCountdown)

        if (waveTransitionCountdown <= 0f) {
            val nextWave = battlefield.currentWave + 1
            battlefield = battlefield.copy(status = GameStatus.PLAYING)
            startWave(nextWave)
        }
    }

    private fun startWave(waveNumber: Int) {
        totalSnakesThisWave = WaveGenerator.getSnakeCountForWave(waveNumber)
        snakesSpawnedThisWave = 0
        spawnTimer = 1.5f
        battlefield = battlefield.copy(currentWave = waveNumber)
    }

    // ── Question Handling ──

    private fun generateNextQuestion() {
        // Primary: OC question bank. Fallback: procedural generators.
        val bankQuestion = questionBank.getQuestion(difficulty)

        currentQuestion = bankQuestion ?: generateProceduralQuestion()
        answered = false
        selectedAnswerIndex = -1
        showCorrect = false
        startQuestionTimer()
    }

    private fun generateProceduralQuestion(): GameQuestion.Standard {
        val generators =
            buildList<() -> GameQuestion.Standard> {
                add { MathGames.generateQuickCalc(difficulty) }
                add { MathGames.generateSequence(difficulty) }
                add { MathGames.generateCompare(difficulty) }
                if (difficulty >= Difficulty.HARD) {
                    add { LogicGames.generatePatternMatch(difficulty) }
                    add { LogicGames.generateOddOneOut(difficulty) }
                }
            }
        return generators.random().invoke()
    }

    private fun startQuestionTimer() {
        questionTimerJob?.cancel()
        questionTimeLeft = config.questionTimerSeconds
        questionTimerJob =
            viewModelScope.launch {
                while (questionTimeLeft > 0) {
                    delay(1000L)
                    questionTimeLeft--
                }
                if (!answered) {
                    handleQuestionTimeout()
                }
            }
    }

    private fun handleQuestionTimeout() {
        answered = true
        showCorrect = true
        battlefield = battlefield.copy(questionsAnswered = battlefield.questionsAnswered + 1)
        displayFeedback("Time's up!", false)
        viewModelScope.launch {
            delay(800L)
            if (battlefield.status == GameStatus.PLAYING || battlefield.status == GameStatus.WAVE_TRANSITION) {
                generateNextQuestion()
            }
        }
    }

    fun submitAnswer(index: Int) {
        if (answered) return
        val q = currentQuestion?.question ?: return

        answered = true
        questionTimerJob?.cancel()
        selectedAnswerIndex = index
        showCorrect = true

        val isCorrect = q.answers[index] == q.correctAnswer
        battlefield =
            battlefield.copy(
                questionsAnswered = battlefield.questionsAnswered + 1,
                questionsCorrect = if (isCorrect) battlefield.questionsCorrect + 1 else battlefield.questionsCorrect,
            )

        if (isCorrect) {
            fireSpell()
            displayFeedback("Spell cast!", true)
        } else {
            displayFeedback("Miss!", false)
        }

        viewModelScope.launch {
            delay(if (isCorrect) 400L else 700L)
            if (battlefield.status == GameStatus.PLAYING || battlefield.status == GameStatus.WAVE_TRANSITION) {
                generateNextQuestion()
            }
        }
    }

    private fun fireSpell() {
        val nearestSnake =
            battlefield.snakes
                .filter { it.hp > 0 }
                .minByOrNull { it.xPosition }
                ?: return

        val spell =
            Spell(
                id = nextSpellId++,
                lane = nearestSnake.lane,
                xPosition = SnakeSpellConstants.WIZARD_X,
                targetSnakeId = nearestSnake.id,
            )

        battlefield = battlefield.copy(spells = battlefield.spells + spell)
    }

    private fun displayFeedback(
        text: String,
        correct: Boolean,
    ) {
        feedbackText = text
        feedbackIsCorrect = correct
        showFeedback = true
        viewModelScope.launch {
            delay(800L)
            showFeedback = false
        }
    }

    fun cleanup() {
        gameLoopJob?.cancel()
        questionTimerJob?.cancel()
    }

    override fun onCleared() {
        super.onCleared()
        cleanup()
    }
}
