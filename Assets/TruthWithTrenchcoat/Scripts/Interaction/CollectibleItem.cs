using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Interaction
{
    /// <summary>
    /// Attach alongside an XRGrabInteractable on any pickup prop (lockpick,
    /// masterkey, wrench, keychain pieces). On first grab, registers the
    /// item with PlayerInventory and fires the relevant GameManager/ClueJournal
    /// hooks via itemId matching, then optionally disables further physics
    /// grabbing (set keepAsHeldObject = true to let the player keep carrying it,
    /// e.g. the wrench, which must be actively held/equipped to use on the drawer).
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class CollectibleItem : MonoBehaviour
    {
        [Tooltip("Unique id used by GameManager / PlayerInventory / DrawerController etc. " +
                 "Expected values used elsewhere in this project: \"Lockpick\", \"Masterkey\", " +
                 "\"Wrench\", \"KeychainPieceA\", \"KeychainPieceB\".")]
        public string itemId;

        [Tooltip("If true, the item stays grabbable/held after collection (tools like the " +
                 "wrench that must be actively equipped). If false, it is collected once and " +
                 "can be set inactive/pocketed.")]
        public bool keepAsHeldObject = true;

        private XRGrabInteractable grabInteractable;
        private bool collected = false;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }

        private void OnDestroy()
        {
            if (grabInteractable != null)
                grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            if (collected) return;
            collected = true;

            PlayerInventory.Instance?.AddItem(itemId);
            HandleGameStateHooks();

            if (!keepAsHeldObject)
            {
                // For one-off pickups where you don't need to keep carrying the physical
                // prop (e.g. after reading a note). Not used by default items above.
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Wires known item ids to their game-state effects. Extend this switch
        /// if you add more collectible items later.
        /// </summary>
        private void HandleGameStateHooks()
        {
            var gm = GameManager.Instance;
            switch (itemId)
            {
                case "Lockpick":
                    gm.HasLockpick = true;
                    gm.AdvanceTo(GameManager.Task.Task2_RestoreLights);
                    break;

                case "Masterkey":
                    gm.HasMasterkey = true;
                    gm.AdvanceTo(GameManager.Task.Task4_UnlockSecurityRoom);
                    break;

                case "Wrench":
                    gm.HasWrench = true;
                    gm.AdvanceTo(GameManager.Task.Task7_FindWrench);
                    break;

                case "KeychainPieceA":
                    ClueJournal.Instance?.CollectKeychainPiece("A");
                    break;

                case "KeychainPieceB":
                    ClueJournal.Instance?.CollectKeychainPiece("B");
                    CheckKeychainComplete();
                    break;
            }
        }

        private void CheckKeychainComplete()
        {
            if (ClueJournal.Instance != null && ClueJournal.Instance.Clue2_Keychain)
            {
                // Both halves joined — advance past Task 8 once the wall is also open.
                // (SecretWallSwitch.cs handles the actual Task 8 advance since opening
                // the wall is the real gating action; this just ensures the clue is logged.)
            }
        }
    }
}
