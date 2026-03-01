using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewUIController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject reviewPanel;

    [Header("Content")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Navigation")]
    [SerializeField] private Button continueButton;

    private Action onContinueCallback;
    private readonly List<GameObject> dynamicCards = new List<GameObject>();

    void Start()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    public void Show(List<ReviewItem> items, Action onContinue)
    {
        onContinueCallback = onContinue;

        if (items == null || items.Count == 0)
        {
            onContinue?.Invoke();
            return;
        }

        if (titleText != null)
            titleText.text = $"Review — {items.Count} question{(items.Count > 1 ? "s" : "")} to revisit";

        ClearCards();

        foreach (var item in items)
            CreateReviewCard(item);

        if (reviewPanel != null)
            reviewPanel.SetActive(true);
    }

    public void Hide()
    {
        if (reviewPanel != null)
            reviewPanel.SetActive(false);

        ClearCards();
    }

    private void OnContinueClicked()
    {
        Hide();
        onContinueCallback?.Invoke();
    }

    private void ClearCards()
    {
        foreach (var card in dynamicCards)
        {
            if (card != null)
                Destroy(card);
        }
        dynamicCards.Clear();
    }

    private void CreateReviewCard(ReviewItem item)
    {
        // Card container
        var card = new GameObject("ReviewCard", typeof(RectTransform), typeof(Image),
            typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(LayoutElement));
        card.transform.SetParent(contentParent, false);

        var cardImage = card.GetComponent<Image>();
        cardImage.color = new Color(0.96f, 0.96f, 0.98f);

        var cardLayout = card.GetComponent<VerticalLayoutGroup>();
        cardLayout.padding = new RectOffset(24, 24, 16, 16);
        cardLayout.spacing = 8;
        cardLayout.childForceExpandWidth = true;
        cardLayout.childForceExpandHeight = false;
        cardLayout.childControlWidth = true;
        cardLayout.childControlHeight = true;

        var cardFitter = card.GetComponent<ContentSizeFitter>();
        cardFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var cardLayoutElem = card.GetComponent<LayoutElement>();
        cardLayoutElem.minHeight = 120;

        // Question text
        CreateCardText(card.transform, "QuestionText", item.questionText,
            24, AppColors.TextPrimary, FontStyles.Bold);

        // Student answer (wrong — red)
        CreateCardText(card.transform, "StudentAnswerText",
            $"Your answer: {item.studentAnswer}",
            20, AppColors.WrongRed, FontStyles.Normal);

        // Correct answer (green)
        CreateCardText(card.transform, "CorrectAnswerText",
            $"Correct answer: {item.correctAnswer}",
            20, AppColors.CorrectGreen, FontStyles.Normal);

        // Explanation
        if (!string.IsNullOrEmpty(item.explanation))
        {
            CreateCardText(card.transform, "ExplanationText",
                item.explanation,
                18, AppColors.TextSecondary, FontStyles.Italic);
        }

        dynamicCards.Add(card);
    }

    private void CreateCardText(Transform parent, string name, string text,
        int fontSize, Color color, FontStyles fontStyle)
    {
        var go = new GameObject(name, typeof(RectTransform),
            typeof(TextMeshProUGUI), typeof(ContentSizeFitter));
        go.transform.SetParent(parent, false);

        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.fontStyle = fontStyle;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.enableWordWrapping = true;

        var fitter = go.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
}
