using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds SnakeSpellScene with HUD, battlefield panel (3 lanes),
/// question panel, answer buttons, overlays, and feedback.
/// </summary>
public static class SceneBuilderSnake
{
    private static readonly Color Background = new Color(0.94f, 0.96f, 0.97f);
    private static readonly Color Purple60 = new Color(0.40f, 0.49f, 0.92f);
    private static readonly Color GrassLight = new Color(0.49f, 0.78f, 0.31f);  // #7EC850
    private static readonly Color GrassDark = new Color(0.36f, 0.68f, 0.23f);   // #5DAE3B
    private static readonly Color DirtRoad = AppColors.DirtRoad;

    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var canvas = UIFactory.CreateCanvas("Canvas");
        UIFactory.CreateBackground(canvas.transform, Background);

        // ── HUD panel (top) ─────────────────────────────────────────────
        var hud = UIFactory.CreatePanel(canvas.transform, "HUDPanel", new Color(0, 0, 0, 0.7f));
        UIFactory.SetRect(hud, new Vector2(0, 0.93f), new Vector2(1, 1),
            new Vector2(0.5f, 1), Vector2.zero, Vector2.zero);

        var livesText = UIFactory.CreateTMP(hud.transform, "LivesText",
            "\u2764 3", fontSize: 28, color: Color.white);
        UIFactory.SetRect(livesText.gameObject, new Vector2(0, 0), new Vector2(0.33f, 1),
            new Vector2(0, 0.5f), Vector2.zero, Vector2.zero);

