using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DrawerCodePuzzle : Interactable
{
    public TextMeshProUGUI codeDisplay;
    public Button[] numberButtons;
    public Button submitButton;
    public Button clearButton;
    public string correctCode = "347";
    public GameObject drawerContents;
    public GameObject clueDocument;

    private string currentCode = "";
    private bool solved = false;

    void Start()
    {
        interactPrompt = "Press E to check drawer";
        objectName = "Locked Drawer";
        if (drawerContents != null) drawerContents.SetActive(false);
        if (clueDocument != null) clueDocument.SetActive(false);

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitCode);
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearCode);

        for (int i = 0; i < numberButtons.Length && i < 10; i++)
        {
            int num = i;
            numberButtons[i].onClick.AddListener(() => AddDigit(num.ToString()));
        }
    }

    public override void OnInteract()
    {
        if (solved) return;
        Debug.Log("Enter 3-digit code...");
        UpdateDisplay();
    }

    public void AddDigit(string digit)
    {
        if (currentCode.Length < 3 && !solved)
        {
            currentCode += digit;
            UpdateDisplay();
        }
    }

    public void ClearCode()
    {
        currentCode = "";
        UpdateDisplay();
    }

    public void SubmitCode()
    {
        if (currentCode == correctCode)
        {
            Solved();
        }
        else
        {
            Debug.Log("Wrong code! Try again.");
            currentCode = "";
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        if (codeDisplay != null)
            codeDisplay.text = currentCode;
    }

    void Solved()
    {
        solved = true;
        Debug.Log("Drawer opened! Found clue document.");
        if (drawerContents != null) drawerContents.SetActive(true);
        if (clueDocument != null) clueDocument.SetActive(true);
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved("Drawer");
    }
}
