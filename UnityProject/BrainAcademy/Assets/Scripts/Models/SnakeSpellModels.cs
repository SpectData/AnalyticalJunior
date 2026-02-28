using System.Collections.Generic;
using UnityEngine;

public static class SnakeTypeData
{
    public static int GetBaseHp(SnakeType type)
    {
        switch (type)
        {
            case SnakeType.Green: return 1;
            case SnakeType.Yellow: return 2;
            case SnakeType.Red: return 3;
            case SnakeType.Purple: return 4;
            default: return 1;
        }
    }

    public static float GetSpeedMultiplier(SnakeType type)
    {
        switch (type)
        {
            case SnakeType.Red: return 0.75f;
            case SnakeType.Purple: return 1.25f;
            default: return 1.0f;
        }
    }

    public static Color GetBodyColor(SnakeType type)
    {
        switch (type)
        {
            case SnakeType.Green: return AppColors.HexToColor("27AE60");
            case SnakeType.Yellow: return AppColors.HexToColor("F1C40F");
            case SnakeType.Red: return AppColors.HexToColor("E74C3C");
            case SnakeType.Purple: return AppColors.HexToColor("8E44AD");
            default: return Color.white;
        }
    }

    public static Color GetHeadColor(SnakeType type)
    {
        switch (type)
        {
            case SnakeType.Green: return AppColors.HexToColor("1E8449");
            case SnakeType.Yellow: return AppColors.HexToColor("D4AC0D");
            case SnakeType.Red: return AppColors.HexToColor("C0392B");
            case SnakeType.Purple: return AppColors.HexToColor("6C3483");
            default: return Color.gray;
        }
    }

    public static string GetDisplayName(SnakeType type)
    {
        switch (type)
        {
            case SnakeType.Green: return "Green Snake";
            case SnakeType.Yellow: return "Yellow Snake";
            case SnakeType.Red: return "Red Snake";
            case SnakeType.Purple: return "Purple Snake";
            default: return "Snake";
        }
    }
}

public class SnakeData
{
    public int id;
    public int lane;
    public SnakeType type;
    public float xPosition;
    public int hp;
    public float speed;

    public SnakeData(int id, int lane, SnakeType type, float xPosition, int hp, float speed)
    {
        this.id = id;
        this.lane = lane;
        this.type = type;
        this.xPosition = xPosition;
        this.hp = hp;
        this.speed = speed;
    }
}

public class SpellData
{
    public int id;
    public int lane;
    public float xPosition;
    public int targetSnakeId;
    public float speed;

    public SpellData(int id, int lane, float xPosition, int targetSnakeId, float speed = 400f)
    {
        this.id = id;
        this.lane = lane;
        this.xPosition = xPosition;
        this.targetSnakeId = targetSnakeId;
        this.speed = speed;
    }
}

public class BattlefieldState
{
    public GameStatus status = GameStatus.NotStarted;
    public List<SnakeData> snakes = new List<SnakeData>();
    public List<SpellData> spells = new List<SpellData>();
    public int currentWave;
    public int lives = 5;
    public int score;
    public int snakesKilled;
    public int questionsAnswered;
    public int questionsCorrect;
    public float waveTransitionTimer;
}

public class DifficultyConfig
{
    public float baseSnakeSpeed;
    public float baseSpawnInterval;
    public int startingLives;
    public List<SnakeType> availableSnakeTypes;
    public int questionTimerSeconds;
    public int pointsPerKill;

    public DifficultyConfig(float baseSnakeSpeed, float baseSpawnInterval, int startingLives,
        List<SnakeType> availableSnakeTypes, int questionTimerSeconds, int pointsPerKill)
    {
        this.baseSnakeSpeed = baseSnakeSpeed;
        this.baseSpawnInterval = baseSpawnInterval;
        this.startingLives = startingLives;
        this.availableSnakeTypes = availableSnakeTypes;
        this.questionTimerSeconds = questionTimerSeconds;
        this.pointsPerKill = pointsPerKill;
    }
}

public static class DifficultyExtensions
{
    public static int GetPoints(this Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy: return 10;
            case Difficulty.Medium: return 25;
            case Difficulty.Hard: return 50;
            case Difficulty.SuperHard: return 100;
            default: return 10;
        }
    }

    public static int GetStreakBonus(this Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy: return 5;
            case Difficulty.Medium: return 12;
            case Difficulty.Hard: return 25;
            case Difficulty.SuperHard: return 50;
            default: return 5;
        }
    }

    public static int GetTimerSeconds(this Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy: return 20;
            case Difficulty.Medium: return 15;
            case Difficulty.Hard: return 10;
            case Difficulty.SuperHard: return 7;
            default: return 20;
        }
    }

    public static string GetDisplayName(this Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy: return "Easy";
            case Difficulty.Medium: return "Medium";
            case Difficulty.Hard: return "Hard";
            case Difficulty.SuperHard: return "Super Hard";
            default: return "Easy";
        }
    }

    public static string ToBankKey(this Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy: return "easy";
            case Difficulty.Medium: return "medium";
            case Difficulty.Hard: return "hard";
            case Difficulty.SuperHard: return "super_hard";
            default: return "easy";
        }
    }

    public static DifficultyConfig ToSnakeSpellConfig(this Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy:
                return new DifficultyConfig(
                    baseSnakeSpeed: 30f,
                    baseSpawnInterval: 60.0f,
                    startingLives: 5,
                    availableSnakeTypes: new List<SnakeType> { SnakeType.Green },
                    questionTimerSeconds: 60,
                    pointsPerKill: 10
                );
            case Difficulty.Medium:
                return new DifficultyConfig(
                    baseSnakeSpeed: 45f,
                    baseSpawnInterval: 60.0f,
                    startingLives: 4,
                    availableSnakeTypes: new List<SnakeType> { SnakeType.Green, SnakeType.Yellow },
                    questionTimerSeconds: 60,
                    pointsPerKill: 25
                );
            case Difficulty.Hard:
                return new DifficultyConfig(
                    baseSnakeSpeed: 60f,
                    baseSpawnInterval: 60.0f,
                    startingLives: 3,
                    availableSnakeTypes: new List<SnakeType>
                        { SnakeType.Green, SnakeType.Yellow, SnakeType.Red },
                    questionTimerSeconds: 60,
                    pointsPerKill: 50
                );
            case Difficulty.SuperHard:
                return new DifficultyConfig(
                    baseSnakeSpeed: 80f,
                    baseSpawnInterval: 60.0f,
                    startingLives: 3,
                    availableSnakeTypes: new List<SnakeType>
                        { SnakeType.Green, SnakeType.Yellow, SnakeType.Red, SnakeType.Purple },
                    questionTimerSeconds: 60,
                    pointsPerKill: 100
                );
            default:
                return Difficulty.Easy.ToSnakeSpellConfig();
        }
    }
}
