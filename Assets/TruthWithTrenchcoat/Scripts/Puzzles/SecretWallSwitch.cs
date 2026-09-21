using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Puzzles
{
    /// <summary>
    /// Task 8 — Open the Secret Lab.
    /// The painting in Dr's Office is the switch. It only responds once the
    /// player has Keychain Piece A (the symbol on it is what confirms the
    /// match to the painting, per the design doc). Interacting with the
    /// painting after that triggers the sliding wall to open into the Secret Room.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class SecretWallSwitch : MonoBehaviour
    {
        [Header("Setup")]
        public GameObject lockedPromptUI;   // "This painting doesn't move... yet."
        public Animator slidingWallAnimator; // drives "IsOpen" bool
        public Transform slidingWallTransform;
        public Vector3 openLocalOffset = new Vector3(3f, 0, 0); // slides sideways
        public float openSpeed = 1f;

        private XRSimpleInteractable interactable;
        private bool isOpen = false;
        private Vector3 closedPos;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            interactable.selectEntered.AddListener(OnInteract);
            if (slidingWallTransform != null) closedPos = slidingWallTransform.localPosition;
        }

        private void OnInteract(SelectEnterEventArgs args)
        {
            if (isOpen) return;

            bool hasSymbolMatch = PlayerInventory.Instance != null &&
                                   PlayerInventory.Instance.HasItem("KeychainPieceA");

            if (!hasSymbolMatch)
            {
                if (lockedPromptUI != null) lockedPromptUI.SetActive(true);
                return;
            }

            Open();
        }

        private void Open()
        {
            isOpen = true;
            if (lockedPromptUI != null) lockedPromptUI.SetActive(false);

            if (slidingWallAnimator != null)
            {
                slidingWallAnimator.SetBool("IsOpen", true);
            }
            else if (slidingWallTransform != null)
            {
                StartCoroutine(SlideOpen());
            }

            var gm = GameManager.Instance;
            gm.SecretRoomOpen = true;
            gm.AdvanceTo(GameManager.Task.Task9_CollectClue3);
        }

        private System.Collections.IEnumerator SlideOpen()
        {
            Vector3 target = closedPos + openLocalOffset;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                slidingWallTransform.localPosition = Vector3.Lerp(closedPos, target, t);
                yield return null;
            }
        }
    }
}
