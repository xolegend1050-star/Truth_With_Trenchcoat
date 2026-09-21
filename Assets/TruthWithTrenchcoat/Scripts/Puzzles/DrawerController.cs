using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Puzzles
{
    /// <summary>
    /// Task 3 setup / Task 7 payoff — the stuck drawer in Dr's Office.
    /// The drawer opens only after the Wrench is collected. Opening it reveals
    /// Keychain Piece A and completes Task 3, advancing to Task 4.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class DrawerController : MonoBehaviour
    {
        public GameObject stuckPromptUI;
        public Animator drawerAnimator;
        public Transform drawerTransform;
        public Vector3 openLocalOffset = new Vector3(0, 0, 0.3f);
        public GameObject keychainPieceAObject;

        private XRSimpleInteractable interactable;
        private bool isOpen = false;
        private Vector3 closedPos;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            interactable.selectEntered.AddListener(OnInteract);
            if (drawerTransform != null) closedPos = drawerTransform.localPosition;
        }

        private void OnDestroy()
        {
            if (interactable != null)
                interactable.selectEntered.RemoveListener(OnInteract);
        }

        private void OnInteract(SelectEnterEventArgs args)
        {
            if (isOpen) return;

            bool hasWrench = PlayerInventory.Instance != null &&
                             PlayerInventory.Instance.HasItem("Wrench");

            if (!hasWrench)
            {
                if (stuckPromptUI != null) stuckPromptUI.SetActive(true);
                return;
            }

            Open();
        }

        private void Open()
        {
            isOpen = true;
            if (stuckPromptUI != null) stuckPromptUI.SetActive(false);

            if (drawerAnimator != null)
                drawerAnimator.SetBool("IsOpen", true);
            else if (drawerTransform != null)
                drawerTransform.localPosition = closedPos + openLocalOffset;

            if (keychainPieceAObject != null)
                keychainPieceAObject.SetActive(true);

            // Task 3 is now genuinely complete.
            GameManager.Instance?.AdvanceTo(GameManager.Task.Task4_UnlockSecurityRoom);
        }
    }
}
