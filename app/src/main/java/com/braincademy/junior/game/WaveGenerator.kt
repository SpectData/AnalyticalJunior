package com.braincademy.junior.game

import com.braincademy.junior.model.DifficultyConfig
import com.braincademy.junior.model.SnakeSpellConstants
import com.braincademy.junior.model.SnakeType
import kotlin.random.Random

object WaveGenerator {
    fun getSnakeCountForWave(waveNumber: Int): Int {
        return (3 + waveNumber).coerceAtMost(12)
    }

    fun pickSnakeType(
        waveNumber: Int,
        config: DifficultyConfig,
    ): SnakeType {
        if (waveNumber <= 1) return SnakeType.GREEN
        return config.availableSnakeTypes.random()
    }

    fun getSpawnInterval(
        waveNumber: Int,
        config: DifficultyConfig,
    ): Float {
        val reduction = (waveNumber - 1) * 0.1f
        return (config.baseSpawnInterval - reduction).coerceAtLeast(1.0f)
    }

    fun getSnakeSpeed(
        snakeType: SnakeType,
        waveNumber: Int,
        config: DifficultyConfig,
    ): Float {
        val waveBonus = (waveNumber - 1) * 2f
        return (config.baseSnakeSpeed + waveBonus) * snakeType.speedMultiplier
    }

    fun pickLane(): Int = Random.nextInt(SnakeSpellConstants.NUM_LANES)
}
