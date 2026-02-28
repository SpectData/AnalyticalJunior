using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [Header("Stats Display")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI gamesPlayedText;
    [SerializeField] private TextMeshProUGUI bestStreakText;

    [Header("Category Buttons")]
    [SerializeField] private Button mathButton;
    [SerializeField] private Button logicButton;
    [SerializeField] private Button snakeSpellButton;

    void Start()
    {
        RefreshStats();

        mathButton.onClick.AddListener(OnMathClicked);
        logicButton.onClick.AddListener(OnLogicClicked);
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

    private void OnMathClicked()
    {
        GameManager.Instance.SelectedCategory = Category.Math;
        GameManager.Instance.SelectedFlow = GameFlow.Quiz;
        SceneTransitionManager.Instance.LoadDifficultySelect();
    }

    private void OnLogicClicked()
    {
        GameManager.Instance.SelectedCategory = Category.Logic;
        GameManager.Instance.SelectedFlow = GameFlow.Quiz;
        SceneTransitionManager.Instance.LoadDifficultySelect();
    }

    private void OnSnakeSpellClicked()
    {
        GameManager.Instance.SelectedFlow = GameFlow.SnakeSpell;
        SceneTransitionManager.Instance.LoadDifficultySelect();
    }
}
