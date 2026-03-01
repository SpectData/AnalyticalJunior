using NUnit.Framework;

public class SnakeTypeDataTests
{
    [Test]
    public void GetBaseHp_ReturnsExpectedValues()
    {
        Assert.AreEqual(1, SnakeTypeData.GetBaseHp(SnakeType.Green));
        Assert.AreEqual(2, SnakeTypeData.GetBaseHp(SnakeType.Yellow));
        Assert.AreEqual(3, SnakeTypeData.GetBaseHp(SnakeType.Red));
        Assert.AreEqual(4, SnakeTypeData.GetBaseHp(SnakeType.Purple));
    }

    [Test]
    public void GetSpeedMultiplier_GreenAndYellow_Return1()
    {
        Assert.AreEqual(1.0f, SnakeTypeData.GetSpeedMultiplier(SnakeType.Green));
        Assert.AreEqual(1.0f, SnakeTypeData.GetSpeedMultiplier(SnakeType.Yellow));
    }

    [Test]
    public void GetSpeedMultiplier_Red_IsSlow()
    {
        Assert.AreEqual(0.75f, SnakeTypeData.GetSpeedMultiplier(SnakeType.Red));
    }

    [Test]
    public void GetSpeedMultiplier_Purple_IsFast()
    {
        Assert.AreEqual(1.25f, SnakeTypeData.GetSpeedMultiplier(SnakeType.Purple));
    }

    [Test]
    public void GetDisplayName_AllTypes_ReturnsExpected()
    {
        Assert.AreEqual("Green Snake", SnakeTypeData.GetDisplayName(SnakeType.Green));
        Assert.AreEqual("Yellow Snake", SnakeTypeData.GetDisplayName(SnakeType.Yellow));
        Assert.AreEqual("Red Snake", SnakeTypeData.GetDisplayName(SnakeType.Red));
        Assert.AreEqual("Purple Snake", SnakeTypeData.GetDisplayName(SnakeType.Purple));
    }

    [Test]
    public void GetCalloutText_AllTypes_ReturnsNonEmptyString()
    {
        Assert.IsNotEmpty(SnakeTypeData.GetCalloutText(SnakeType.Green));
        Assert.IsNotEmpty(SnakeTypeData.GetCalloutText(SnakeType.Yellow));
        Assert.IsNotEmpty(SnakeTypeData.GetCalloutText(SnakeType.Red));
        Assert.IsNotEmpty(SnakeTypeData.GetCalloutText(SnakeType.Purple));
    }

    [Test]
    public void GetCalloutText_ContainsSnakeName()
    {
        Assert.That(SnakeTypeData.GetCalloutText(SnakeType.Green), Does.Contain("Green"));
        Assert.That(SnakeTypeData.GetCalloutText(SnakeType.Yellow), Does.Contain("Yellow"));
        Assert.That(SnakeTypeData.GetCalloutText(SnakeType.Red), Does.Contain("Red"));
        Assert.That(SnakeTypeData.GetCalloutText(SnakeType.Purple), Does.Contain("Purple"));
    }

    [Test]
    public void GetCalloutText_ContainsHpInfo()
    {
        Assert.That(SnakeTypeData.GetCalloutText(SnakeType.Green), Does.Contain("1 HP"));
        Assert.That(SnakeTypeData.GetCalloutText(SnakeType.Yellow), Does.Contain("2 HP"));
        Assert.That(SnakeTypeData.GetCalloutText(SnakeType.Red), Does.Contain("3 HP"));
        Assert.That(SnakeTypeData.GetCalloutText(SnakeType.Purple), Does.Contain("4 HP"));
    }

    [Test]
    public void GetSize_ReturnsExpectedValues()
    {
        Assert.AreEqual(80f, SnakeTypeData.GetSize(SnakeType.Green));
        Assert.AreEqual(80f, SnakeTypeData.GetSize(SnakeType.Yellow));
        Assert.AreEqual(90f, SnakeTypeData.GetSize(SnakeType.Red));
        Assert.AreEqual(100f, SnakeTypeData.GetSize(SnakeType.Purple));
    }
}

public class DifficultyExtensionsTests
{
    [Test]
    public void GetPoints_ReturnsExpectedValues()
    {
        Assert.AreEqual(10, Difficulty.Easy.GetPoints());
        Assert.AreEqual(25, Difficulty.Medium.GetPoints());
        Assert.AreEqual(50, Difficulty.Hard.GetPoints());
        Assert.AreEqual(100, Difficulty.SuperHard.GetPoints());
    }

