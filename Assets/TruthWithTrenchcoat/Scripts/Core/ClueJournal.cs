using System;
using UnityEngine;

namespace TruthWithTrenchcoat.Core
{
    /// <summary>
    /// Tracks the 3 story clues. Puzzle scripts call the Collect methods;
    /// UI (journal panel) subscribes to OnClueCollected to show a popup/log entry.
    /// </summary>
    public class ClueJournal : MonoBehaviour
    {
        public static ClueJournal Instance { get; private set; }

        [Header("Clue State (read-only at runtime)")]
        public bool Clue1_MissingFootage = false;
        public bool Clue2_Keychain = false;
        public bool Clue3_ProjectEcho = false;

        // Clue 2 requires BOTH keychain halves — tracked separately, combined below.
        private bool keychainPieceA = false;
        private bool keychainPieceB = false;

        public event Action<string, string> OnClueCollected; // (clueId, description)

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void CollectClue1(string description = "1.5 hours of CCTV footage is missing.")
        {
            if (Clue1_MissingFootage) return;
            Clue1_MissingFootage = true;
            OnClueCollected?.Invoke("Clue1", description);
        }

        public void CollectKeychainPiece(string pieceId)
        {
            if (pieceId == "A") keychainPieceA = true;
            else if (pieceId == "B") keychainPieceB = true;

            if (keychainPieceA && keychainPieceB && !Clue2_Keychain)
            {
                Clue2_Keychain = true;
                OnClueCollected?.Invoke("Clue2", "The two halves of the broken keychain fit together perfectly.");
            }
        }

        public void CollectClue3(string description = "Dr. Arjun's recording reveals the truth about Project ECHO.")
        {
            if (Clue3_ProjectEcho) return;
            Clue3_ProjectEcho = true;
            OnClueCollected?.Invoke("Clue3", description);
        }

        public bool AllCluesCollected => Clue1_MissingFootage && Clue2_Keychain && Clue3_ProjectEcho;
    }
}
