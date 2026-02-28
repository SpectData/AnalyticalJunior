using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Builds SnakeSpellResultsScene.
/// </summary>
public static class SceneBuilderResults
{
    private static readonly Color Background = new Color(0.94f, 0.96f, 0.97f);
    private static readonly Color SnakeGreen = new Color(0.18f, 0.80f, 0.44f);

    // ── SnakeSpellResultsScene ──────────────────────────────────────────

    public static void BuildSnakeResultsScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var canvas = UIFactory.CreateCanvas("Canvas");
        UIFactory.CreateBackground(canvas.transform, Background);

        // Results card
        var card = UIFactory.CreatePanel(canvas.transform, "ResultsCard", Color.white);
        UIFactory.SetRect(card, new Vector2(0.05f, 0.30f), new Vector2(0.95f, 0.88f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var titleText = UIFactory.CreateTMP(card.transform, "TitleText",
            "Game Over!", fontSize: 40, color: SnakeGreen);
        UIFactory.SetRect(titleText.gameObject, new Vector2(0, 0.88f), new Vector2(1, 1),
            new Vector2(0.5f, 1), Vector2.zero, Vector2.zero);

        var emojiText = UIFactory.CreateTMP(card.transform, "EmojiText",
            "\uD83D\uDC0D\u2728", fontSize: 48);
        UIFactory.SetRect(emojiText.gameObject, new Vector2(0, 0.76f), new Vector2(1, 0.88f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var scoreText = UIFactory.CreateTMP(card.transform, "ScoreText",
            "0 points", fontSize: 36, color: SnakeGreen);
        UIFactory.SetRect(scoreText.gameObject, new Vector2(0, 0.64f), new Vector2(1, 0.76f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // Stat rows
        float y = 0.52f;
        var wavesText = CreateStatRow(card.transform, "WavesText", "Waves", ref y);
        var snakesText = CreateStatRow(card.transform, "SnakesText", "Snakes Defeated", ref y);
        var questionsText = CreateStatRow(card.transform, "QuestionsText", "Questions", ref y);
        var accuracyText = CreateStatRow(card.transform, "AccuracyText", "Accuracy", ref y);

        // Buttons
        var playAgainBtn = UIFactory.CreateButton(canvas.transform, "PlayAgainButton",
            "Play Again", SnakeGreen, 32);
        UIFactory.SetRect(playAgainBtn.gameObject, new Vector2(0.1f, 0.14f), new Vector2(0.9f, 0.24f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var menuBtn = UIFactory.CreateButton(canvas.transform, "MenuButton",
            "Menu", new Color(0.5f, 0.5f, 0.5f), 32);
        UIFactory.SetRect(menuBtn.gameObject, new Vector2(0.1f, 0.02f), new Vector2(0.9f, 0.12f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // Controller
        var ctrlGo = new GameObject("SnakeResultsController");
        var comp = ctrlGo.AddComponent<SnakeResultsUIController>();

        UIFactory.Wire(comp, "titleText", titleText);
        UIFactory.Wire(comp, "emojiText", emojiText);
        UIFactory.Wire(comp, "scoreText", scoreText);
        UIFactory.Wire(comp, "wavesText", wavesText);
        UIFactory.Wire(comp, "snakesText", snakesText);
        UIFactory.Wire(comp, "questionsText", questionsText);
        UIFactory.Wire(comp, "accuracyText", accuracyText);
        UIFactory.Wire(comp, "playAgainButton", playAgainBtn);
        UIFactory.Wire(comp, "menuButton", menuBtn);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/SnakeSpellResultsScene.unity");
        Debug.Log("[Setup] SnakeSpellResultsScene built");
    }

    private static TMPro.TextMeshProUGUI CreateStatRow(Transform parent,
        string name, string label, ref float y)
    {
        var text = UIFactory.CreateTMP(parent, name, "0",
            fontSize: 28, color: Color.black);
        UIFactory.SetRect(text.gameObject, new Vector2(0, y - 0.10f), new Vector2(1, y),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        y -= 0.12f;
        return text;
    }
}
