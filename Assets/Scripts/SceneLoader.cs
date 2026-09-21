using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("Scene Names (must match Build Settings)")]
    public string introScene = "game";
    public string facilityScene = "FacilityScene";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadFacilityScene()
    {
        Debug.Log("Loading Facility Scene...");
        SceneManager.LoadScene(facilityScene);
    }

    public void LoadIntroScene()
    {
        SceneManager.LoadScene(introScene);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(introScene);
    }

    // Call this from cutscene or any trigger
    public void OnCutsceneEnd()
    {
        LoadFacilityScene();
    }
}
