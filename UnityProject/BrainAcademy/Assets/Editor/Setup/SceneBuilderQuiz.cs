using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds QuizGameScene with HUD, question card, answer buttons,
/// memory grid, and feedback overlay.
/// </summary>
public static class SceneBuilderQuiz
{
    private static readonly Color Background = new Color(0.94f, 0.96f, 0.97f);
    private static readonly Color Purple60 = new Color(0.40f, 0.49f, 0.92f);

    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var canvas = UIFactory.CreateCanvas("Canvas");
        UIFactory.CreateBackground(canvas.transform, Background);

        // ── HUD card ────────────────────────────────────────────────────
        var hud = UIFactory.CreatePanel(canvas.transform, "HUDCard", Color.white);
        UIFactory.SetRect(hud, new Vector2(0.03f, 0.90f), new Vector2(0.97f, 0.97f),
            new Vector2(0.5f, 1), Vector2.zero, Vector2.zero);

        var timeLeft = UIFactory.CreateTMP(hud.transform, "TimeLeftText",
            "30", fontSize: 32, color: Color.black);
        UIFactory.SetRect(timeLeft.gameObject, new Vector2(0, 0), new Vector2(0.25f, 1),
            new Vector2(0, 0.5f), Vector2.zero, Vector2.zero);

        var counter = UIFactory.CreateTMP(hud.transform, "QuestionCounterText",
            "1 / 5", fontSize: 28, color: Color.gray);
        UIFactory.SetRect(counter.gameObject, new Vector2(0.25f, 0), new Vector2(0.75f, 1),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var scoreText = UIFactory.CreateTMP(hud.transform, "ScoreText",
            "Score: 0", fontSize: 28, color: Purple60);
        UIFactory.SetRect(scoreText.gameObject, new Vector2(0.75f, 0), new Vector2(1, 1),
            new Vector2(1, 0.5f), Vector2.zero, Vector2.zero);

        // ── Progress bar ────────────────────────────────────────────────
        var progressBar = UIFactory.CreateFilledImage(canvas.transform, "ProgressBar", Purple60);
        UIFactory.SetRect(progressBar, new Vector2(0.03f, 0.885f), new Vector2(0.97f, 0.895f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // ── Question card ───────────────────────────────────────────────
        var qCard = UIFactory.CreatePanel(canvas.transform, "QuestionCard", Color.white);
        UIFactory.SetRect(qCard, new Vector2(0.03f, 0.60f), new Vector2(0.97f, 0.88f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var labelText = UIFactory.CreateTMP(qCard.transform, "LabelText",
            "MATH", fontSize: 22, color: Purple60);
        UIFactory.SetRect(labelText.gameObject, new Vector2(0, 0.80f), new Vector2(1, 1),
            new Vector2(0.5f, 1), Vector2.zero, Vector2.zero);

        // Question text container
        var qtContainer = new GameObject("QuestionTextContainer", typeof(RectTransform));
        qtContainer.transform.SetParent(qCard.transform, false);
        UIFactory.SetRect(qtContainer, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.78f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var questionText = UIFactory.CreateTMP(qtContainer.transform, "QuestionText",
            "Question goes here...", fontSize: 28, color: Color.black);
        UIFactory.Stretch(questionText.gameObject);

        // Sequence container (for pattern questions)
        var seqContainer = new GameObject("SequenceContainer",
            typeof(HorizontalLayoutGroup));
        seqContainer.transform.SetParent(qCard.transform, false);
        UIFactory.SetRect(seqContainer, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.78f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        seqContainer.SetActive(false);

        // ── Answer buttons ──────────────────────────────────────────────
        var ansContainer = new GameObject("AnswerButtonsContainer",
            typeof(VerticalLayoutGroup));
        ansContainer.transform.SetParent(canvas.transform, false);
        UIFactory.SetRect(ansContainer, new Vector2(0.03f, 0.28f), new Vector2(0.97f, 0.58f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var vlg = ansContainer.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 12;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        var answerButtons = UIFactory.CreateAnswerButtons(
            ansContainer.transform, PrefabFactory.AnswerButtonPrefab, 4);

        // ── Memory grid ─────────────────────────────────────────────────
        var memContainer = new GameObject("MemoryGridContainer", typeof(RectTransform));
        memContainer.transform.SetParent(canvas.transform, false);
        UIFactory.SetRect(memContainer, new Vector2(0.1f, 0.28f), new Vector2(0.9f, 0.58f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        memContainer.SetActive(false);

        var memGrid = new GameObject("MemoryGrid",
            typeof(GridLayoutGroup));
        memGrid.transform.SetParent(memContainer.transform, false);
        var memGridRT = UIFactory.Stretch(memGrid);

        var glg = memGrid.GetComponent<GridLayoutGroup>();
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = 3;
        glg.spacing = new Vector2(8, 8);
        glg.cellSize = new Vector2(80, 80);

        var memGridUI = memGrid.AddComponent<MemoryGridUI>();
        UIFactory.Wire(memGridUI, "gridLayout", glg);
        UIFactory.Wire(memGridUI, "cellPrefab", PrefabFactory.MemoryCellPrefab);
        UIFactory.Wire(memGridUI, "gridRect", memGridRT);

        // ── Feedback overlay ────────────────────────────────────────────
        var (fbGo, fbComp, _) = UIFactory.CreateFeedbackOverlay(canvas.transform);

        // ── Controllers ─────────────────────────────────────────────────
        var quizCtrlGo = new GameObject("QuizController");
        quizCtrlGo.AddComponent<QuizController>();

        var uiCtrlGo = new GameObject("QuizUIController");
        var quizUI = uiCtrlGo.AddComponent<QuizUIController>();

        UIFactory.Wire(quizUI, "timeLeftText", timeLeft);
        UIFactory.Wire(quizUI, "questionCounterText", counter);
        UIFactory.Wire(quizUI, "scoreText", scoreText);
        UIFactory.Wire(quizUI, "progressBar", progressBar.GetComponent<Image>());
        UIFactory.Wire(quizUI, "labelText", labelText);
        UIFactory.Wire(quizUI, "questionText", questionText);
        UIFactory.Wire(quizUI, "sequenceContainer", seqContainer);
        UIFactory.Wire(quizUI, "questionTextContainer", qtContainer);
        UIFactory.WireList(quizUI, "answerButtons",
            new Object[] { answerButtons[0], answerButtons[1],
                           answerButtons[2], answerButtons[3] });
        UIFactory.Wire(quizUI, "memoryGridContainer", memContainer);
        UIFactory.Wire(quizUI, "memoryGridUI", memGridUI);
        UIFactory.Wire(quizUI, "feedbackOverlay", fbComp);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/QuizGameScene.unity");
        Debug.Log("[Setup] QuizGameScene built");
    }
}
