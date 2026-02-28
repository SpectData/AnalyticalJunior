using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnakeResultsUIController : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI emojiText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI wavesText;
    [SerializeField] private TextMeshProUGUI snakesText;
    [SerializeField] private TextMeshProUGUI questionsText;
    [SerializeField] private TextMeshProUGUI accuracyText;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button menuButton;

    void Start()
    {
        GameManager gm = GameManager.Instance;

        if (titleText != null) titleText.text = "Game Over!";
        if (emojiText != null) emojiText.text = "\uD83D\uDC0D\u2728";
        if (scoreText != null) scoreText.text = $"{gm.LastSnakeScore} points";
        if (wavesText != null) wavesText.text = gm.LastSnakeWave.ToString();
        if (snakesText != null) snakesText.text = gm.LastSnakeKills.ToString();
        if (questionsText != null) questionsText.text = gm.LastSnakeQuestionsAnswered.ToString();
        if (accuracyText != null) accuracyText.text = $"{gm.GetSnakeAccuracy()}%";

        playAgainButton.onClick.AddListener(OnPlayAgain);
        menuButton.onClick.AddListener(OnMenu);
    }

    private void OnPlayAgain()
    {
        SceneTransitionManager.Instance.LoadSnakeSpell();
    }

    private void OnMenu()
    {
        SceneTransitionManager.Instance.LoadMenu();
    }
}
