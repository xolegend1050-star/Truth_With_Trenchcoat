using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public GameObject menuUI;
    public string firstScene = "game";

    void Start()
    {
        if (titleText != null)
            titleText.text = "TRUTH WITH TRENCHCOAT";
        if (subtitleText != null)
            subtitleText.text = "A Detective Murder Mystery";
        if (menuUI != null)
            menuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(firstScene);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
