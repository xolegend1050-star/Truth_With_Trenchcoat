using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI subtitleText;

    [Header("Player Movement Script")]
    public MonoBehaviour fpsControllerScript;

    [System.Serializable]
    public struct DialogueLine
    {
        public string speaker;
        [TextArea(2, 5)] public string text;
    }

    [Header("Cutscene Sequence")]
    public DialogueLine[] sequence;

    [Header("Scene Transition")]
    public bool loadFacilityOnEnd = true;

    private int currentIndex = 0;

    private void Start()
    {
        if (fpsControllerScript != null) fpsControllerScript.enabled = false;

        if (sequence.Length > 0)
        {
            DisplayLine(0);
        }
        else
        {
            EndCutscene();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    private void DisplayLine(int index)
    {
        speakerText.text = sequence[index].speaker;
        subtitleText.text = sequence[index].text;
    }

    private void NextLine()
    {
        currentIndex++;

        if (currentIndex < sequence.Length)
        {
            DisplayLine(currentIndex);
        }
        else
        {
            EndCutscene();
        }
    }

    public void EndCutscene()
    {
        gameObject.SetActive(false);

        if (fpsControllerScript != null)
        {
            fpsControllerScript.enabled = true;
        }

        Debug.Log("Cutscene ended!");

        // Load the facility scene after cutscene
        if (loadFacilityOnEnd)
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadFacilityScene();
            }
            else
            {
                Debug.LogWarning("SceneLoader not found! Add SceneLoader to the scene.");
            }
        }
    }
}
