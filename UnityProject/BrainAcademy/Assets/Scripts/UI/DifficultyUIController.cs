using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyUIController : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Difficulty Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button superHardButton;

    void Start()
    {
        // Set title based on current flow
        if (titleText != null)
        {
            if (GameManager.Instance.SelectedFlow == GameFlow.SnakeSpell)
                titleText.text = "Snake Spellcaster \u2014 Select Difficulty";
            else
                titleText.text = GameManager.Instance.SelectedCategory == Category.Math
                    ? "Math \u2014 Select Difficulty"
                    : "Logic & Puzzles \u2014 Select Difficulty";
        }

        backButton.onClick.AddListener(OnBackClicked);
        easyButton.onClick.AddListener(() => OnDifficultySelected(Difficulty.Easy));
        mediumButton.onClick.AddListener(() => OnDifficultySelected(Difficulty.Medium));
        hardButton.onClick.AddListener(() => OnDifficultySelected(Difficulty.Hard));
        superHardButton.onClick.AddListener(() => OnDifficultySelected(Difficulty.SuperHard));
    }

    private void OnDifficultySelected(Difficulty difficulty)
    {
        GameManager.Instance.SelectedDifficulty = difficulty;

        if (GameManager.Instance.SelectedFlow == GameFlow.SnakeSpell)
            SceneTransitionManager.Instance.LoadSnakeSpell();
        else
            SceneTransitionManager.Instance.LoadQuizGame();
    }

    private void OnBackClicked()
    {
        SceneTransitionManager.Instance.LoadMenu();
    }
}
