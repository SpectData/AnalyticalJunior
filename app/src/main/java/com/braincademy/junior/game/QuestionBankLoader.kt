package com.braincademy.junior.game

import android.content.Context
import com.braincademy.junior.model.Difficulty
import com.braincademy.junior.model.GameQuestion
import com.braincademy.junior.model.Question
import org.json.JSONObject

class QuestionBankLoader(context: Context) {
    private data class BankQuestion(
        val id: String,
        val topic: String,
        val difficulty: String,
        val question: String,
        val options: List<String>,
        val correctAnswer: String,
    )

    private val allQuestions: List<BankQuestion>
    private val usedIds = mutableSetOf<String>()

    init {
        val json =
            context.assets.open("question_bank.json")
                .bufferedReader().use { it.readText() }
        val root = JSONObject(json)

        val questions = mutableListOf<BankQuestion>()

        val math = root.getJSONArray("mathematical_reasoning")
        for (i in 0 until math.length()) {
            questions.add(parseBankQuestion(math.getJSONObject(i)))
        }

        val thinking = root.getJSONArray("thinking_skills")
        for (i in 0 until thinking.length()) {
            questions.add(parseBankQuestion(thinking.getJSONObject(i)))
        }

        allQuestions = questions
    }

    private fun parseBankQuestion(obj: JSONObject): BankQuestion {
        val options = mutableListOf<String>()
        val arr = obj.getJSONArray("options")
        for (i in 0 until arr.length()) {
            options.add(arr.getString(i))
        }
        return BankQuestion(
            id = obj.getString("id"),
            topic = obj.getString("topic"),
            difficulty = obj.getString("difficulty"),
            question = obj.getString("question"),
            options = options,
            correctAnswer = obj.getString("correct_answer"),
        )
    }

    fun getQuestion(difficulty: Difficulty): GameQuestion.Standard? {
        val diffStr = difficulty.toBankKey()

        var available = allQuestions.filter { it.difficulty == diffStr && it.id !in usedIds }

        if (available.isEmpty()) {
            // All questions for this difficulty used — recycle
            usedIds.removeAll { id ->
                allQuestions.any { it.id == id && it.difficulty == diffStr }
            }
            available = allQuestions.filter { it.difficulty == diffStr }
        }

        if (available.isEmpty()) return null

        return convertToGameQuestion(available.random())
    }

    private fun convertToGameQuestion(bq: BankQuestion): GameQuestion.Standard {
        usedIds.add(bq.id)

        val label =
            bq.topic
                .replace("_", " ")
                .split(" ")
                .joinToString(" ") { it.replaceFirstChar { c -> c.uppercase() } }

        return GameQuestion.Standard(
            Question(
                label = label,
                questionText = bq.question,
                answers = bq.options,
                correctAnswer = bq.correctAnswer,
            ),
        )
    }

    fun reset() {
        usedIds.clear()
    }

    private fun Difficulty.toBankKey(): String =
        when (this) {
            Difficulty.EASY -> "easy"
            Difficulty.MEDIUM -> "medium"
            Difficulty.HARD -> "hard"
            Difficulty.SUPER_HARD -> "super_hard"
        }
}
