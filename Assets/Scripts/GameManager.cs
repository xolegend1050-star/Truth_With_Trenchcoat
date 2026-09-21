using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Clue Tracking")]
    public int totalClues = 8;
    public int cluesCollected = 0;

    [Header("Puzzle Tracking")]
    public TextMeshProUGUI clueCountText;
    public TextMeshProUGUI messageText;
    public GameObject victoryScreen;
    public TMPro.TextMeshProUGUI endingText;

    private HashSet<string> solvedPuzzles = new HashSet<string>();
    private string currentEnding = "";
    private bool gameComplete = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (victoryScreen != null) victoryScreen.SetActive(false);
        UpdateUI();
        ShowMessage("Find clues and solve puzzles to uncover the truth.");
    }

    public void OnClueCollected()
    {
        cluesCollected++;
        UpdateUI();
        ShowMessage($"Clue collected! ({cluesCollected}/{totalClues})");

        if (cluesCollected >= totalClues)
        {
            ShowMessage("All clues collected! Head to the Secret Room.");
        }
    }

    public void OnPuzzleSolved(string puzzleName)
    {
        if (!solvedPuzzles.Contains(puzzleName))
        {
            solvedPuzzles.Add(puzzleName);
            ShowMessage($"Puzzle solved: {puzzleName} ({solvedPuzzles.Count}/8)");
            Debug.Log($"Puzzle solved: {puzzleName}. Total: {solvedPuzzles.Count}/8");

            if (solvedPuzzles.Count >= 8)
            {
                StartCoroutine(GameComplete());
            }
        }
    }

    public void SetEnding(string ending)
    {
        currentEnding = ending;
        Debug.Log($"Game ending set: {ending}");
    }

    IEnumerator GameComplete()
    {
        gameComplete = true;
        ShowMessage("All puzzles solved! The truth is revealed.");

        yield return new WaitForSeconds(3f);

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
            if (endingText != null)
            {
                switch (currentEnding)
                {
                    case "Report":
                        endingText.text = "CASE CLOSED\n\nYou reported the evidence to authorities.\nThe truth was delivered through proper channels.\nDr. Arjun's disappearance was officially investigated.";
                        break;
                    case "Protect":
                        endingText.text = "TRUTH PROTECTED\n\nYou chose to hide the evidence.\nDr. Arjun's work remains safe.\nThe organization's secrets are buried.";
                        break;
                    case "Expose":
                        endingText.text = "TRUTH REVEALED\n\nYou exposed the organization to the public.\nThe world now knows about Project Echo.\nDr. Arjun's sacrifice was not in vain.";
                        break;
                    default:
                        endingText.text = "GAME COMPLETE\n\nThank you for playing Truth with Trenchcoat.";
                        break;
                }
            }
        }
    }

    public bool IsPuzzleSolved(string puzzleName)
    {
        return solvedPuzzles.Contains(puzzleName);
    }

    public int GetSolvedCount()
    {
        return solvedPuzzles.Count;
    }

    void UpdateUI()
    {
        if (clueCountText != null)
            clueCountText.text = $"Clues: {cluesCollected}/{totalClues}";
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            StartCoroutine(FadeMessage());
        }
    }

    IEnumerator FadeMessage()
    {
        messageText.alpha = 1f;
        yield return new WaitForSeconds(3f);
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            messageText.alpha = 1f - (elapsed / 1f);
            yield return null;
        }
        messageText.alpha = 0f;
    }
}
