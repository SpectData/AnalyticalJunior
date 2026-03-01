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
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GameObject sequenceContainer;
    [SerializeField] private GameObject questionTextContainer;

    [Header("Answer Buttons")]
    [SerializeField] private List<AnswerButtonUI> answerButtons;

    [Header("Lightning Bolt")]
    [SerializeField] private Button lightningBoltButton;

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

        bool inWavePhase = controller.Battlefield.status == GameStatus.Playing
                        && controller.Battlefield.phase == GamePhase.WavePhase;

        // Show question panel only during wave phase
        if (questionPanel != null)
            questionPanel.SetActive(inWavePhase);

        if (inWavePhase)
            UpdateQuestionPanel();

        UpdateLightningBoltButton();
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

    private void UpdateLightningBoltButton()
    {
        if (lightningBoltButton == null) return;

        bool show = controller.HasLightningBolt;
        lightningBoltButton.gameObject.SetActive(show);
    }

    private void UpdateFeedback()
    {
        if (feedbackOverlay != null && controller.ShowFeedback)
        {
            feedbackOverlay.Show(controller.FeedbackText, controller.FeedbackIsCorrect);
        }
    }
}