        var waveText = UIFactory.CreateTMP(hud.transform, "WaveText",
            "Wave 1", fontSize: 28, color: Color.white);
        UIFactory.SetRect(waveText.gameObject, new Vector2(0.33f, 0), new Vector2(0.66f, 1),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var scoreText = UIFactory.CreateTMP(hud.transform, "ScoreText",
            "0", fontSize: 28, color: Color.white);
        UIFactory.SetRect(scoreText.gameObject, new Vector2(0.66f, 0), new Vector2(1, 1),
            new Vector2(1, 0.5f), Vector2.zero, Vector2.zero);

        // ── Battlefield panel (middle) ──────────────────────────────────
        var battlefield = new GameObject("BattlefieldPanel", typeof(RectTransform));
        battlefield.transform.SetParent(canvas.transform, false);
        var bfRT = UIFactory.SetRect(battlefield,
            new Vector2(0, 0.48f), new Vector2(1, 0.93f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // 3 lane backgrounds
        var laneImages = new Image[3];
        var dirtImages = new Image[3];
        var wizardRTs = new RectTransform[3];

        for (int i = 0; i < 3; i++)
        {
            float yMin = 1f - (i + 1) / 3f;
            float yMax = 1f - i / 3f;

            // Lane background
            bool evenLane = i % 2 == 0;
            var lane = UIFactory.CreateImage(battlefield.transform,
                $"Lane{i}", evenLane ? GrassLight : GrassDark,
                Vector2.zero);
            UIFactory.SetRect(lane, new Vector2(0, yMin), new Vector2(1, yMax),
                new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var laneImg = lane.GetComponent<Image>();
            Sprite grassSprite = evenLane ? PrefabFactory.GrassLightSprite : PrefabFactory.GrassDarkSprite;
            if (grassSprite != null)
            {
                laneImg.sprite = grassSprite;
                laneImg.color = Color.white;
            }
            laneImages[i] = laneImg;

            // Dirt road (lane divider strip at bottom of lane)
            var dirt = UIFactory.CreateImage(battlefield.transform,
                $"DirtRoad{i}", DirtRoad, Vector2.zero);
            UIFactory.SetRect(dirt, new Vector2(0, yMin), new Vector2(1, yMin + 0.04f),
                new Vector2(0.5f, 0), Vector2.zero, Vector2.zero);
            dirtImages[i] = dirt.GetComponent<Image>();

            // Wizard (left side of each lane)
            var wizard = new GameObject($"Wizard{i}", typeof(Image));
            wizard.transform.SetParent(battlefield.transform, false);
            var wizImg = wizard.GetComponent<Image>();
            wizImg.preserveAspect = true;
            if (PrefabFactory.WizardSprite != null)
            {
                wizImg.sprite = PrefabFactory.WizardSprite;
                wizImg.color = Color.white;
            }
            else
            {
                wizImg.color = Purple60;
            }
            // Flip horizontally so wizard faces right (toward snakes)
            wizard.transform.localScale = new Vector3(-1, 1, 1);
            var wrt = UIFactory.SetRect(wizard,
                new Vector2(0, yMin), new Vector2(0, yMax),
                new Vector2(0, 0.5f), new Vector2(50, 0), new Vector2(90, 0));
            wizardRTs[i] = wrt;
        }

        // Wave transition overlay
        var waveOverlay = new GameObject("WaveTransitionOverlay", typeof(RectTransform));
        waveOverlay.transform.SetParent(battlefield.transform, false);
        UIFactory.Stretch(waveOverlay);
        waveOverlay.SetActive(false);

        var bgDim = UIFactory.CreateImage(waveOverlay.transform, "Dim",
            new Color(0, 0, 0, 0.5f), Vector2.zero);
        UIFactory.Stretch(bgDim);

        var waveTransText = UIFactory.CreateTMP(waveOverlay.transform,
            "WaveTransitionText", "Wave 1 Complete!",
            fontSize: 40, color: Color.white);
        UIFactory.Stretch(waveTransText.gameObject);

        // Game over overlay
        var gameOverOverlay = new GameObject("GameOverOverlay", typeof(RectTransform));
        gameOverOverlay.transform.SetParent(battlefield.transform, false);
        UIFactory.Stretch(gameOverOverlay);
        gameOverOverlay.SetActive(false);

        var goDim = UIFactory.CreateImage(gameOverOverlay.transform, "Dim",
            new Color(0, 0, 0, 0.6f), Vector2.zero);
        UIFactory.Stretch(goDim);

        UIFactory.CreateTMP(gameOverOverlay.transform, "GameOverText",
            "Game Over!", fontSize: 48, color: Color.white);

        // ── Question panel (bottom) ─────────────────────────────────────
        var qPanel = UIFactory.CreatePanel(canvas.transform, "QuestionPanel", Color.white);
        UIFactory.SetRect(qPanel, new Vector2(0, 0), new Vector2(1, 0.48f),
            new Vector2(0.5f, 0), Vector2.zero, Vector2.zero);

        // Timer bar
        var timerBar = UIFactory.CreateFilledImage(qPanel.transform, "TimerBar", Purple60);
        UIFactory.SetRect(timerBar, new Vector2(0.03f, 0.90f), new Vector2(0.85f, 0.96f),
            new Vector2(0, 1), Vector2.zero, Vector2.zero);

        var timerText = UIFactory.CreateTMP(qPanel.transform, "TimerText",
            "30s", fontSize: 22, color: Color.gray);
        UIFactory.SetRect(timerText.gameObject, new Vector2(0.87f, 0.90f), new Vector2(0.97f, 0.96f),
            new Vector2(1, 1), Vector2.zero, Vector2.zero);

        // Label
        var labelText = UIFactory.CreateTMP(qPanel.transform, "LabelText",
            "MATH", fontSize: 20, color: Purple60);
        UIFactory.SetRect(labelText.gameObject, new Vector2(0.03f, 0.80f), new Vector2(0.97f, 0.89f),
            new Vector2(0, 1), Vector2.zero, Vector2.zero);

        // Question text container
        var qtContainer = new GameObject("QuestionTextContainer", typeof(RectTransform));
        qtContainer.transform.SetParent(qPanel.transform, false);
        UIFactory.SetRect(qtContainer, new Vector2(0.03f, 0.55f), new Vector2(0.97f, 0.80f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var questionText = UIFactory.CreateTMP(qtContainer.transform, "QuestionText",
            "Question...", fontSize: 36, color: Color.black);
        UIFactory.Stretch(questionText.gameObject);

        // Sequence container
        var seqContainer = new GameObject("SequenceContainer",
            typeof(HorizontalLayoutGroup));
        seqContainer.transform.SetParent(qPanel.transform, false);
        UIFactory.SetRect(seqContainer, new Vector2(0.03f, 0.55f), new Vector2(0.97f, 0.80f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        seqContainer.SetActive(false);

        // Answer buttons
        var ansContainer = new GameObject("AnswerButtons", typeof(VerticalLayoutGroup));
        ansContainer.transform.SetParent(qPanel.transform, false);
        UIFactory.SetRect(ansContainer, new Vector2(0.03f, 0.05f), new Vector2(0.97f, 0.52f),
            new Vector2(0.5f, 0), Vector2.zero, Vector2.zero);

        var vlg = ansContainer.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 8;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        var answerButtons = UIFactory.CreateAnswerButtons(
            ansContainer.transform, PrefabFactory.AnswerButtonPrefab, 4);

        // Feedback overlay
        var (fbGo, fbComp, _) = UIFactory.CreateFeedbackOverlay(canvas.transform);

        // ── Controllers ─────────────────────────────────────────────────
        var ssCtrlGo = new GameObject("SnakeSpellController");
        ssCtrlGo.AddComponent<SnakeSpellController>();

        var uiCtrlGo = new GameObject("SnakeSpellUIController");
        var ssUI = uiCtrlGo.AddComponent<SnakeSpellUIController>();

        UIFactory.Wire(ssUI, "livesText", livesText);
        UIFactory.Wire(ssUI, "waveText", waveText);
        UIFactory.Wire(ssUI, "scoreText", scoreText);
        UIFactory.Wire(ssUI, "timerBar", timerBar.GetComponent<Image>());
        UIFactory.Wire(ssUI, "timerText", timerText);
        UIFactory.Wire(ssUI, "labelText", labelText);
        UIFactory.Wire(ssUI, "questionText", questionText);
        UIFactory.Wire(ssUI, "sequenceContainer", seqContainer);
        UIFactory.Wire(ssUI, "questionTextContainer", qtContainer);
        UIFactory.WireList(ssUI, "answerButtons",
            new Object[] { answerButtons[0], answerButtons[1],
                           answerButtons[2], answerButtons[3] });
        UIFactory.Wire(ssUI, "feedbackOverlay", fbComp);

        // Battlefield renderer
        var bfRendGo = new GameObject("BattlefieldRenderer");
        var bfRend = bfRendGo.AddComponent<BattlefieldRenderer>();

        UIFactory.Wire(bfRend, "battlefieldPanel", bfRT);
        UIFactory.WireList(bfRend, "laneBackgrounds",
            new Object[] { laneImages[0], laneImages[1], laneImages[2] });
        UIFactory.WireList(bfRend, "dirtRoads",
            new Object[] { dirtImages[0], dirtImages[1], dirtImages[2] });
        UIFactory.WireList(bfRend, "wizardObjects",
            new Object[] { wizardRTs[0], wizardRTs[1], wizardRTs[2] });
        UIFactory.Wire(bfRend, "snakePrefab", PrefabFactory.SnakePrefab);
        UIFactory.Wire(bfRend, "spellPrefab", PrefabFactory.SpellPrefab);
        UIFactory.Wire(bfRend, "waveTransitionOverlay", waveOverlay);
        UIFactory.Wire(bfRend, "waveTransitionText", waveTransText);
        UIFactory.Wire(bfRend, "gameOverOverlay", gameOverOverlay);

        // Wire sprites (null-safe — fields stay null if sprites not yet generated)
        if (PrefabFactory.WizardSprite != null)
            UIFactory.Wire(bfRend, "wizardSprite", PrefabFactory.WizardSprite);
        if (PrefabFactory.SnakeGreenSprite != null)
            UIFactory.Wire(bfRend, "snakeGreenSprite", PrefabFactory.SnakeGreenSprite);
        if (PrefabFactory.SnakeYellowSprite != null)
            UIFactory.Wire(bfRend, "snakeYellowSprite", PrefabFactory.SnakeYellowSprite);
        if (PrefabFactory.SnakeRedSprite != null)
            UIFactory.Wire(bfRend, "snakeRedSprite", PrefabFactory.SnakeRedSprite);
        if (PrefabFactory.SnakePurpleSprite != null)
            UIFactory.Wire(bfRend, "snakePurpleSprite", PrefabFactory.SnakePurpleSprite);
        if (PrefabFactory.SpellSprite != null)
            UIFactory.Wire(bfRend, "spellSprite", PrefabFactory.SpellSprite);
        if (PrefabFactory.GrassLightSprite != null)
            UIFactory.Wire(bfRend, "grassLightSprite", PrefabFactory.GrassLightSprite);
        if (PrefabFactory.GrassDarkSprite != null)
            UIFactory.Wire(bfRend, "grassDarkSprite", PrefabFactory.GrassDarkSprite);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/SnakeSpellScene.unity");
        Debug.Log("[Setup] SnakeSpellScene built");
    }
}
