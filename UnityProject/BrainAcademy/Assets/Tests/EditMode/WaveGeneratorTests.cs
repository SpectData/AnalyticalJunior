using NUnit.Framework;

public class WaveGeneratorTests
{
    [Test]
    public void GetSnakeCountForWave_Wave1_Returns4()
    {
        Assert.AreEqual(4, WaveGenerator.GetSnakeCountForWave(1));
    }

    [Test]
    public void GetSnakeCountForWave_Wave5_Returns8()
    {
        Assert.AreEqual(8, WaveGenerator.GetSnakeCountForWave(5));
    }

    [Test]
    public void GetSnakeCountForWave_CapsAt12()
    {
        Assert.AreEqual(12, WaveGenerator.GetSnakeCountForWave(20));
        Assert.AreEqual(12, WaveGenerator.GetSnakeCountForWave(100));
    }

    [Test]
    public void GetSpawnInterval_Wave1_ReturnsBaseInterval()
    {
        var config = Difficulty.Easy.ToSnakeSpellConfig();
        float interval = WaveGenerator.GetSpawnInterval(1, config);
        Assert.AreEqual(config.baseSpawnInterval, interval);
    }

    [Test]
    public void GetSpawnInterval_HigherWave_ReturnsSmallerInterval()
    {
        var config = Difficulty.Easy.ToSnakeSpellConfig();
        float wave1 = WaveGenerator.GetSpawnInterval(1, config);
        float wave5 = WaveGenerator.GetSpawnInterval(5, config);
        Assert.Less(wave5, wave1);
    }

    [Test]
    public void GetSpawnInterval_NeverBelowMinimum()
    {
        var config = Difficulty.Easy.ToSnakeSpellConfig();
        float interval = WaveGenerator.GetSpawnInterval(1000, config);
        Assert.GreaterOrEqual(interval, 1.0f);
    }

    [Test]
    public void GetSnakeSpeed_GreenWave1_ReturnsBaseSpeed()
    {
        var config = Difficulty.Easy.ToSnakeSpellConfig();
        float speed = WaveGenerator.GetSnakeSpeed(SnakeType.Green, 1, config);
        Assert.AreEqual(config.baseSnakeSpeed, speed);
    }

    [Test]
    public void GetSnakeSpeed_HigherWave_IsFaster()
    {
        var config = Difficulty.Easy.ToSnakeSpellConfig();
        float wave1 = WaveGenerator.GetSnakeSpeed(SnakeType.Green, 1, config);
        float wave5 = WaveGenerator.GetSnakeSpeed(SnakeType.Green, 5, config);
        Assert.Greater(wave5, wave1);
    }

    [Test]
    public void GetSnakeSpeed_RedSnake_IsSlowerThanGreen()
    {
        var config = Difficulty.Hard.ToSnakeSpellConfig();
        float green = WaveGenerator.GetSnakeSpeed(SnakeType.Green, 1, config);
        float red = WaveGenerator.GetSnakeSpeed(SnakeType.Red, 1, config);
        Assert.Less(red, green);
    }

    [Test]
    public void GetSnakeSpeed_PurpleSnake_IsFasterThanGreen()
    {
        var config = Difficulty.SuperHard.ToSnakeSpellConfig();
        float green = WaveGenerator.GetSnakeSpeed(SnakeType.Green, 1, config);
        float purple = WaveGenerator.GetSnakeSpeed(SnakeType.Purple, 1, config);
        Assert.Greater(purple, green);
    }

    [Test]
    public void PickSnakeType_Wave1_AlwaysReturnsGreen()
    {
        var config = Difficulty.SuperHard.ToSnakeSpellConfig();
        Assert.AreEqual(SnakeType.Green, WaveGenerator.PickSnakeType(1, config));
    }
}
