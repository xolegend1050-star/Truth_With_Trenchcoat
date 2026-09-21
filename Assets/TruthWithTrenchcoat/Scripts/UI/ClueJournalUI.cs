using UnityEngine;
using TMPro;
using System.Collections;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.UI
{
    /// <summary>
    /// Shows a short "Clue Collected" popup whenever ClueJournal logs a new
    /// clue. Purely presentational — safe to skip if you'd rather show
    /// clues in a different UI (e.g. a physical notebook prop).
    /// </summary>
    public class ClueJournalUI : MonoBehaviour
    {
        public GameObject popupRoot;
        public TMP_Text popupTitleText;
        public TMP_Text popupBodyText;
        public float displaySeconds = 4f;

        private Coroutine hideRoutine;

        private void OnEnable()
        {
            if (ClueJournal.Instance != null)
                ClueJournal.Instance.OnClueCollected += ShowPopup;
        }

        private void OnDisable()
        {
            if (ClueJournal.Instance != null)
                ClueJournal.Instance.OnClueCollected -= ShowPopup;
        }

        private void ShowPopup(string clueId, string description)
        {
            if (popupRoot == null) return;

            string title = clueId switch
            {
                "Clue1" => "Clue 1 Collected",
                "Clue2" => "Clue 2 Collected",
                "Clue3" => "Clue 3 Collected",
                _ => "Clue Collected"
            };

            if (popupTitleText != null) popupTitleText.text = title;
            if (popupBodyText != null) popupBodyText.text = description;

            popupRoot.SetActive(true);

            if (hideRoutine != null) StopCoroutine(hideRoutine);
            hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(displaySeconds);
            popupRoot.SetActive(false);
        }
    }
}
