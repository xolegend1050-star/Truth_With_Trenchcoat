using UnityEngine;
using TMPro;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.FPS
{
    /// <summary>
    /// Non-VR interaction handler. Raycasts from camera center.
    /// E key = interact, Left Click = also interact, F = pick up item.
    /// Shows prompt text onscreen when looking at interactable objects.
    /// Works alongside the existing game scripts by calling their public methods.
    /// </summary>
    public class FPSInteraction : MonoBehaviour
    {
        [Header("Raycast")]
        public float interactRange = 3f;
        public Camera playerCamera;
        public LayerMask interactableLayer = ~0;

        [Header("UI")]
        public TextMeshProUGUI promptText;
        public GameObject crosshair;

        [Header("Pickup")]
        public Transform holdPoint;
        public float holdDistance = 1.5f;

        private GameObject currentTarget;
        private string currentPrompt = "";
        private GameObject heldObject;
        private Rigidbody heldRb;

        void Start()
        {
            if (playerCamera == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerCamera = player.GetComponentInChildren<Camera>();
            }

            if (holdPoint == null)
            {
                holdPoint = new GameObject("HoldPoint").transform;
                holdPoint.SetParent(transform);
                holdPoint.localPosition = new Vector3(0, 0, holdDistance);
            }

            if (crosshair != null)
                crosshair.SetActive(true);
        }

        void Update()
        {
            // Don't interact while in a puzzle UI
            if (GameManager.Instance != null && GameManager.Instance.CurrentTask == GameManager.Task.GameComplete)
                return;

            HandleRaycast();
            HandleInteraction();
            HandleDrop();
        }

        void HandleRaycast()
        {
            if (playerCamera == null) return;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                var obj = hit.collider.gameObject;

                // Check for FPS interactable component
                var fpsInteract = obj.GetComponent<FPSInteractable>();
                if (fpsInteract != null)
                {
                    currentTarget = obj;
                    currentPrompt = fpsInteract.interactPrompt;
                    return;
                }

                // Check for game puzzles (they have public methods we can call)
                var puzzleName = obj.name;
                if (puzzleName.Contains("ColorBox") || puzzleName.Contains("ServerCode") ||
                    puzzleName.Contains("CCTV") || puzzleName.Contains("StuckDrawer") ||
                    puzzleName.Contains("SecretComputer") || puzzleName.Contains("FinalDecision") ||
                    puzzleName.Contains("WallPainting") || puzzleName.Contains("Lockpick") ||
                    puzzleName.Contains("Masterkey") || puzzleName.Contains("Wrench") ||
                    puzzleName.Contains("KeychainHalf"))
                {
                    currentTarget = obj;
                    currentPrompt = GetPromptForObject(obj);
                    return;
                }

                currentTarget = null;
                currentPrompt = "";
            }
            else
            {
                currentTarget = null;
                currentPrompt = "";
            }

            if (heldObject != null)
            {
                currentPrompt = "Press F to drop";
            }
        }

        void HandleInteraction()
        {
            // Update prompt UI
            if (promptText != null)
            {
                if (!string.IsNullOrEmpty(currentPrompt))
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = currentPrompt;
                }
                else
                {
                    promptText.gameObject.SetActive(false);
                }
            }

            // E key or left click = interact
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                if (currentTarget == null) return;

                // FPSInteractable (custom)
                var fpsInteract = currentTarget.GetComponent<FPSInteractable>();
                if (fpsInteract != null)
                {
                    fpsInteract.OnInteract();
                    return;
                }

                // Pickup items
                HandlePickup(currentTarget);
            }
        }

        void HandlePickup(GameObject obj)
        {
            if (heldObject != null) return;

            // Check if this is a known pickup
            string itemId = GetItemId(obj);
            if (itemId == null) return;

            // Register in inventory
            PlayerInventory.Instance?.AddItem(itemId);

            // Handle game state hooks
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
                    break;
            }

            // Hold the object
            heldObject = obj;
            heldRb = obj.GetComponent<Rigidbody>();
            if (heldRb != null)
            {
                heldRb.isKinematic = true;
                heldRb.useGravity = false;
            }

            obj.transform.SetParent(holdPoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            // Disable collider so it doesn't interfere
            var col = obj.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        void HandleDrop()
        {
            if (Input.GetKeyDown(KeyCode.F) && heldObject != null)
            {
                var obj = heldObject;
                heldObject = null;

                obj.transform.SetParent(null);
                var col = obj.GetComponent<Collider>();
                if (col != null) col.enabled = true;

                heldRb = obj.GetComponent<Rigidbody>();
                if (heldRb != null)
                {
                    heldRb.isKinematic = false;
                    heldRb.useGravity = true;
                }
            }
        }

        string GetItemId(GameObject obj)
        {
            string name = obj.name;
            if (name.Contains("Lockpick")) return "Lockpick";
            if (name.Contains("Masterkey")) return "Masterkey";
            if (name.Contains("Wrench")) return "Wrench";
            if (name.Contains("KeychainHalf_A") || name.Contains("KeychainHalf1")) return "KeychainPieceA";
            if (name.Contains("KeychainHalf_B") || name.Contains("KeychainHalf2")) return "KeychainPieceB";
            return null;
        }

        string GetPromptForObject(GameObject obj)
        {
            string name = obj.name;
            if (name.Contains("Lockpick")) return "Press E to pick up Lockpick";
            if (name.Contains("Masterkey")) return "Press E to pick up Masterkey";
            if (name.Contains("Wrench")) return "Press E to pick up Wrench";
            if (name.Contains("KeychainHalf")) return "Press E to pick up Keychain piece";
            if (name.Contains("ColorBox")) return "Press E to activate";
            if (name.Contains("ServerCode") || name.Contains("PowerRestoration")) return "Press E to access keypad";
            if (name.Contains("CCTV")) return "Press E to review footage";
            if (name.Contains("StuckDrawer")) return "Press E to inspect drawer";
            if (name.Contains("SecretComputer")) return "Press E to access computer";
            if (name.Contains("FinalDecision")) return "Press E to make decision";
            if (name.Contains("WallPainting")) return "Press E to examine painting";
            return "Press E to interact";
        }
    }

    /// <summary>
    /// Simple interactable component for FPS mode.
    /// Attach to any object that should respond to E key press.
    /// </summary>
    public class FPSInteractable : MonoBehaviour
    {
        public string interactPrompt = "Press E to interact";

        public virtual void OnInteract()
        {
            Debug.Log($"Interacted with {gameObject.name}");
        }
    }
}
