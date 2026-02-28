using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnakeSpellUIController : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Question Panel")]
    [SerializeField] private Image timerBar;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GameObject sequenceContainer;
    [SerializeField] private GameObject questionTextContainer;

    [Header("Answer Buttons")]
    [SerializeField] private List<AnswerButtonUI> answerButtons;

    [Header("Feedback")]
    [SerializeField] private FeedbackOverlay feedbackOverlay;

    private SnakeSpellController controller;

    void Start()
    {
        controller = FindObjectOfType<SnakeSpellController>();
    }

    void Update()
    {
        if (controller == null || controller.Battlefield == null) return;

        UpdateHUD();
        UpdateQuestionPanel();
        UpdateFeedback();
    }

    private void UpdateHUD()
    {
        BattlefieldState bf = controller.Battlefield;

        if (livesText != null)
        {
            livesText.text = $"\u2764 {bf.lives}";
            livesText.color = bf.lives <= 1 ? AppColors.HardRed : AppColors.TextPrimary;
        }

        if (waveText != null)
            waveText.text = $"Wave {bf.currentWave}";

        if (scoreText != null)
            scoreText.text = bf.score.ToString();
    }

    private void UpdateQuestionPanel()
    {
        GameQuestion.Standard q = controller.CurrentQuestion;
        if (q == null) return;
        Question question = q.question;

        // Timer
        if (timerBar != null)
        {
            float maxTime = controller.Config.questionTimerSeconds;
            timerBar.fillAmount = controller.QuestionTimeLeft / maxTime;
            timerBar.color = controller.QuestionTimeLeft > 3 ? AppColors.Purple60 : AppColors.HardRed;
        }

        if (timerText != null)
            timerText.text = $"{controller.QuestionTimeLeft}s";

        // Label
        if (labelText != null)
            labelText.text = question.label.ToUpper();

        // Sequence or text
        bool hasSequence = question.sequence != null && question.sequence.Count > 0;
        if (sequenceContainer != null) sequenceContainer.SetActive(hasSequence);
        if (questionTextContainer != null) questionTextContainer.SetActive(!hasSequence);

        if (!hasSequence && questionText != null)
            questionText.text = question.questionText ?? "";

        // Answer buttons
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < question.answers.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                string answer = question.answers[i];
                bool isCorrect = answer == question.correctAnswer;
                bool isSelected = controller.SelectedAnswerIndex == i;

                AnswerState state = AnswerState.Default;
                if (controller.ShowCorrect && isCorrect)
                    state = AnswerState.Correct;
                else if (isSelected && !isCorrect)
                    state = AnswerState.Wrong;

                answerButtons[i].SetAnswer(answer, state, !controller.Answered);

                int capturedIndex = i;
                answerButtons[i].SetClickAction(() => controller.SubmitAnswer(capturedIndex));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateFeedback()
    {
        if (feedbackOverlay != null && controller.ShowFeedback)
        {
            feedbackOverlay.Show(controller.FeedbackText, controller.FeedbackIsCorrect);
        }
    }
}
