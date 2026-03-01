using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds the review session panel to SnakeSpellScene.
/// Called after SceneBuilderSnake.Build() so the scene already exists.
/// </summary>
public static class SceneBuilderReview
{
    private static readonly Color DimBackground = new Color(0, 0, 0, 0.85f);
    private static readonly Color PanelColor = new Color(0.97f, 0.97f, 0.99f);
    private static readonly Color ButtonColor = AppColors.Purple60;

    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SnakeSpellScene.unity");

        // Find the existing Canvas
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogWarning("[Setup] SceneBuilderReview: Canvas not found in SnakeSpellScene");
            return;
        }

        // ── Review panel (full-screen overlay, starts hidden) ─────────
        var reviewPanel = new GameObject("ReviewPanel", typeof(RectTransform), typeof(Image));
        reviewPanel.transform.SetParent(canvas.transform, false);
        reviewPanel.GetComponent<Image>().color = DimBackground;
        UIFactory.Stretch(reviewPanel);
        reviewPanel.SetActive(false);

        // Inner card (centered, with padding from edges)
        var innerCard = UIFactory.CreatePanel(reviewPanel.transform, "ReviewCard", PanelColor);
        UIFactory.SetRect(innerCard, new Vector2(0.03f, 0.05f), new Vector2(0.97f, 0.95f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // Title
        var titleText = UIFactory.CreateTMP(innerCard.transform, "ReviewTitle",
            "Review", fontSize: 32, color: AppColors.TextPrimary);
        UIFactory.SetRect(titleText.gameObject, new Vector2(0, 0.92f), new Vector2(1, 1),
            new Vector2(0.5f, 1), Vector2.zero, Vector2.zero);

        // ── ScrollView ────────────────────────────────────────────────
        var scrollView = new GameObject("ScrollView", typeof(RectTransform),
            typeof(ScrollRect), typeof(Image));
        scrollView.transform.SetParent(innerCard.transform, false);
        scrollView.GetComponent<Image>().color = Color.clear;
        UIFactory.SetRect(scrollView, new Vector2(0, 0.10f), new Vector2(1, 0.90f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // Viewport (masks content)
        var viewport = new GameObject("Viewport", typeof(RectTransform),
            typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollView.transform, false);
        viewport.GetComponent<Image>().color = Color.white;
        viewport.GetComponent<Mask>().showMaskGraphic = false;
        UIFactory.Stretch(viewport);

        // Content (vertical layout — review cards go here at runtime)
        var content = new GameObject("Content", typeof(RectTransform),
            typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);

        var contentRT = content.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0, 0);

        var contentVLG = content.GetComponent<VerticalLayoutGroup>();
        contentVLG.padding = new RectOffset(16, 16, 8, 8);
        contentVLG.spacing = 16;
        contentVLG.childForceExpandWidth = true;
        contentVLG.childForceExpandHeight = false;
        contentVLG.childControlWidth = true;
        contentVLG.childControlHeight = true;

        var contentFitter = content.GetComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Wire ScrollRect
        var scrollRect = scrollView.GetComponent<ScrollRect>();
        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = contentRT;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;

        // ── Continue button ───────────────────────────────────────────
        var continueBtn = UIFactory.CreateButton(innerCard.transform, "ContinueButton",
            "Continue", ButtonColor, 28);
        UIFactory.SetRect(continueBtn.gameObject, new Vector2(0.15f, 0.01f), new Vector2(0.85f, 0.08f),
            new Vector2(0.5f, 0), Vector2.zero, Vector2.zero);

        // ── Controller ────────────────────────────────────────────────
        var ctrlGo = new GameObject("ReviewUIController");
        var comp = ctrlGo.AddComponent<ReviewUIController>();

        UIFactory.Wire(comp, "reviewPanel", reviewPanel);
        UIFactory.Wire(comp, "contentParent", content.GetComponent<RectTransform>());
        UIFactory.Wire(comp, "titleText", titleText);
        UIFactory.Wire(comp, "continueButton", continueBtn);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/SnakeSpellScene.unity");
        Debug.Log("[Setup] ReviewPanel added to SnakeSpellScene");
    }
}
