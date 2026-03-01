using UnityEngine;

public static class WaveGenerator
{
    public static int GetSnakeCountForWave(int waveNumber)
    {
        return Mathf.Min(3 + waveNumber, 12);
    }

    public static SnakeType PickSnakeType(int waveNumber, DifficultyConfig config)
    {
        if (waveNumber <= 1) return SnakeType.Green;
        return config.availableSnakeTypes[Random.Range(0, config.availableSnakeTypes.Count)];
    }

    public static float GetSpawnInterval(int waveNumber, DifficultyConfig config)
    {
        float reduction = (waveNumber - 1) * 0.1f;
        return Mathf.Max(config.baseSpawnInterval - reduction, 1.0f);
    }

    public static float GetSnakeSpeed(SnakeType snakeType, int waveNumber, DifficultyConfig config)
    {
        float waveBonus = (waveNumber - 1) * 2f;
        return (config.baseSnakeSpeed + waveBonus) * SnakeTypeData.GetSpeedMultiplier(snakeType);
    }

    public static float PickAngle()
    {
        return Random.Range(0f, 360f);
    }
}
