using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// One-shot project setup — creates scenes, prefabs, wires references,
/// configures player settings, and sets up build settings.
///
/// Usage (menu):    Brain Academy > Setup Project
/// Usage (CLI):     Unity.exe -projectPath ... -executeMethod ProjectSetup.Run -batchmode -quit
///
/// Prerequisites:
///   1. Unity project must already exist (created via Unity Hub or -createProject)
///   2. Packages/manifest.json must include "com.unity.nuget.newtonsoft-json"
///   3. TextMeshPro essentials should be imported (Window > TMP > Import Essential Resources)
/// </summary>
public static class ProjectSetup
{
    [MenuItem("Brain Academy/Setup Project")]
    public static void Run()
    {
        Debug.Log("=== Brain Academy Project Setup ===");

        EnsureDirectories();
        ImportTMPEssentials();
        CopyQuestionBank();
        ConfigurePlayerSettings();

        // Prefabs must be created before scenes (scenes instantiate prefabs)
        PrefabFactory.CreateAll();

        // Build all scenes
        SceneBuilderMenu.BuildMenuScene();
        SceneBuilderMenu.BuildDifficultyScene();
        SceneBuilderSnake.Build();
        SceneBuilderResults.BuildSnakeResultsScene();

        ConfigureBuildSettings();

        // Re-open MenuScene as the default scene
        EditorSceneManager.OpenScene("Assets/Scenes/MenuScene.unity");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("=== Setup Complete ===");
    }

    // ── Directories ─────────────────────────────────────────────────────

    private static void EnsureDirectories()
    {
        string[] dirs = { "Scenes", "Prefabs", "Resources" };
        foreach (var dir in dirs)
        {
            if (!AssetDatabase.IsValidFolder($"Assets/{dir}"))
                AssetDatabase.CreateFolder("Assets", dir);
        }
    }

    // ── TMP Essential Resources ────────────────────────────────────────

    private static void ImportTMPEssentials()
    {
        // Check if already imported (font asset exists in project)
        var existing = AssetDatabase.FindAssets("t:TMP_FontAsset");
        if (existing.Length > 0)
        {
            Debug.Log("[Setup] TMP fonts already present, skipping import");
            return;
        }

        // Find the .unitypackage inside the ugui package cache
        string packageCache = Path.Combine(Application.dataPath, "..", "Library", "PackageCache");
        string essentialsPath = null;

        if (Directory.Exists(packageCache))
        {
            foreach (var dir in Directory.GetDirectories(packageCache, "com.unity.ugui*"))
            {
                string candidate = Path.Combine(dir, "Package Resources",
                    "TMP Essential Resources.unitypackage");
                if (File.Exists(candidate))
                {
                    essentialsPath = candidate;
                    break;
                }
            }
        }

        if (essentialsPath != null)
        {
            Debug.Log("[Setup] Importing TMP Essential Resources...");
            AssetDatabase.ImportPackage(essentialsPath, false);
            AssetDatabase.Refresh();
            Debug.Log("[Setup] TMP Essential Resources imported");
        }
        else
        {
            Debug.LogWarning("[Setup] TMP Essential Resources.unitypackage not found");
        }
    }

    // ── Question bank ───────────────────────────────────────────────────

    private static void CopyQuestionBank()
    {
        // Try to find question_bank.json from the repo root
        string projectRoot = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", "..", ".."));
        string source = Path.Combine(projectRoot, "docs", "question_bank.json");
        string dest = Path.Combine(Application.dataPath, "Resources", "question_bank.json");

        if (File.Exists(source))
        {
            File.Copy(source, dest, overwrite: true);
            Debug.Log($"[Setup] Copied question_bank.json to Assets/Resources/");
        }
        else
        {
            // Fallback: check app assets (Android project path)
            string altSource = Path.Combine(projectRoot, "app", "src", "main",
                "assets", "question_bank.json");
            if (File.Exists(altSource))
            {
                File.Copy(altSource, dest, overwrite: true);
                Debug.Log($"[Setup] Copied question_bank.json from Android assets");
            }
            else
            {
                Debug.LogWarning(
                    "[Setup] question_bank.json not found. " +
                    "Copy manually to Assets/Resources/question_bank.json");
            }
        }

        AssetDatabase.Refresh();
    }

    // ── Player settings ─────────────────────────────────────────────────

    private static void ConfigurePlayerSettings()
    {
        PlayerSettings.companyName = "BrainCademy";
        PlayerSettings.productName = "Brain Academy Junior";

#if UNITY_ANDROID
        PlayerSettings.SetApplicationIdentifier(
            UnityEditor.Build.NamedBuildTarget.Android,
            "com.braincademy.junior");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
#endif

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

        Debug.Log("[Setup] Player settings configured");
    }

    // ── Build settings ──────────────────────────────────────────────────

    private static void ConfigureBuildSettings()
    {
        var scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MenuScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/DifficultySelectScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/SnakeSpellScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/SnakeSpellResultsScene.unity", true),
        };

        EditorBuildSettings.scenes = scenes;
        Debug.Log("[Setup] Build settings configured (4 scenes)");
    }
}
