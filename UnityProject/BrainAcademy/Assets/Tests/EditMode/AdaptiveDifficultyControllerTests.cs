using NUnit.Framework;

public class AdaptiveDifficultyControllerTests
{
    [Test]
    public void GetSpawnRate_Initial_ReturnsMinRate()
    {
        var controller = new AdaptiveDifficultyController();
        Assert.AreEqual(0.022f, controller.GetSpawnRate());
    }

    [Test]
    public void RecordAnswer_AllCorrect_IncreasesSpawnRate()
    {
        var controller = new AdaptiveDifficultyController();
        float initial = controller.GetSpawnRate();

        for (int i = 0; i < 20; i++)
            controller.RecordAnswer(true);

        Assert.Greater(controller.GetSpawnRate(), initial);
    }

    [Test]
    public void RecordAnswer_AllWrong_StaysAtMinRate()
    {
        var controller = new AdaptiveDifficultyController();

        for (int i = 0; i < 20; i++)
            controller.RecordAnswer(false);

        Assert.AreEqual(0.022f, controller.GetSpawnRate());
    }

    [Test]
    public void GetSpawnRate_ConvergesToTarget_WhenMixedAnswers()
    {
        var controller = new AdaptiveDifficultyController(
            targetSuccessRate: 0.8f,
            maxSpawnRate: 2.0f,
            adjustmentSpeed: 0.1f);

        // Feed exactly 80% correct answers repeatedly — rate should stabilise
        for (int round = 0; round < 10; round++)
        {
            for (int i = 0; i < 4; i++)
                controller.RecordAnswer(true);
            controller.RecordAnswer(false);
        }

        float rate = controller.GetSpawnRate();
        float rateSnapshot = rate;

        // One more round at 80% should barely change the rate
        for (int i = 0; i < 4; i++)
            controller.RecordAnswer(true);
        controller.RecordAnswer(false);

        float diff = System.Math.Abs(controller.GetSpawnRate() - rateSnapshot);
        Assert.Less(diff, 0.05f);
    }

    [Test]
    public void RecordAnswer_RollingWindow_DropsOldAnswers()
    {
        var controller = new AdaptiveDifficultyController(windowSize: 5);

        // Fill window with wrong answers
        for (int i = 0; i < 5; i++)
            controller.RecordAnswer(false);

        Assert.AreEqual(0f, controller.GetSuccessRate());

        // Now push 5 correct answers — old wrong answers drop out
        for (int i = 0; i < 5; i++)
            controller.RecordAnswer(true);

        Assert.AreEqual(1f, controller.GetSuccessRate());
    }

    [Test]
    public void GetSpawnRate_NeverExceedsMax()
    {
        var controller = new AdaptiveDifficultyController(
            maxSpawnRate: 1.0f,
            adjustmentSpeed: 10.0f);

        for (int i = 0; i < 100; i++)
            controller.RecordAnswer(true);

        Assert.LessOrEqual(controller.GetSpawnRate(), 1.0f);
    }

    [Test]
    public void GetSpawnRate_NeverBelowMin()
    {
        var controller = new AdaptiveDifficultyController(
            minSpawnRate: 0.5f,
            adjustmentSpeed: 10.0f);

        for (int i = 0; i < 100; i++)
            controller.RecordAnswer(false);

        Assert.GreaterOrEqual(controller.GetSpawnRate(), 0.5f);
    }

    [Test]
    public void GetSuccessRate_ReturnsCorrectRatio()
    {
        var controller = new AdaptiveDifficultyController(windowSize: 10);

        controller.RecordAnswer(true);
        controller.RecordAnswer(true);
        controller.RecordAnswer(false);
        controller.RecordAnswer(true);
        controller.RecordAnswer(false);

        // 3 correct out of 5
        Assert.AreEqual(0.6f, controller.GetSuccessRate(), 0.001f);
    }
}
