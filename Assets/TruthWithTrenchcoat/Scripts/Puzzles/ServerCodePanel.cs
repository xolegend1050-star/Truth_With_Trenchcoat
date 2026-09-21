using UnityEngine;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Puzzles
{
    /// <summary>
    /// Task 5 — Restore the Server.
    /// Fixed code: "1423" (from the stopped watch on the Main Lab desk,
    /// reading 14:23 / 2:53 PM). Wire this to any numeric keypad UI —
    /// this script just validates the submitted string and fires the
    /// unlock. If you use a physical 4-digit dial/keypad interactable
    /// instead of a UI canvas, call SubmitDigit() per dial and check
    /// CurrentEntry against CorrectCode, or call TrySubmitCode() directly
    /// once all 4 digits are set.
    /// </summary>
    public class ServerCodePanel : MonoBehaviour
    {
        private const string CorrectCode = "1423";

        [Header("Feedback")]
        public AudioSource correctSfx;
        public AudioSource wrongSfx;
        public GameObject serverOfflineVisual;
        public GameObject serverOnlineVisual;

        private string currentEntry = "";
        private bool solved = false;

        /// <summary>Call this from keypad button interactables, one digit at a time.</summary>
        public void SubmitDigit(int digit)
        {
            if (solved) return;
            if (currentEntry.Length >= 4) currentEntry = "";
            currentEntry += digit.ToString();

            if (currentEntry.Length == 4)
            {
                TryValidate();
            }
        }

        public void TryValidate()
        {
            if (currentEntry == CorrectCode)
            {
                Solve();
            }
            else
            {
                wrongSfx?.Play();
                currentEntry = "";
            }
        }

        private void Solve()
        {
            solved = true;
            correctSfx?.Play();

            if (serverOfflineVisual != null) serverOfflineVisual.SetActive(false);
            if (serverOnlineVisual != null) serverOnlineVisual.SetActive(true);

            var gm = GameManager.Instance;
            gm.ServerRestored = true;
            gm.AdvanceTo(GameManager.Task.Task6_CollectClue1);
        }
    }
}
