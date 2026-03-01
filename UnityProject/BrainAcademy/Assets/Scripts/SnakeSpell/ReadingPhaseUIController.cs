using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the reading comprehension UI panel shown during inter-wave phases.
/// Displays a passage (title + scrollable body), a question, and answer buttons.
/// Tracks passage grouping via CurrentPassageId and reports results via OnAnswerSubmitted.
/// </summary>
public class ReadingPhaseUIController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject readingPanel;

    [Header("Passage Display")]
    [SerializeField] private TextMeshProUGUI passageTitleText;
    [SerializeField] private ScrollRect passageScrollRect;
    [SerializeField] private TextMeshProUGUI passageBodyText;

    [Header("Question")]
    [SerializeField] private TextMeshProUGUI questionText;

    [Header("Answer Buttons")]
    [SerializeField] private List<AnswerButtonUI> answerButtons;

    [Header("Feedback")]
    [SerializeField] private FeedbackOverlay feedbackOverlay;

    /// <summary>
    /// Fired when the player submits an answer. Bool = true if correct.
    /// </summary>
    public event Action<bool> OnAnswerSubmitted;

    private string currentPassageId;
    private GameQuestion.ReadingComprehension currentQuestion;
    private bool answered;

    /// <summary>
    /// Returns the current passage ID for grouping.
    /// The game loop uses this to request the next question for the same passage.
    /// </summary>
    public string CurrentPassageId => currentPassageId;

    /// <summary>
    /// Whether the panel is currently visible.
    /// </summary>
    public bool IsVisible => readingPanel != null && readingPanel.activeSelf;

    /// <summary>
    /// Shows the reading panel with the given question.
    /// Called by the game loop when entering inter-wave phase.
    /// </summary>
    public void ShowPanel(GameQuestion.ReadingComprehension question)
    {
        if (question == null) return;

        currentQuestion = question;
        currentPassageId = question.passageId;
        answered = false;

        if (passageTitleText != null)
            passageTitleText.text = question.passageTitle;

        if (passageBodyText != null)
            passageBodyText.text = question.passageText;

        if (passageScrollRect != null)
            passageScrollRect.verticalNormalizedPosition = 1f;

        if (questionText != null)
            questionText.text = question.question.questionText;

        UpdateAnswerButtons();

        if (readingPanel != null)
            readingPanel.SetActive(true);
    }

    /// <summary>
    /// Hides the reading panel.
    /// Called by the game loop when transitioning back to wave phase.
    /// </summary>
    public void HidePanel()
    {
        if (readingPanel != null)
            readingPanel.SetActive(false);

        currentQuestion = null;
    }

    private void UpdateAnswerButtons()
    {
        if (currentQuestion == null) return;
        Question q = currentQuestion.question;

        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < q.answers.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].SetAnswer(q.answers[i], AnswerState.Default, true);

                int capturedIndex = i;
                answerButtons[i].SetClickAction(() => SubmitAnswer(capturedIndex));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SubmitAnswer(int index)
    {
        if (answered) return;
        if (currentQuestion == null) return;

        answered = true;
        Question q = currentQuestion.question;
        bool isCorrect = q.answers[index] == q.correctAnswer;

        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i >= q.answers.Count) continue;

            bool isCorrectAnswer = q.answers[i] == q.correctAnswer;

            AnswerState state = AnswerState.Default;
            if (isCorrectAnswer)
                state = AnswerState.Correct;
            else if (i == index && !isCorrect)
                state = AnswerState.Wrong;

            answerButtons[i].SetAnswer(q.answers[i], state, false);
        }

        if (feedbackOverlay != null)
        {
            string feedbackMsg = isCorrect ? "Lightning Bolt earned!" : "No bonus this wave";
            feedbackOverlay.Show(feedbackMsg, isCorrect);
        }

        OnAnswerSubmitted?.Invoke(isCorrect);
    }
}
