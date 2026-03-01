using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Persistent stats
    public GameStats Stats { get; private set; }

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

    public int GetSnakeAccuracy()
    {
        if (LastSnakeQuestionsAnswered == 0) return 0;
        return (LastSnakeQuestionsCorrect * 100) / LastSnakeQuestionsAnswered;
    }
}
