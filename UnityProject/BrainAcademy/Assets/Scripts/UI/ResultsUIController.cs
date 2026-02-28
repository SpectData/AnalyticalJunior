using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUIController : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI correctCountText;
    [SerializeField] private TextMeshProUGUI accuracyText;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button menuButton;

    void Start()
    {
        GameManager gm = GameManager.Instance;

        if (titleText != null) titleText.text = "Round Complete!";

        // Stars
        int starCount = gm.GetQuizStarCount();
        string starStr = new string('\u2B50', starCount) + new string('\u2606', 3 - starCount);
        if (starsText != null) starsText.text = starStr;

        if (scoreText != null) scoreText.text = $"{gm.LastQuizScore} points";
        if (correctCountText != null)
            correctCountText.text = $"{gm.LastQuizCorrectCount} / {gm.LastQuizTotalQuestions} correct";
        if (accuracyText != null)
            accuracyText.text = $"{gm.GetQuizAccuracy()}% accuracy";

        playAgainButton.onClick.AddListener(OnPlayAgain);
        menuButton.onClick.AddListener(OnMenu);
    }

    private void OnPlayAgain()
    {
        SceneTransitionManager.Instance.LoadQuizGame();
    }

    private void OnMenu()
    {
        SceneTransitionManager.Instance.LoadMenu();
    }
}
