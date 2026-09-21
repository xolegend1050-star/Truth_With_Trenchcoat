#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class AutoSetupMainMenu
{
    [MenuItem("Tools/Setup MainMenu Scene")]
    public static void SetupMainMenu()
    {
        // Create Canvas
        GameObject canvas = new GameObject("Canvas");
        Canvas c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Create EventSystem
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Create TitleText
        GameObject titleObj = CreateText("TitleText", canvas.transform, "TRUTH WITH TRENCHCOAT", 60, Color.white, 100);

        // Create SubtitleText
        GameObject subObj = CreateText("SubtitleText", canvas.transform, "A Detective Murder Mystery", 30, Color.gray, 40);

        // Create PlayButton
        GameObject playBtn = CreateButton("PlayButton", canvas.transform, "Play", -40);

        // Create QuitButton
        GameObject quitBtn = CreateButton("QuitButton", canvas.transform, "Quit", -100);

        // Create GameManager
        GameObject gm = new GameObject("GameManager");
        var mainMenu = gm.AddComponent<MainMenu>();
        mainMenu.titleText = titleObj.GetComponent<TMPro.TextMeshProUGUI>();
        mainMenu.subtitleText = subObj.GetComponent<TMPro.TextMeshProUGUI>();
        mainMenu.menuUI = canvas;

        // Wire PlayButton
        var playOnClick = playBtn.GetComponent<UnityEngine.UI.Button>().onClick;
        playOnClick.AddListener(mainMenu.PlayGame);

        // Wire QuitButton
        var quitOnClick = quitBtn.GetComponent<UnityEngine.UI.Button>().onClick;
        quitOnClick.AddListener(mainMenu.QuitGame);

        // Save scene
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        Debug.Log("MainMenu scene setup complete!");
        EditorUtility.DisplayDialog("Done", "MainMenu scene created successfully!\n\nNext: Add scenes to Build Settings.", "OK");
    }

    static GameObject CreateText(string name, Transform parent, string content, float fontSize, Color color, float yPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        TMPro.TextMeshProUGUI tmp = obj.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, yPos);
        rt.sizeDelta = new Vector2(800, 80);
        return obj;
    }

    static GameObject CreateButton(string name, Transform parent, string label, float yPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        UnityEngine.UI.Image img = obj.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        obj.AddComponent<UnityEngine.UI.Button>();
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, yPos);
        rt.sizeDelta = new Vector2(200, 50);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        TMPro.TextMeshProUGUI tmp = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 30;
        tmp.color = Color.white;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        return obj;
    }
}
#endif
