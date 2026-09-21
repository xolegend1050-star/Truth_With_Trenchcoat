#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoSetupBuildSettings
{
    [MenuItem("Tools/Setup Build Settings")]
    public static void SetupBuildSettings()
    {
        string[] scenePaths = new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/game.unity",
            "Assets/Scenes/SampleScene.unity"
        };

        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[scenePaths.Length];

        for (int i = 0; i < scenePaths.Length; i++)
        {
            string fullPath = Path.Combine(Application.dataPath, "..", scenePaths[i]);
            bool exists = File.Exists(fullPath);
            scenes[i] = new EditorBuildSettingsScene(scenePaths[i], exists);
            Debug.Log($"Scene {i}: {scenePaths[i]} - {(exists ? "Found" : "MISSING")}");
        }

        EditorBuildSettings.scenes = scenes;
        Debug.Log("Build Settings configured!");
        EditorUtility.DisplayDialog("Done", "Build Settings configured!\n\nScenes added:\n1. MainMenu\n2. game (cutscene)\n3. SampleScene (facility)", "OK");
    }
}
#endif
