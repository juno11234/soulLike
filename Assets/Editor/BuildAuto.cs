using UnityEngine;

using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;
using System.IO;

public static class BuildAuto
{
    /// <summary>
    /// This method is called by Jenkins to build the project.
    /// </summary>
    public static void Build()
    {
        // Get enabled scenes from Build Settings
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("No scenes are enabled in Build Settings. Aborting build.");
            EditorApplication.Exit(1);
            return;
        }

        // Define build path and executable name
        string buildPath = "Builds/StandaloneWindows64";
        string exeName = "SoulLike.exe";
        string locationPathName = Path.Combine(buildPath, exeName);

        // Ensure the target directory exists.
        Directory.CreateDirectory(buildPath);
        
        Debug.Log($"Starting build for target: {BuildTarget.StandaloneWindows64}");
        Debug.Log($"Scenes to build: {string.Join(", ", scenes)}");
        Debug.Log($"Output location: {locationPathName}");

        // Configure build options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = locationPathName,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None // No special options
        };

        // Start the build
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        // Report the result and exit with a status code
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded!");
            Debug.Log($"- Output: {summary.outputPath}");
            Debug.Log($"- Size: {summary.totalSize} bytes");
            Debug.Log($"- Duration: {summary.totalTime}");
            EditorApplication.Exit(0); // Success
        }
        else
        {
            Debug.LogError("Build failed!");
            Debug.LogError($"- Reason: {summary.result}");
            Debug.LogError($"- Errors: {summary.totalErrors}");
            EditorApplication.Exit(1); // Failure
        }
    }
}