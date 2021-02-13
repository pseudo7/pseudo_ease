using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

public class GameVersionEditor : IPreprocessBuildWithReport
{
    private const string TargetFile = "GameVersion.cs";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report) => WriteVersionClass();

    [DidReloadScripts]
    public static void WriteVersionClass()
    {
        var filePath = Path.Combine(Application.dataPath, TargetFile);

        if (!File.Exists(filePath))
            File.Create(filePath);

        var newInformation = CreateClass(PlayerSettings.iOS.buildNumber, PlayerSettings.Android.bundleVersionCode);

        var currentText = File.ReadAllText(filePath);

        if (currentText.Equals(newInformation)) return;
        Debug.Log("Updated GameVersion.cs");

        File.WriteAllText(filePath, newInformation);
        AssetDatabase.Refresh();
    }

    private static string CreateClass(string iOSBuildCode, int androidBuildNumber) =>
        $"public static class GameVersion\n{{\n    private const string iOSBuildNumber = \"{iOSBuildCode}\";\n    private const string AndroidBuildNumber = \"{androidBuildNumber}\";\n\n    public static string BuildNumber\n    {{\n        get\n        {{\n#if UNITY_ANDROID\n            return AndroidBuildNumber;\n#elif UNITY_IOS || UNITY_IPHONE\n            return iOSBuildNumber;\n#endif\n            return (0).ToString();\n        }}\n    }}\n}}";
}