public static class SnakeSpellConstants
{
    public const float FieldRadius = 500f;
    public const float WizardHitRadius = 30f;
    public const float SpellDespawnRadius = 520f;
    public const int LightningBoltZapCount = 3;

    public static string GetLifeLossMessage(int livesLost)
    {
        return livesLost == 1
            ? "A snake reached the wizard! -1 life"
            : $"Snakes reached the wizard! -{livesLost} lives";
    }

    public static string GetGameStartHint(int startingLives)
    {
        return $"Protect the wizard! You have {startingLives} lives.";
    }
}
