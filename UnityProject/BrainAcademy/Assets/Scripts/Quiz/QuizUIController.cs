using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizUIController : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI questionCounterText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image progressBar;

    [Header("Question Card")]
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GameObject sequenceContainer;
    [SerializeField] private GameObject questionTextContainer;

    [Header("Answer Buttons")]
    [SerializeField] private List<AnswerButtonUI> answerButtons;

    [Header("Memory Grid")]
    [SerializeField] private GameObject memoryGridContainer;
    [SerializeField] private MemoryGridUI memoryGridUI;

    [Header("Feedback")]
    [SerializeField] private FeedbackOverlay feedbackOverlay;

    private QuizController controller;

    void Start()
    {
        controller = FindObjectOfType<QuizController>();
    }

    void Update()
    {
        if (controller == null) return;

        UpdateHUD();
        UpdateQuestionDisplay();
        UpdateFeedback();
    }

    private void UpdateHUD()
    {
        if (timeLeftText != null)
        {
            timeLeftText.text = controller.TimeLeft.ToString();
            timeLeftText.color = controller.TimeLeft <= 5 ? AppColors.HardRed : AppColors.TextPrimary;
        }

        if (questionCounterText != null)
            questionCounterText.text = $"{controller.CurrentQuestionIndex} / {QuizController.TotalQuestions}";

        if (scoreText != null)
            scoreText.text = $"Score: {controller.GameScore}";

        if (progressBar != null)
            progressBar.fillAmount = (controller.CurrentQuestionIndex - 1f) / QuizController.TotalQuestions;
    }

    private void UpdateQuestionDisplay()
    {
        GameQuestion q = controller.CurrentQuestion;

        if (q is GameQuestion.Standard stdQ)
        {
            Question question = stdQ.question;

            if (memoryGridContainer != null) memoryGridContainer.SetActive(false);

            if (labelText != null) labelText.text = question.label.ToUpper();

            // Show sequence or question text
            bool hasSequence = question.sequence != null && question.sequence.Count > 0;
            if (sequenceContainer != null) sequenceContainer.SetActive(hasSequence);
            if (questionTextContainer != null) questionTextContainer.SetActive(!hasSequence);

            if (!hasSequence && questionText != null)
                questionText.text = question.questionText ?? "";

            // Update answer buttons
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
                    answerButtons[i].SetClickAction(() => controller.SelectAnswer(capturedIndex));
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else if (q is GameQuestion.Memory memQ)
        {
            if (memoryGridContainer != null) memoryGridContainer.SetActive(true);
            if (sequenceContainer != null) sequenceContainer.SetActive(false);
            if (questionTextContainer != null) questionTextContainer.SetActive(true);

            if (labelText != null) labelText.text = "MEMORY GRID";

            if (questionText != null)
            {
                questionText.text = controller.MemoryShowPhase
                    ? "Remember the highlighted cells!"
                    : $"Tap the {memQ.memoryQuestion.highlighted.Count} cells that were highlighted!";
            }

            // Hide standard answer buttons
            foreach (var btn in answerButtons)
                btn.gameObject.SetActive(false);

            if (memoryGridUI != null)
            {
                memoryGridUI.UpdateGrid(
                    memQ.memoryQuestion,
                    controller.MemoryShowPhase,
                    controller.MemorySelectedCells,
                    controller.MemoryDone,
                    controller.Answered,
                    (index) => controller.SelectMemoryCell(index)
                );
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
