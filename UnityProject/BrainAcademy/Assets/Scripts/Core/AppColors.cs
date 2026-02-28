using UnityEngine;

public static class AppColors
{
    // Primary gradient
    public static readonly Color Purple60 = HexToColor("667EEA");
    public static readonly Color Purple80 = HexToColor("764BA2");

    // Category accents
    public static readonly Color MathBlue = HexToColor("4A90D9");
    public static readonly Color LogicOrange = HexToColor("E67E22");
    public static readonly Color SnakeGreen = HexToColor("2ECC71");

    // Difficulty colors
    public static readonly Color EasyGreen = HexToColor("27AE60");
    public static readonly Color MediumYellow = HexToColor("F39C12");
    public static readonly Color HardRed = HexToColor("E74C3C");
    public static readonly Color SuperHardPurple = HexToColor("8E44AD");

    // Battlefield
    public static readonly Color SpellGold = HexToColor("E8D44D");
    public static readonly Color GrassLight = HexToColor("7EC850");
    public static readonly Color GrassDark = HexToColor("5DAE3B");
    public static readonly Color DirtRoad = new Color(0.545f, 0.451f, 0.333f, 0.25f);

    // UI
    public static readonly Color Background = HexToColor("F0F4F8");
    public static readonly Color CardWhite = Color.white;
    public static readonly Color TextPrimary = HexToColor("2C3E50");
    public static readonly Color TextSecondary = HexToColor("7F8C8D");
    public static readonly Color BorderLight = HexToColor("D0D8E0");
    public static readonly Color SurfaceLight = HexToColor("F0F4F8");

    // Feedback
    public static readonly Color CorrectGreen = HexToColor("27AE60");
    public static readonly Color WrongRed = HexToColor("E74C3C");

    public static Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#" + hex, out color);
        return color;
    }

    public static Color GetDifficultyColor(Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy: return EasyGreen;
            case Difficulty.Medium: return MediumYellow;
            case Difficulty.Hard: return HardRed;
            case Difficulty.SuperHard: return SuperHardPurple;
            default: return EasyGreen;
        }
    }

    public static Color GetCategoryColor(Category c)
    {
        switch (c)
        {
            case Category.Math: return MathBlue;
            case Category.Logic: return LogicOrange;
            default: return MathBlue;
        }
    }
}
