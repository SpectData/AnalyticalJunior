package com.braincademy.junior.game

import com.braincademy.junior.model.Difficulty
import com.braincademy.junior.model.GameQuestion
import com.braincademy.junior.model.Question
import kotlin.math.pow
import kotlin.random.Random

object MathGames {
    fun generateQuickCalc(difficulty: Difficulty): GameQuestion.Standard {
        var a: Int
        var b: Int
        val op: String
        val answer: Int

        when (difficulty) {
            Difficulty.EASY -> {
                a = rand(1, 10)
                b = rand(1, 10)
                op = listOf("+", "-").random()
            }
            Difficulty.MEDIUM -> {
                val ops = listOf("+", "-", "\u00D7")
                op = ops.random()
                if (op == "\u00D7") {
                    a = rand(2, 12)
                    b = rand(2, 12)
                } else {
                    a = rand(5, 30)
                    b = rand(5, 30)
                }
            }
            Difficulty.HARD -> {
                val ops = listOf("+", "-", "\u00D7")
                op = ops.random()
                if (op == "\u00D7") {
                    a = rand(5, 15)
                    b = rand(5, 15)
                } else {
                    a = rand(20, 100)
                    b = rand(20, 100)
                }
            }
            Difficulty.SUPER_HARD -> {
                val ops = listOf("+", "-", "\u00D7", "\u00F7")
                op = ops.random()
                when (op) {
                    "\u00D7" -> {
                        a = rand(10, 25)
                        b = rand(10, 25)
                    }
                    "\u00F7" -> {
                        b = rand(2, 12)
                        a = b * rand(2, 15)
                    }
                    else -> {
                        a = rand(50, 200)
                        b = rand(50, 200)
                    }
                }
            }
        }

        answer =
            when (op) {
                "+" -> a + b
                "-" -> {
                    if (b > a) {
                        val tmp = a
                        a = b
                        b = tmp
                    }
                    a - b
                }
                "\u00F7" -> a / b
                else -> a * b
            }

        val distractors = generateDistractors(answer, 3)
        val answers = (listOf(answer) + distractors).shuffled()
        return GameQuestion.Standard(
            Question(
                label = "Quick Calc",
                questionText = "$a $op $b = ?",
                answers = answers.map { it.toString() },
                correctAnswer = answer.toString(),
            ),
        )
    }

    fun generateSequence(difficulty: Difficulty): GameQuestion.Standard {
        val seq: List<Int> =
            when (difficulty) {
                Difficulty.EASY -> {
                    val start = rand(1, 10)
                    val step = rand(1, 5)
                    List(5) { i -> start + step * i }
                }
                Difficulty.MEDIUM -> {
                    val start = rand(1, 5)
                    val mult = rand(2, 3)
                    List(5) { i -> start * mult.toDouble().pow(i).toInt() }
                }
                Difficulty.HARD -> {
                    val a = rand(1, 3)
                    val b = rand(1, 4)
                    val c = rand(1, 3)
                    List(5) { i -> a * i * i + b * i + c }
                }
                Difficulty.SUPER_HARD -> {
                    val a = rand(2, 5)
                    val b = rand(2, 6)
                    val c = rand(1, 5)
                    List(5) { i -> a * i * i + b * i + c }
                }
            }

        val answer = seq[4]
        val display = seq.take(4).map { it.toString() } + "?"
        val distractors = generateDistractors(answer, 3)
        val answers = (listOf(answer) + distractors).shuffled()

        return GameQuestion.Standard(
            Question(
                label = "Number Sequence",
                sequence = display,
                answers = answers.map { it.toString() },
                correctAnswer = answer.toString(),
            ),
        )
    }

    fun generateCompare(difficulty: Difficulty): GameQuestion.Standard {
        var exprA: String
        var exprB: String
        var valA: Int
        var valB: Int

        when (difficulty) {
            Difficulty.EASY -> {
                val a1 = rand(1, 10)
                val a2 = rand(1, 10)
                val b1 = rand(1, 10)
                val b2 = rand(1, 10)
                valA = a1 + a2
                valB = b1 + b2
                exprA = "$a1 + $a2"
                exprB = "$b1 + $b2"
            }
            Difficulty.MEDIUM -> {
                val a1 = rand(2, 8)
                val a2 = rand(2, 8)
                val b1 = rand(2, 8)
                val b2 = rand(2, 8)
                valA = a1 * a2
                valB = b1 * b2
                exprA = "$a1 \u00D7 $a2"
                exprB = "$b1 \u00D7 $b2"
            }
            Difficulty.HARD -> {
                val a1 = rand(5, 15)
                val a2 = rand(2, 8)
                val a3 = rand(1, 10)
                val b1 = rand(5, 15)
                val b2 = rand(2, 8)
                val b3 = rand(1, 10)
                valA = a1 * a2 + a3
                valB = b1 * b2 + b3
                exprA = "$a1 \u00D7 $a2 + $a3"
                exprB = "$b1 \u00D7 $b2 + $b3"
            }
            Difficulty.SUPER_HARD -> {
                val a1 = rand(10, 25)
                val a2 = rand(3, 12)
                val a3 = rand(10, 50)
                val b1 = rand(10, 25)
                val b2 = rand(3, 12)
                val b3 = rand(10, 50)
                valA = a1 * a2 + a3
                valB = b1 * b2 + b3
                exprA = "$a1 \u00D7 $a2 + $a3"
                exprB = "$b1 \u00D7 $b2 + $b3"
            }
        }

        if (valA == valB) valB += 1

        val correct = if (valA > valB) exprA else exprB
        val answers = listOf(exprA, exprB).shuffled()

        return GameQuestion.Standard(
            Question(
                label = "Compare",
                questionText = "Which is larger?",
                answers = answers,
                correctAnswer = correct,
            ),
        )
    }

    private fun rand(
        min: Int,
        max: Int,
    ) = Random.nextInt(min, max + 1)

    private fun generateDistractors(
        correct: Int,
        count: Int,
    ): List<Int> {
        val set = mutableSetOf<Int>()
        while (set.size < count) {
            val r = Random.nextFloat()
            val d =
                when {
                    r < 0.3f -> correct + rand(1, 5)
                    r < 0.6f -> correct - rand(1, 5)
                    r < 0.8f -> correct + rand(5, 15)
                    else -> correct - rand(5, 15)
                }
            if (d != correct && d >= 0) set.add(d)
        }
        return set.toList()
    }
}
