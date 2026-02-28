using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum AnswerState
{
    Default,
    Correct,
    Wrong
}

public class AnswerButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private TextMeshProUGUI answerText;

    private UnityAction clickAction;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(() => clickAction?.Invoke());
    }

    public void SetAnswer(string text, AnswerState state, bool interactable)
    {
        if (answerText != null) answerText.text = text;
        button.interactable = interactable;

        switch (state)
        {
            case AnswerState.Correct:
                if (backgroundImage != null)
                    backgroundImage.color = new Color(
                        AppColors.EasyGreen.r, AppColors.EasyGreen.g, AppColors.EasyGreen.b, 0.15f);
                if (borderImage != null) borderImage.color = AppColors.EasyGreen;
                if (answerText != null) answerText.color = AppColors.EasyGreen;
                break;

            case AnswerState.Wrong:
                if (backgroundImage != null)
                    backgroundImage.color = new Color(
                        AppColors.HardRed.r, AppColors.HardRed.g, AppColors.HardRed.b, 0.15f);
                if (borderImage != null) borderImage.color = AppColors.HardRed;
                if (answerText != null) answerText.color = AppColors.HardRed;
                break;

            default:
                if (backgroundImage != null) backgroundImage.color = Color.white;
                if (borderImage != null) borderImage.color = AppColors.BorderLight;
                if (answerText != null) answerText.color = AppColors.TextPrimary;
                break;
        }
    }

    public void SetClickAction(UnityAction action)
    {
        clickAction = action;
    }
}
