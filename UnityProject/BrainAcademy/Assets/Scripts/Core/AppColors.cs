using UnityEngine;

public static class AppColors
{
    // Primary gradient
    public static readonly Color Purple60 = HexToColor("667EEA");
    public static readonly Color Purple80 = HexToColor("764BA2");

    // Game accent
    public static readonly Color SnakeGreen = HexToColor("2ECC71");

    // Difficulty colors
    public static readonly Color EasyGreen = HexToColor("27AE60");
    public static readonly Color MediumYellow = HexToColor("F39C12");
    public static readonly Color HardRed = HexToColor("E74C3C");
    public static readonly Color SuperHardPurple = HexToColor("8E44AD");

    // Battlefield (PvZ-style palette)
    public static readonly Color SpellGold = HexToColor("FFD700");
    public static readonly Color GrassLight = HexToColor("5DBF2D");
    public static readonly Color GrassDark = HexToColor("4A9E23");
    public static readonly Color DirtRoad = HexToColor("8B6914");

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
}