    [Test]
    public void GetStreakBonus_ReturnsExpectedValues()
    {
        Assert.AreEqual(5, Difficulty.Easy.GetStreakBonus());
        Assert.AreEqual(12, Difficulty.Medium.GetStreakBonus());
        Assert.AreEqual(25, Difficulty.Hard.GetStreakBonus());
        Assert.AreEqual(50, Difficulty.SuperHard.GetStreakBonus());
    }

    [Test]
    public void ToBankKey_ReturnsSnakeCaseStrings()
    {
        Assert.AreEqual("easy", Difficulty.Easy.ToBankKey());
        Assert.AreEqual("medium", Difficulty.Medium.ToBankKey());
        Assert.AreEqual("hard", Difficulty.Hard.ToBankKey());
        Assert.AreEqual("super_hard", Difficulty.SuperHard.ToBankKey());
    }

    [Test]
    public void ToSnakeSpellConfig_Easy_HasOnlyGreenSnakes()
    {
        var config = Difficulty.Easy.ToSnakeSpellConfig();
        Assert.AreEqual(1, config.availableSnakeTypes.Count);
        Assert.Contains(SnakeType.Green, config.availableSnakeTypes);
    }

    [Test]
    public void ToSnakeSpellConfig_SuperHard_HasAllSnakeTypes()
    {
        var config = Difficulty.SuperHard.ToSnakeSpellConfig();
        Assert.AreEqual(4, config.availableSnakeTypes.Count);
    }

    [Test]
    public void ToSnakeSpellConfig_HarderDifficulty_HasFasterSnakes()
    {
        float easySpeed = Difficulty.Easy.ToSnakeSpellConfig().baseSnakeSpeed;
        float hardSpeed = Difficulty.Hard.ToSnakeSpellConfig().baseSnakeSpeed;
        Assert.Greater(hardSpeed, easySpeed);
    }

    [Test]
    public void ToSnakeSpellConfig_HarderDifficulty_HasFewerLives()
    {
        int easyLives = Difficulty.Easy.ToSnakeSpellConfig().startingLives;
        int hardLives = Difficulty.Hard.ToSnakeSpellConfig().startingLives;
        Assert.GreaterOrEqual(easyLives, hardLives);
    }
}

public class SnakeDataTests
{
    [Test]
    public void Constructor_SetsAllFields()
    {
        var snake = new SnakeData(id: 5, angleDeg: 135f, type: SnakeType.Red,
            distance: 500f, hp: 3, speed: 60f);

        Assert.AreEqual(5, snake.id);
        Assert.AreEqual(135f, snake.angleDeg);
        Assert.AreEqual(SnakeType.Red, snake.type);
        Assert.AreEqual(500f, snake.distance);
        Assert.AreEqual(3, snake.hp);
        Assert.AreEqual(60f, snake.speed);
    }
}

public class SnakeSpellConstantsMessageTests
{
    [Test]
    public void GetLifeLossMessage_SingleLife_ReturnsSingular()
    {
        string msg = SnakeSpellConstants.GetLifeLossMessage(1);
        Assert.That(msg, Does.Contain("-1 life"));
        Assert.That(msg, Does.Not.Contain("lives"));
    }

    [Test]
    public void GetLifeLossMessage_MultipleLives_ReturnsPlural()
    {
        string msg = SnakeSpellConstants.GetLifeLossMessage(2);
        Assert.That(msg, Does.Contain("-2 lives"));
    }

    [Test]
    public void GetGameStartHint_ContainsLivesCount()
    {
        string msg = SnakeSpellConstants.GetGameStartHint(4);
        Assert.That(msg, Does.Contain("4 lives"));
    }

    [Test]
    public void GetGameStartHint_ContainsProtectMessage()
    {
        string msg = SnakeSpellConstants.GetGameStartHint(5);
        Assert.That(msg, Does.Contain("Protect the wizard"));
    }
}

public class BattlefieldStateTests
{
    [Test]
    public void DefaultState_HasCorrectDefaults()
    {
        var state = new BattlefieldState();
        Assert.AreEqual(GameStatus.NotStarted, state.status);
        Assert.AreEqual(5, state.lives);
        Assert.AreEqual(0, state.score);
        Assert.AreEqual(0, state.snakesKilled);
        Assert.NotNull(state.snakes);
        Assert.NotNull(state.spells);
        Assert.AreEqual(0, state.snakes.Count);
        Assert.AreEqual(0, state.spells.Count);
    }

    [Test]
    public void DefaultState_PhaseIsWavePhase()
    {
        var state = new BattlefieldState();
        Assert.AreEqual(GamePhase.WavePhase, state.phase);
    }

    [Test]
    public void DefaultState_HasNoLightningBolt()
    {
        var state = new BattlefieldState();
        Assert.IsFalse(state.hasLightningBolt);
    }
}
