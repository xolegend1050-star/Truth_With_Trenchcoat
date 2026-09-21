using UnityEngine;
using UnityEngine.UI;

public class PowerPanelPuzzle : Interactable
{
    public GameObject circuitUI;
    public Button[] wireSlots;
    public Button[] availableWires;
    public Image[] connectionIndicators;
    public string correctSequence = "RBGY";
    public GameObject powerLight;
    public Material powerOnMaterial;

    private string currentSequence = "";
    private bool solved = false;

    void Start()
    {
        interactPrompt = "Press E to examine power panel";
        objectName = "Power Panel";
        if (circuitUI != null) circuitUI.SetActive(false);
        if (powerLight != null) powerLight.SetActive(false);

        for (int i = 0; i < availableWires.Length; i++)
        {
            int idx = i;
            availableWires[i].onClick.AddListener(() => ConnectWire(idx));
        }
    }

    public override void OnInteract()
    {
        if (solved) return;
        circuitUI.SetActive(true);
        currentSequence = "";
        Debug.Log("Connect wires in correct sequence: Red, Blue, Green, Yellow");
        UpdateIndicators();
    }

    public void ConnectWire(int wireIndex)
    {
        if (solved) return;

        char[] wireColors = { 'R', 'B', 'G', 'Y' };
        if (wireIndex < wireColors.Length)
        {
            currentSequence += wireColors[wireIndex];
            Debug.Log($"Connected: {wireColors[wireIndex]}");
            UpdateIndicators();

            if (currentSequence.Length == correctSequence.Length)
            {
                CheckSequence();
            }
        }
    }

    void UpdateIndicators()
    {
        for (int i = 0; i < connectionIndicators.Length; i++)
        {
            if (i < currentSequence.Length)
            {
                connectionIndicators[i].color = GetWireColor(currentSequence[i]);
            }
            else
            {
                connectionIndicators[i].color = Color.gray;
            }
        }
    }

    Color GetWireColor(char c)
    {
        switch (c)
        {
            case 'R': return Color.red;
            case 'B': return Color.blue;
            case 'G': return Color.green;
            case 'Y': return Color.yellow;
            default: return Color.gray;
        }
    }

    void CheckSequence()
    {
        if (currentSequence == correctSequence)
        {
            Solved();
        }
        else
        {
            Debug.Log("Wrong sequence! Power surge - resetting.");
            currentSequence = "";
            UpdateIndicators();
        }
    }

    void Solved()
    {
        solved = true;
        circuitUI.SetActive(false);
        if (powerLight != null)
        {
            powerLight.SetActive(true);
            powerLight.GetComponent<Renderer>().material = powerOnMaterial;
        }
        Debug.Log("Power restored! Emergency lighting activated.");
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved("PowerPanel");
    }
}
