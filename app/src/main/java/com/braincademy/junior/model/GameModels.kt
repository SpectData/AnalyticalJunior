package com.braincademy.junior.model

enum class Category(val displayName: String) {
    MATH("Math"),
    LOGIC("Logic & Puzzles"),
}

enum class Difficulty(val displayName: String, val points: Int, val timerSeconds: Int) {
    EASY("Easy", 10, 20),
    MEDIUM("Medium", 25, 15),
    HARD("Hard", 50, 10),
    SUPER_HARD("Super Hard", 100, 7),
}

data class Question(
    val label: String,
    val questionText: String? = null,
    val sequence: List<String>? = null,
    val answers: List<String>,
    val correctAnswer: String,
)

data class MemoryQuestion(
    val gridSize: Int,
    val highlighted: List<Int>,
    val showTimeMs: Long,
)

sealed class GameQuestion {
    data class Standard(val question: Question) : GameQuestion()

    data class Memory(val memoryQuestion: MemoryQuestion) : GameQuestion()
}

data class GameStats(
    val totalScore: Int = 0,
    val gamesPlayed: Int = 0,
    val bestStreak: Int = 0,
)
