package com.braincademy.junior.model

import androidx.compose.ui.graphics.Color

enum class SnakeType(
    val displayName: String,
    val baseHp: Int,
    val speedMultiplier: Float,
    val bodyColor: Color,
    val headColor: Color,
) {
    GREEN("Green Snake", 1, 1.0f, Color(0xFF27AE60), Color(0xFF1E8449)),
    YELLOW("Yellow Snake", 2, 1.0f, Color(0xFFF1C40F), Color(0xFFD4AC0D)),
    RED("Red Snake", 3, 0.75f, Color(0xFFE74C3C), Color(0xFFC0392B)),
    PURPLE("Purple Snake", 4, 1.25f, Color(0xFF8E44AD), Color(0xFF6C3483)),
}

data class Snake(
    val id: Int,
    val lane: Int,
    val type: SnakeType,
    val xPosition: Float,
    val hp: Int,
    val speed: Float,
)

data class Spell(
    val id: Int,
    val lane: Int,
    val xPosition: Float,
    val targetSnakeId: Int,
    val speed: Float = 400f,
)

enum class GameStatus {
    NOT_STARTED,
    PLAYING,
    WAVE_TRANSITION,
    GAME_OVER,
}

data class BattlefieldState(
    val status: GameStatus = GameStatus.NOT_STARTED,
    val snakes: List<Snake> = emptyList(),
    val spells: List<Spell> = emptyList(),
    val currentWave: Int = 0,
    val lives: Int = 5,
    val score: Int = 0,
    val snakesKilled: Int = 0,
    val questionsAnswered: Int = 0,
    val questionsCorrect: Int = 0,
    val waveTransitionTimer: Float = 0f,
)

data class DifficultyConfig(
    val baseSnakeSpeed: Float,
    val baseSpawnInterval: Float,
    val startingLives: Int,
    val availableSnakeTypes: List<SnakeType>,
    val questionTimerSeconds: Int,
    val pointsPerKill: Int,
)

fun Difficulty.toSnakeSpellConfig(): DifficultyConfig =
    when (this) {
        Difficulty.EASY ->
            DifficultyConfig(
                baseSnakeSpeed = 30f,
                baseSpawnInterval = 4.0f,
                startingLives = 5,
                availableSnakeTypes = listOf(SnakeType.GREEN),
                questionTimerSeconds = 20,
                pointsPerKill = 10,
            )
        Difficulty.MEDIUM ->
            DifficultyConfig(
                baseSnakeSpeed = 45f,
                baseSpawnInterval = 3.0f,
                startingLives = 4,
                availableSnakeTypes = listOf(SnakeType.GREEN, SnakeType.YELLOW),
                questionTimerSeconds = 15,
                pointsPerKill = 25,
            )
        Difficulty.HARD ->
            DifficultyConfig(
                baseSnakeSpeed = 60f,
                baseSpawnInterval = 2.5f,
                startingLives = 3,
                availableSnakeTypes = listOf(SnakeType.GREEN, SnakeType.YELLOW, SnakeType.RED),
                questionTimerSeconds = 10,
                pointsPerKill = 50,
            )
        Difficulty.SUPER_HARD ->
            DifficultyConfig(
                baseSnakeSpeed = 80f,
                baseSpawnInterval = 2.0f,
                startingLives = 3,
                availableSnakeTypes = SnakeType.entries,
                questionTimerSeconds = 7,
                pointsPerKill = 100,
            )
    }

object SnakeSpellConstants {
    const val FIELD_WIDTH = 1000f
    const val WIZARD_X = 40f
    const val NUM_LANES = 3
}
