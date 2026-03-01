using System.Collections.Generic;
using System.Linq;

public class AdaptiveDifficultyController
{
    private readonly float targetSuccessRate;
    private readonly float minSpawnRate;
    private readonly float maxSpawnRate;
    private readonly int windowSize;
    private readonly float adjustmentSpeed;

    private readonly Queue<bool> answerHistory;
    private float currentSpawnRate;

    public AdaptiveDifficultyController(
        float targetSuccessRate = 0.8f,
        float minSpawnRate = 0.022f,
        float maxSpawnRate = 2.0f,
        int windowSize = 20,
        float adjustmentSpeed = 0.1f)
    {
        this.targetSuccessRate = targetSuccessRate;
        this.minSpawnRate = minSpawnRate;
        this.maxSpawnRate = maxSpawnRate;
        this.windowSize = windowSize;
        this.adjustmentSpeed = adjustmentSpeed;

        answerHistory = new Queue<bool>();
        currentSpawnRate = minSpawnRate;
    }

    public void RecordAnswer(bool correct)
    {
        answerHistory.Enqueue(correct);

        if (answerHistory.Count > windowSize)
        {
            answerHistory.Dequeue();
        }

        float successRate = GetSuccessRate();
        currentSpawnRate += adjustmentSpeed * (successRate - targetSuccessRate);

        if (currentSpawnRate < minSpawnRate)
            currentSpawnRate = minSpawnRate;
        if (currentSpawnRate > maxSpawnRate)
            currentSpawnRate = maxSpawnRate;
    }

    public float GetSpawnRate()
    {
        return currentSpawnRate;
    }

    public float GetSuccessRate()
    {
        if (answerHistory.Count == 0)
            return 0f;

        int correctCount = answerHistory.Count(a => a);
        return (float)correctCount / answerHistory.Count;
    }
}
