using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Persistent stats
    public GameStats Stats { get; private set; }

    // Navigation state (set before scene transitions)
    public Category SelectedCategory { get; set; }
    public Difficulty SelectedDifficulty { get; set; }
    public GameFlow SelectedFlow { get; set; }

    // Quiz results (set by QuizController before transitioning to ResultsScene)
    public int LastQuizScore { get; set; }
    public int LastQuizCorrectCount { get; set; }
    public int LastQuizTotalQuestions { get; set; }
    public int LastQuizStreak { get; set; }

    // Snake results (set by SnakeSpellController before transitioning)
    public int LastSnakeScore { get; set; }
    public int LastSnakeWave { get; set; }
    public int LastSnakeKills { get; set; }
    public int LastSnakeQuestionsAnswered { get; set; }
    public int LastSnakeQuestionsCorrect { get; set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadStats();
    }

    public void AddQuizResult(int score, int streak)
    {
        Stats.totalScore += score;
        Stats.gamesPlayed++;
        if (streak > Stats.bestStreak)
            Stats.bestStreak = streak;
        SaveStats();
    }

    public void SaveStats()
    {
        PlayerPrefs.SetInt("TotalScore", Stats.totalScore);
        PlayerPrefs.SetInt("GamesPlayed", Stats.gamesPlayed);
        PlayerPrefs.SetInt("BestStreak", Stats.bestStreak);
        PlayerPrefs.Save();
    }

    public void LoadStats()
    {
        Stats = new GameStats
        {
            totalScore = PlayerPrefs.GetInt("TotalScore", 0),
            gamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0),
            bestStreak = PlayerPrefs.GetInt("BestStreak", 0),
        };
    }

    public int GetQuizAccuracy()
    {
        if (LastQuizTotalQuestions == 0) return 0;
        return (LastQuizCorrectCount * 100) / LastQuizTotalQuestions;
    }

    public int GetQuizStarCount()
    {
        int accuracy = GetQuizAccuracy();
        if (accuracy >= 100) return 3;
        if (accuracy >= 60) return 2;
        if (accuracy >= 20) return 1;
        return 0;
    }

    public int GetSnakeAccuracy()
    {
        if (LastSnakeQuestionsAnswered == 0) return 0;
        return (LastSnakeQuestionsCorrect * 100) / LastSnakeQuestionsAnswered;
    }
}
