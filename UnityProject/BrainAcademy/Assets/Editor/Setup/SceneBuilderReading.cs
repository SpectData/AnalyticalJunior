using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds the Reading Comprehension panel overlay to SnakeSpellScene.
/// Must run AFTER SceneBuilderSnake.Build().
/// </summary>
public static class SceneBuilderReading
{
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SnakeSpellScene.unity");

        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("[Setup] SceneBuilderReading: Canvas not found in SnakeSpellScene");
            return;
        }

        // ── Reading Panel (full-screen overlay, starts hidden) ────────────
        var readingPanel = new GameObject("ReadingPanel", typeof(RectTransform));
        readingPanel.transform.SetParent(canvas.transform, false);
        UIFactory.Stretch(readingPanel);
        readingPanel.SetActive(false);

        // Dim background
        var dim = UIFactory.CreateImage(readingPanel.transform, "DimBackground",
            new Color(0, 0, 0, 0.6f), Vector2.zero);
        UIFactory.Stretch(dim);

        // Content panel (white card with margins)
        var content = UIFactory.CreatePanel(readingPanel.transform,
            "ContentPanel", Color.white);
        UIFactory.SetRect(content,
            new Vector2(0.03f, 0.02f), new Vector2(0.97f, 0.98f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // ── Passage title ─────────────────────────────────────────────────
        var titleText = UIFactory.CreateTMP(content.transform, "PassageTitleText",
            "Passage Title", fontSize: 28, color: AppColors.Purple60,
            align: TextAlignmentOptions.Left);
        UIFactory.SetRect(titleText.gameObject,
            new Vector2(0.04f, 0.90f), new Vector2(0.96f, 0.97f),
            new Vector2(0, 1), Vector2.zero, Vector2.zero);

        // ── Scrollable passage area ───────────────────────────────────────
        var (scrollRect, bodyText) = CreatePassageScrollView(content.transform);

        // ── Divider ───────────────────────────────────────────────────────
        var divider = UIFactory.CreateImage(content.transform, "Divider",
            AppColors.BorderLight, Vector2.zero);
        UIFactory.SetRect(divider,
            new Vector2(0.04f, 0.44f), new Vector2(0.96f, 0.445f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // ── Question text ─────────────────────────────────────────────────
        var qText = UIFactory.CreateTMP(content.transform, "QuestionText",
            "Question goes here...", fontSize: 26, color: Color.black,
            align: TextAlignmentOptions.TopLeft);
        UIFactory.SetRect(qText.gameObject,
            new Vector2(0.04f, 0.33f), new Vector2(0.96f, 0.43f),
            new Vector2(0, 1), Vector2.zero, Vector2.zero);

        // ── Answer buttons ────────────────────────────────────────────────
        var ansContainer = new GameObject("ReadingAnswerButtons",
            typeof(VerticalLayoutGroup));
        ansContainer.transform.SetParent(content.transform, false);
        UIFactory.SetRect(ansContainer,
            new Vector2(0.04f, 0.03f), new Vector2(0.96f, 0.32f),
            new Vector2(0.5f, 0), Vector2.zero, Vector2.zero);

        var vlg = ansContainer.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 8;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        var answerButtons = UIFactory.CreateAnswerButtons(
            ansContainer.transform, PrefabFactory.AnswerButtonPrefab, 4);

        // ── Feedback overlay ──────────────────────────────────────────────
        var (fbGo, fbComp, _) = UIFactory.CreateFeedbackOverlay(readingPanel.transform);

        // ── Controller ────────────────────────────────────────────────────
        var ctrlGo = new GameObject("ReadingPhaseUIController");
        var ctrl = ctrlGo.AddComponent<ReadingPhaseUIController>();

        UIFactory.Wire(ctrl, "readingPanel", readingPanel);
        UIFactory.Wire(ctrl, "passageTitleText", titleText);
        UIFactory.Wire(ctrl, "passageScrollRect", scrollRect);
        UIFactory.Wire(ctrl, "passageBodyText", bodyText);
        UIFactory.Wire(ctrl, "questionText", qText);
        UIFactory.WireList(ctrl, "answerButtons",
            new Object[] { answerButtons[0], answerButtons[1],
                           answerButtons[2], answerButtons[3] });
        UIFactory.Wire(ctrl, "feedbackOverlay", fbComp);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/SnakeSpellScene.unity");
        Debug.Log("[Setup] Reading comprehension panel added to SnakeSpellScene");
    }

    private static (ScrollRect scrollRect, TextMeshProUGUI bodyText)
        CreatePassageScrollView(Transform parent)
    {
        // ScrollView root with mask
        var scrollGo = new GameObject("PassageScrollView",
            typeof(ScrollRect), typeof(Image), typeof(Mask));
        scrollGo.transform.SetParent(parent, false);
        UIFactory.SetRect(scrollGo,
            new Vector2(0.04f, 0.45f), new Vector2(0.96f, 0.89f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var scrollImg = scrollGo.GetComponent<Image>();
        scrollImg.color = new Color(0.97f, 0.97f, 0.97f, 1f);
        scrollGo.GetComponent<Mask>().showMaskGraphic = true;

        var sr = scrollGo.GetComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Clamped;

        // Viewport
        var viewport = new GameObject("Viewport", typeof(RectTransform));
        viewport.transform.SetParent(scrollGo.transform, false);
        UIFactory.Stretch(viewport);
        sr.viewport = viewport.GetComponent<RectTransform>();

        // Content (grows with text via ContentSizeFitter)
        var contentGo = new GameObject("Content",
            typeof(RectTransform), typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter));
        contentGo.transform.SetParent(viewport.transform, false);

        var contentRT = contentGo.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0, 0);

        var csf = contentGo.GetComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var contentVlg = contentGo.GetComponent<VerticalLayoutGroup>();
        contentVlg.padding = new RectOffset(8, 8, 8, 8);
        contentVlg.childForceExpandWidth = true;
        contentVlg.childForceExpandHeight = false;
        contentVlg.childControlWidth = true;
        contentVlg.childControlHeight = true;

        // Passage body text
        var bodyText = UIFactory.CreateTMP(contentGo.transform, "PassageBodyText",
            "Passage text goes here...", fontSize: 22, color: Color.black,
            align: TextAlignmentOptions.TopLeft);
        bodyText.enableWordWrapping = true;

        sr.content = contentGo.GetComponent<RectTransform>();

        return (sr, bodyText);
    }
}
