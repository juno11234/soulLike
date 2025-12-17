using UnityEngine;

using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Linq;
using System.IO;

public static class BuildAuto
{
    /// <summary>
    /// 젠킨스빌드를 위한 스크립트
    /// </summary>
    public static void Build()
    {
        // 씬의 이름을 가져옴 .활성화된 .경로정보만 .배열로변환
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("씬이 없어.");
            EditorApplication.Exit(1);
            return;
        }

        // 저장 경로 및 실행파일 이름
        string buildPath = "Builds/StandaloneWindows64";
        string exeName = "SoulLike.exe";
        string locationPathName = Path.Combine(buildPath, exeName);

        // 저장할 디렉토리 확인 및 생성
        Directory.CreateDirectory(buildPath);

        #region  디버깅 로그들

        Debug.Log($"현재 작업중인 주소: {Directory.GetCurrentDirectory()}");
        Debug.Log($"빌드 환경: {BuildTarget.StandaloneWindows64}");
        Debug.Log($"빌드된 씬: {string.Join(", ", scenes)}");
        Debug.Log($"빌드저장 주소: {locationPathName}");

        #endregion
        
        //  빌드옵션설정
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = locationPathName,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None // No special options
        };

        // 빌드 시작
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        // 결과 보고 및 종료
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"- 결과: {summary.outputPath}");
            Debug.Log($"- 용량: {summary.totalSize} bytes");
            Debug.Log($"- 시간: {summary.totalTime}");
            EditorApplication.Exit(0); // Success
        }
        else
        {
            Debug.LogError($"- 문제원인: {summary.result}");
            Debug.LogError($"- 에러: {summary.totalErrors}");
            EditorApplication.Exit(1); // Failure
        }
    }
}