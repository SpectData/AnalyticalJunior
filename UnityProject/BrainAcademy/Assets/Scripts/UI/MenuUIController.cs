using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI gamesPlayedText;
    [SerializeField] private TextMeshProUGUI bestStreakText;

    [Header("Game Button")]
    [SerializeField] private Button snakeSpellButton;

    void Start()
    {
        RefreshStats();
        snakeSpellButton.onClick.AddListener(OnSnakeSpellClicked);
    }

    void OnEnable()
    {
        RefreshStats();
    }

    private void RefreshStats()
    {
        if (GameManager.Instance == null) return;

        GameStats stats = GameManager.Instance.Stats;
        if (totalScoreText != null) totalScoreText.text = stats.totalScore.ToString();
        if (gamesPlayedText != null) gamesPlayedText.text = stats.gamesPlayed.ToString();
        if (bestStreakText != null) bestStreakText.text = stats.bestStreak.ToString();
    }

    private void OnSnakeSpellClicked()
    {
        SceneTransitionManager.Instance.LoadDifficultySelect();
    }
}
