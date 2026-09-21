using UnityEngine;
using TMPro;

public class ProjectEchoPasswordPuzzle : Interactable
{
    public TextMeshProUGUI passwordInput;
    public string correctPassword = "ECHO";
    public GameObject[] documents;
    public GameObject lockedTerminal;
    public GameObject unlockedTerminal;
    public TextMeshProUGUI[] documentTexts;
    public string[] hiddenLetters = { "E", "C", "H", "O" };

    private string currentPassword = "";
    private int lettersFound = 0;
    private bool solved = false;

    void Start()
    {
        interactPrompt = "Press E to examine documents";
        objectName = "Project Documents";
        if (unlockedTerminal != null) unlockedTerminal.SetActive(false);
        if (passwordInput != null) passwordInput.gameObject.SetActive(false);

        if (documentTexts != null)
        {
            for (int i = 0; i < documentTexts.Length && i < hiddenLetters.Length; i++)
            {
                documentTexts[i].text = $"Document {i + 1}: [Highlighted letter: {hiddenLetters[i]}]";
            }
        }
    }

    public override void OnInteract()
    {
        if (solved) return;
        Debug.Log("Search through documents for hidden password letters.");
        foreach (var doc in documents) doc.SetActive(true);
    }

    public void FoundLetter(int index)
    {
        if (index >= 0 && index < hiddenLetters.Length)
        {
            lettersFound++;
            Debug.Log($"Found letter: {hiddenLetters[index]} ({lettersFound}/{hiddenLetters.Length})");

            if (lettersFound >= hiddenLetters.Length)
            {
                Debug.Log("All letters found! Password: ECHO");
                AskForPassword();
            }
        }
    }

    void AskForPassword()
    {
        if (passwordInput != null)
        {
            passwordInput.gameObject.SetActive(true);
            passwordInput.text = "";
        }
    }

    public void SubmitPassword(string password)
    {
        if (password.ToUpper() == correctPassword)
        {
            Solved();
        }
        else
        {
            Debug.Log("Wrong password! Try again.");
        }
    }

    void Solved()
    {
        solved = true;
        if (unlockedTerminal != null) unlockedTerminal.SetActive(true);
        if (lockedTerminal != null) lockedTerminal.SetActive(false);
        Debug.Log("Project Echo terminal unlocked!");
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved("ProjectEcho");
    }
}
