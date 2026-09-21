using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Puzzles
{
    /// <summary>
    /// Task 9 — Collect Clue 3 (Project ECHO).
    /// The main computer / audio recorder inside the Secret Room.
    /// If the server hasn't been restored (should always be true by this point
    /// in the linear flow, but the check is kept as a safety net per the
    /// design doc's explicit "server needs to be started" message), it refuses
    /// to play. Otherwise it plays Dr. Arjun's recording and logs Clue 3,
    /// which unlocks Task 10 (Final Decision).
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class SecretComputer : MonoBehaviour
    {
        [Header("Setup")]
        public GameObject serverNeededPromptUI; // "The server needs to be started."
        public AudioSource arjunRecordingAudio;
        public GameObject echoDocumentsRevealUI; // visual/story reveal (documents, text, etc.)

        private XRSimpleInteractable interactable;
        private bool played = false;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            interactable.selectEntered.AddListener(OnInteract);
        }

        private void OnInteract(SelectEnterEventArgs args)
        {
            var gm = GameManager.Instance;

            if (!gm.ServerRestored)
            {
                if (serverNeededPromptUI != null) serverNeededPromptUI.SetActive(true);
                return;
            }

            if (played) return;
            played = true;

            arjunRecordingAudio?.Play();
            if (echoDocumentsRevealUI != null) echoDocumentsRevealUI.SetActive(true);

            ClueJournal.Instance?.CollectClue3();
            gm.AdvanceTo(GameManager.Task.Task10_FinalDecision);
        }
    }
}
