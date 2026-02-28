using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// CLI-invokable Android build. Usage:
///   Unity.exe -batchmode -quit -projectPath ... -executeMethod BuildAndroid.Build
/// </summary>
public static class BuildAndroid
{
    private const string ApkPath = "Builds/BrainAcademy.apk";

    [MenuItem("Brain Academy/Build/Android APK")]
    public static void Build()
    {
        // Ensure output directory exists
        System.IO.Directory.CreateDirectory("Builds");

        // Gather all scenes in build settings, or fall back to our known scenes
        var scenes = new[]
        {
            "Assets/Scenes/MenuScene.unity",
            "Assets/Scenes/DifficultySelectScene.unity",
            "Assets/Scenes/QuizGameScene.unity",
            "Assets/Scenes/ResultsScene.unity",
            "Assets/Scenes/SnakeSpellScene.unity",
            "Assets/Scenes/SnakeSpellResultsScene.unity",
        };

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = ApkPath,
            target = BuildTarget.Android,
            options = BuildOptions.None,
        };

        Debug.Log("[Build] Starting Android APK build...");
        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[Build] APK built successfully: {ApkPath} ({report.summary.totalSize / (1024 * 1024)} MB)");
        }
        else
        {
            Debug.LogError($"[Build] Build failed: {report.summary.result}");
            // Exit with error code in batch mode
            if (Application.isBatchMode)
                EditorApplication.Exit(1);
        }
    }
}
