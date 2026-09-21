using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FinalDecisionPuzzle : Interactable
{
    public GameObject decisionUI;
    public Button reportButton;
    public Button protectButton;
    public Button exposeButton;
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines = {
        "You've uncovered the truth about Project Echo...",
        "Dr. Arjun was silenced for discovering something dangerous.",
        "What will you do with this evidence?",
        "Your decision will determine the fate of many."
    };

    private int currentLine = 0;
    private bool solved = false;
    private bool dialogueComplete = false;

    void Start()
    {
        interactPrompt = "Press E to make final decision";
        objectName = "Final Evidence";
        if (decisionUI != null) decisionUI.SetActive(false);

        if (reportButton != null)
            reportButton.onClick.AddListener(() => MakeDecision("Report"));
        if (protectButton != null)
            protectButton.onClick.AddListener(() => MakeDecision("Protect"));
        if (exposeButton != null)
            exposeButton.onClick.AddListener(() => MakeDecision("Expose"));
    }

    public override void OnInteract()
    {
        if (solved) return;
        decisionUI.SetActive(true);
        currentLine = 0;
        ShowDialogue();
    }

    void ShowDialogue()
    {
        if (currentLine < dialogueLines.Length)
        {
            if (dialogueText != null)
                dialogueText.text = dialogueLines[currentLine];
            currentLine++;

            if (currentLine >= dialogueLines.Length)
            {
                dialogueComplete = true;
                EnableButtons();
            }
            else
            {
                DisableButtons();
            }
        }
    }

    void Update()
    {
        if (decisionUI.activeSelf && !solved && dialogueComplete == false)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                ShowDialogue();
            }
        }
    }

    void EnableButtons()
    {
        if (reportButton != null) reportButton.interactable = true;
        if (protectButton != null) protectButton.interactable = true;
        if (exposeButton != null) exposeButton.interactable = true;
    }

    void DisableButtons()
    {
        if (reportButton != null) reportButton.interactable = false;
        if (protectButton != null) protectButton.interactable = false;
        if (exposeButton != null) exposeButton.interactable = false;
    }

    public void MakeDecision(string decision)
    {
        if (solved) return;
        solved = true;

        Debug.Log($"Final decision: {decision}");

        switch (decision)
        {
            case "Report":
                Debug.Log("ENDING: Report - Case Closed. Truth delivered to authorities.");
                GameManager.Instance?.SetEnding("Report");
                break;
            case "Protect":
                Debug.Log("ENDING: Protect - Truth Protected. Evidence hidden to protect Arjun.");
                GameManager.Instance?.SetEnding("Protect");
                break;
            case "Expose":
                Debug.Log("ENDING: Expose - Truth Revealed. Organization exposed to public.");
                GameManager.Instance?.SetEnding("Expose");
                break;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved("FinalDecision");

        StartCoroutine(EndingSequence());
    }

    System.Collections.IEnumerator EndingSequence()
    {
        yield return new WaitForSeconds(2f);
        if (dialogueText != null)
            dialogueText.text = "Thank you for playing Truth with Trenchcoat.";
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
