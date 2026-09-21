using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Puzzles
{
    /// <summary>
    /// Task 6 — Collect Clue 1 (Missing Footage).
    /// Requires GameManager.ServerRestored == true before it will do anything
    /// (mirrors the "server needs to be started" gating used on the secret
    /// computer too). Interacting with the CCTV wall after the server is up
    /// reveals a 1.5-hour missing window and logs Clue 1.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class CCTVFootagePuzzle : MonoBehaviour
    {
        [Header("UI / Feedback")]
        public GameObject footageStaticScreen;     // shown before server restored
        public GameObject missingWindowDisplay;     // shown after interacting, post-restore
        public string missingWindowLabel = "13:38 — 14:52 MISSING";

        private XRSimpleInteractable interactable;
        private bool clueLogged = false;

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
                // Server not up yet — footage stays static/inaccessible.
                if (footageStaticScreen != null) footageStaticScreen.SetActive(true);
                return;
            }

            if (clueLogged) return;
            clueLogged = true;

            if (missingWindowDisplay != null) missingWindowDisplay.SetActive(true);

            ClueJournal.Instance?.CollectClue1($"CCTV footage missing: {missingWindowLabel} (1.5 hours).");
            gm.AdvanceTo(GameManager.Task.Task7_FindWrench);
        }
    }
}
