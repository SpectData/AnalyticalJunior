package com.braincademy.junior.game

import com.braincademy.junior.model.Difficulty
import com.braincademy.junior.model.GameQuestion
import com.braincademy.junior.model.MemoryQuestion
import com.braincademy.junior.model.Question
import kotlin.random.Random

object LogicGames {
    private val shapes = listOf("\u25CF", "\u25A0", "\u25B2", "\u2666", "\u2605")

    fun generatePatternMatch(difficulty: Difficulty): GameQuestion.Standard {
        val answer: String
        val pattern: List<String>

        when (difficulty) {
            Difficulty.EASY -> {
                val a = shapes.random()
                var b: String
                do {
                    b = shapes.random()
                } while (b == a)
                pattern = listOf(a, b, a, b, "?")
                answer = a
            }
            Difficulty.MEDIUM -> {
                val picks = shapes.shuffled().take(3)
                pattern = listOf(picks[0], picks[1], picks[2], picks[0], picks[1], "?")
                answer = picks[2]
            }
            Difficulty.HARD -> {
                val a = shapes.random()
                var b: String
                do {
                    b = shapes.random()
                } while (b == a)
                pattern = listOf(a, a, b, b, a, a, b, "?")
                answer = b
            }
            Difficulty.SUPER_HARD -> {
                val picks = shapes.shuffled().take(4)
                pattern =
                    listOf(
                        picks[0], picks[1], picks[2], picks[3],
                        picks[0], picks[1], picks[2], "?",
                    )
                answer = picks[3]
            }
        }

        val wrongShapes = shapes.filter { it != answer }
        val distractors = wrongShapes.shuffled().take(3)
        val answers = (listOf(answer) + distractors).shuffled()

        return GameQuestion.Standard(
            Question(
                label = "Pattern Match",
                sequence = pattern,
                answers = answers,
                correctAnswer = answer,
            ),
        )
    }

    private data class OddOneOutConfig(
        val group: List<Int>,
        val outliers: List<Int>,
        val question: String,
    )

    fun generateOddOneOut(difficulty: Difficulty): GameQuestion.Standard {
        val config = pickOddOneOutConfig(difficulty)
        val picks = config.group.shuffled().take(3)
        val odd = config.outliers.random()
        val items = (picks + odd).shuffled()

        return GameQuestion.Standard(
            Question(
                label = "Odd One Out",
                questionText = config.question,
                answers = items.map { it.toString() },
                correctAnswer = odd.toString(),
            ),
        )
    }

    private fun pickOddOneOutConfig(difficulty: Difficulty): OddOneOutConfig =
        when (difficulty) {
            Difficulty.EASY -> {
                if (Random.nextBoolean()) {
                    OddOneOutConfig(
                        listOf(2, 4, 6, 8, 10, 12, 14),
                        listOf(1, 3, 5, 7, 9, 11, 13),
                        "Which number does NOT belong?",
                    )
                } else {
                    OddOneOutConfig(
                        listOf(1, 3, 5, 7, 9, 11, 13),
                        listOf(2, 4, 6, 8, 10, 12, 14),
                        "Which number does NOT belong?",
                    )
                }
            }
            Difficulty.MEDIUM -> {
                if (Random.nextBoolean()) {
                    OddOneOutConfig(
                        listOf(2, 3, 5, 7, 11, 13, 17, 19, 23),
                        listOf(4, 6, 8, 9, 10, 12, 14, 15),
                        "Which is NOT a prime number?",
                    )
                } else {
                    OddOneOutConfig(
                        listOf(3, 6, 9, 12, 15, 18, 21),
                        listOf(4, 5, 7, 8, 10, 11, 13),
                        "Which is NOT a multiple of 3?",
                    )
                }
            }
            Difficulty.HARD -> {
                if (Random.nextBoolean()) {
                    OddOneOutConfig(
                        listOf(1, 4, 9, 16, 25, 36, 49, 64),
                        listOf(2, 3, 5, 6, 7, 8, 10, 11, 12, 13, 15),
                        "Which is NOT a perfect square?",
                    )
                } else {
                    OddOneOutConfig(
                        listOf(1, 2, 3, 5, 8, 13, 21, 34),
                        listOf(4, 6, 7, 9, 10, 11, 12, 14),
                        "Which is NOT a Fibonacci number?",
                    )
                }
            }
            Difficulty.SUPER_HARD -> {
                when (Random.nextInt(3)) {
                    0 ->
                        OddOneOutConfig(
                            listOf(1, 8, 27, 64, 125),
                            listOf(2, 4, 9, 16, 25, 32, 36, 50),
                            "Which is NOT a perfect cube?",
                        )
                    1 ->
                        OddOneOutConfig(
                            listOf(2, 4, 8, 16, 32, 64, 128),
                            listOf(3, 6, 10, 12, 24, 48, 96),
                            "Which is NOT a power of 2?",
                        )
                    else ->
                        OddOneOutConfig(
                            listOf(1, 3, 6, 10, 15, 21, 28, 36),
                            listOf(2, 4, 5, 7, 8, 9, 11, 12, 14),
                            "Which is NOT a triangular number?",
                        )
                }
            }
        }

    fun generateMemoryGrid(difficulty: Difficulty): GameQuestion.Memory {
        val gridSize: Int
        val cellCount: Int
        val showTimeMs: Long

        when (difficulty) {
            Difficulty.EASY -> {
                gridSize = 3
                cellCount = 3
                showTimeMs = 3000L
            }
            Difficulty.MEDIUM -> {
                gridSize = 4
                cellCount = 4
                showTimeMs = 2500L
            }
            Difficulty.HARD -> {
                gridSize = 4
                cellCount = 5
                showTimeMs = 2000L
            }
            Difficulty.SUPER_HARD -> {
                gridSize = 5
                cellCount = 7
                showTimeMs = 1500L
            }
        }

        val totalCells = gridSize * gridSize
        val highlighted = mutableListOf<Int>()
        while (highlighted.size < cellCount) {
            val idx = Random.nextInt(totalCells)
            if (idx !in highlighted) highlighted.add(idx)
        }

        return GameQuestion.Memory(
            MemoryQuestion(gridSize = gridSize, highlighted = highlighted, showTimeMs = showTimeMs),
        )
    }
}
