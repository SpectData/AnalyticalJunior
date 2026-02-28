using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds MenuScene and DifficultySelectScene.
/// </summary>
public static class SceneBuilderMenu
{
    // Colors from AppColors
    private static readonly Color Purple60 = new Color(0.40f, 0.49f, 0.92f);     // #667EEA
    private static readonly Color Purple80 = new Color(0.46f, 0.29f, 0.64f);     // #764BA2
    private static readonly Color SnakeGreen = new Color(0.18f, 0.80f, 0.44f);   // #2ECC71
    private static readonly Color EasyGreen = new Color(0.15f, 0.68f, 0.38f);    // #27AE60
    private static readonly Color MediumYellow = new Color(0.95f, 0.61f, 0.07f); // #F39C12
    private static readonly Color HardRed = new Color(0.91f, 0.30f, 0.24f);      // #E74C3C
    private static readonly Color SuperHardPurple = new Color(0.56f, 0.27f, 0.68f); // #8E44AD
    private static readonly Color Background = new Color(0.94f, 0.96f, 0.97f);   // #F0F4F8

    // ── MenuScene ───────────────────────────────────────────────────────

    public static void BuildMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Canvas
        var canvas = UIFactory.CreateCanvas("Canvas");
        UIFactory.CreateBackground(canvas.transform, Background);

        // Header
        var header = UIFactory.CreatePanel(canvas.transform, "Header", Purple60);
        UIFactory.SetRect(header, new Vector2(0, 0.85f), Vector2.one,
            new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero);

        UIFactory.CreateTMP(header.transform, "Title", "Brain Academy Junior",
            fontSize: 48, color: Color.white);

        // Stats card
        var statsCard = UIFactory.CreatePanel(canvas.transform, "StatsCard", Color.white);
        UIFactory.SetRect(statsCard, new Vector2(0.05f, 0.72f), new Vector2(0.95f, 0.83f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var totalScore = UIFactory.CreateTMP(statsCard.transform, "TotalScoreText", "0",
            fontSize: 36, color: Purple60);
        UIFactory.SetRect(totalScore.gameObject, new Vector2(0, 0.5f), new Vector2(0.33f, 1f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var gamesPlayed = UIFactory.CreateTMP(statsCard.transform, "GamesPlayedText", "0",
            fontSize: 36, color: Purple60);
        UIFactory.SetRect(gamesPlayed.gameObject, new Vector2(0.33f, 0.5f), new Vector2(0.66f, 1f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var bestStreak = UIFactory.CreateTMP(statsCard.transform, "BestStreakText", "0",
            fontSize: 36, color: Purple60);
        UIFactory.SetRect(bestStreak.gameObject, new Vector2(0.66f, 0.5f), new Vector2(1f, 1f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // Labels
        UIFactory.CreateTMP(statsCard.transform, "ScoreLabel", "Total Score",
            fontSize: 20, color: Color.gray);
        UIFactory.CreateTMP(statsCard.transform, "PlayedLabel", "Games Played",
            fontSize: 20, color: Color.gray);
        UIFactory.CreateTMP(statsCard.transform, "StreakLabel", "Best Streak",
            fontSize: 20, color: Color.gray);

        // Game button
        var snakeBtn = UIFactory.CreateButton(canvas.transform, "SnakeSpellButton",
            "Snake Spellcaster", SnakeGreen, 32);
        UIFactory.SetRect(snakeBtn.gameObject, new Vector2(0.05f, 0.40f), new Vector2(0.95f, 0.56f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // Controller GO
        var ctrlGo = new GameObject("MenuController");
        var menuUI = ctrlGo.AddComponent<MenuUIController>();

        UIFactory.Wire(menuUI, "totalScoreText", totalScore);
        UIFactory.Wire(menuUI, "gamesPlayedText", gamesPlayed);
        UIFactory.Wire(menuUI, "bestStreakText", bestStreak);
        UIFactory.Wire(menuUI, "snakeSpellButton", snakeBtn);

        // Persistent managers (DontDestroyOnLoad singletons)
        var gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();

        var stm = new GameObject("SceneTransitionManager");
        stm.AddComponent<SceneTransitionManager>();

        var am = new GameObject("AudioManager");
        am.AddComponent<AudioManager>();

        // Save
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MenuScene.unity");
        Debug.Log("[Setup] MenuScene built");
    }

    // ── DifficultySelectScene ───────────────────────────────────────────

    public static void BuildDifficultyScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var canvas = UIFactory.CreateCanvas("Canvas");
        UIFactory.CreateBackground(canvas.transform, Background);

        // Back button
        var backBtn = UIFactory.CreateButton(canvas.transform, "BackButton",
            "< Back", new Color(0.5f, 0.5f, 0.5f), 24);
        UIFactory.SetRect(backBtn.gameObject, new Vector2(0.02f, 0.92f), new Vector2(0.25f, 0.97f),
            new Vector2(0, 1), Vector2.zero, Vector2.zero);

        // Title
        var title = UIFactory.CreateTMP(canvas.transform, "TitleText",
            "Select Difficulty", fontSize: 40);
        UIFactory.SetRect(title.gameObject, new Vector2(0, 0.82f), new Vector2(1, 0.90f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // 2x2 grid of difficulty buttons
        var easyBtn = UIFactory.CreateButton(canvas.transform, "EasyButton",
            "Easy", EasyGreen, 32);
        UIFactory.SetRect(easyBtn.gameObject, new Vector2(0.05f, 0.62f), new Vector2(0.48f, 0.78f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var mediumBtn = UIFactory.CreateButton(canvas.transform, "MediumButton",
            "Medium", MediumYellow, 32);
        UIFactory.SetRect(mediumBtn.gameObject, new Vector2(0.52f, 0.62f), new Vector2(0.95f, 0.78f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var hardBtn = UIFactory.CreateButton(canvas.transform, "HardButton",
            "Hard", HardRed, 32);
        UIFactory.SetRect(hardBtn.gameObject, new Vector2(0.05f, 0.44f), new Vector2(0.48f, 0.60f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var superHardBtn = UIFactory.CreateButton(canvas.transform, "SuperHardButton",
            "Super Hard", SuperHardPurple, 32);
        UIFactory.SetRect(superHardBtn.gameObject, new Vector2(0.52f, 0.44f), new Vector2(0.95f, 0.60f),
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        // Controller
        var ctrlGo = new GameObject("DifficultyController");
        var diffUI = ctrlGo.AddComponent<DifficultyUIController>();

        UIFactory.Wire(diffUI, "backButton", backBtn);
        UIFactory.Wire(diffUI, "titleText", title);
        UIFactory.Wire(diffUI, "easyButton", easyBtn);
        UIFactory.Wire(diffUI, "mediumButton", mediumBtn);
        UIFactory.Wire(diffUI, "hardButton", hardBtn);
        UIFactory.Wire(diffUI, "superHardButton", superHardBtn);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/DifficultySelectScene.unity");
        Debug.Log("[Setup] DifficultySelectScene built");
    }
}
