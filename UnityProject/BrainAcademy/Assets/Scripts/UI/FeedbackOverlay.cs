using System.Collections;
using TMPro;
using UnityEngine;

public class FeedbackOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private CanvasGroup canvasGroup;

    private Coroutine fadeCoroutine;
    private bool isShowing;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    public void Show(string text, bool isCorrect)
    {
        if (isShowing) return;
        isShowing = true;

        if (feedbackText != null)
        {
            feedbackText.text = text;
            feedbackText.color = isCorrect ? AppColors.CorrectGreen : AppColors.WrongRed;
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInOut());
    }

    private IEnumerator FadeInOut()
    {
        // Fade in
        float elapsed = 0f;
        float fadeInTime = 0.15f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null) canvasGroup.alpha = elapsed / fadeInTime;
            yield return null;
        }
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(0.5f);

        // Fade out
        elapsed = 0f;
        float fadeOutTime = 0.15f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null) canvasGroup.alpha = 1f - (elapsed / fadeOutTime);
            yield return null;
        }
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        isShowing = false;
    }
}
