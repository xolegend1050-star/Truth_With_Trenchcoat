using UnityEngine;
using TruthWithTrenchcoat.Core;
using TruthWithTrenchcoat.Puzzles;
using TruthWithTrenchcoat.UI;

namespace TruthWithTrenchcoat.FPS
{
    /// <summary>
    /// Bridges FPS input (E key, clicks) to the existing puzzle scripts.
    /// Attach this to a "_PuzzleBridge" GameObject in the scene.
    /// It finds puzzle objects by name and handles their interaction logic
    /// without requiring XR Interaction Toolkit.
    /// </summary>
    public class FPSPuzzleHandler : MonoBehaviour
    {
        public Camera playerCamera;
        public float interactRange = 3f;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                TryInteract();
            }
        }

        void TryInteract()
        {
            if (playerCamera == null) return;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (!Physics.Raycast(ray, out RaycastHit hit, interactRange)) return;

            GameObject obj = hit.collider.gameObject;

            // Color Sequence Puzzle
            if (obj.name.Contains("ColorBox"))
            {
                HandleColorBox(obj);
                return;
            }

            // Server Code Panel
            if (obj.name.Contains("Key_"))
            {
                HandleServerKey(obj);
                return;
            }

            // CCTV
            if (obj.name.Contains("CCTV"))
            {
                HandleCCTV(obj);
                return;
            }

            // Stuck Drawer
            if (obj.name.Contains("StuckDrawer"))
            {
                HandleDrawer(obj);
                return;
            }

            // Secret Computer
            if (obj.name.Contains("SecretComputer"))
            {
                HandleSecretComputer(obj);
                return;
            }

            // Wall Painting (secret switch)
            if (obj.name.Contains("WallPainting"))
            {
                HandleWallPainting(obj);
                return;
            }

            // Final Decision Console
            if (obj.name.Contains("FinalDecision") || obj.name.Contains("Button_"))
            {
                HandleFinalDecision(obj);
                return;
            }
        }

        // ==================== COLOR SEQUENCE ====================

        private int colorSequenceIndex = 0;
        private readonly ColorSequencePuzzle.BoxColor[] correctSequence = {
            ColorSequencePuzzle.BoxColor.Blue,
            ColorSequencePuzzle.BoxColor.Green,
            ColorSequencePuzzle.BoxColor.Yellow,
            ColorSequencePuzzle.BoxColor.Red
        };

        void HandleColorBox(GameObject obj)
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.CurrentTask != GameManager.Task.Task2_RestoreLights) return;

            ColorSequencePuzzle.BoxColor color = GetBoxColor(obj);
            if (color == correctSequence[colorSequenceIndex])
            {
                colorSequenceIndex++;
                // Visual feedback - change color
                var rend = obj.GetComponent<Renderer>();
                if (rend != null) rend.material.color = Color.green * 2f;

                if (colorSequenceIndex >= correctSequence.Length)
                {
                    // Puzzle solved
                    gm.LightsRestored = true;
                    gm.AdvanceTo(GameManager.Task.Task3_InspectOffice);
                    colorSequenceIndex = 0;
                    Debug.Log("[PUZZLE] Lights restored!");
                }
            }
            else
            {
                // Wrong - reset
                colorSequenceIndex = 0;
                Debug.Log("[PUZZLE] Wrong sequence! Reset.");
            }
        }

        ColorSequencePuzzle.BoxColor GetBoxColor(GameObject obj)
        {
            if (obj.name.Contains("Blue")) return ColorSequencePuzzle.BoxColor.Blue;
            if (obj.name.Contains("Green")) return ColorSequencePuzzle.BoxColor.Green;
            if (obj.name.Contains("Yellow")) return ColorSequencePuzzle.BoxColor.Yellow;
            if (obj.name.Contains("Red")) return ColorSequencePuzzle.BoxColor.Red;
            return ColorSequencePuzzle.BoxColor.Blue;
        }

        // ==================== SERVER CODE ====================

        private string currentCode = "";
        private const string correctCode = "1423";

        void HandleServerKey(GameObject obj)
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.CurrentTask != GameManager.Task.Task5_RestoreServer) return;

            // Extract digit from name "Key_0", "Key_1", etc.
            string name = obj.name;
            int lastUnderscore = name.LastIndexOf('_');
            if (lastUnderscore < 0) return;
            string digitStr = name.Substring(lastUnderscore + 1);
            if (!int.TryParse(digitStr, out int digit)) return;

            currentCode += digit.ToString();
            Debug.Log($"[SERVER] Entry: {currentCode}");

            if (currentCode.Length == 4)
            {
                if (currentCode == correctCode)
                {
                    gm.ServerRestored = true;
                    gm.AdvanceTo(GameManager.Task.Task6_CollectClue1);
                    Debug.Log("[PUZZLE] Server restored! Code 1423 accepted.");
                }
                else
                {
                    Debug.Log("[SERVER] Wrong code! Reset.");
                }
                currentCode = "";
            }
        }

        // ==================== CCTV ====================

        void HandleCCTV(GameObject obj)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (!gm.ServerRestored)
            {
                Debug.Log("[CCTV] Server needs to be started first.");
                return;
            }

            if (gm.CurrentTask != GameManager.Task.Task6_CollectClue1) return;

            ClueJournal.Instance?.CollectClue1("CCTV footage missing: 13:38 - 14:52 (1.5 hours).");
            gm.AdvanceTo(GameManager.Task.Task7_FindWrench);
            Debug.Log("[PUZZLE] Clue 1 collected - missing footage.");
        }

        // ==================== DRAWER ====================

        void HandleDrawer(GameObject obj)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            bool hasWrench = PlayerInventory.Instance != null && PlayerInventory.Instance.HasItem("Wrench");

            if (!hasWrench)
            {
                Debug.Log("[DRAWER] The drawer is stuck. Need a wrench.");
                return;
            }

            gm.AdvanceTo(GameManager.Task.Task4_UnlockSecurityRoom);
            Debug.Log("[PUZZLE] Drawer opened! Task 3 complete.");
        }

        // ==================== SECRET COMPUTER ====================

        void HandleSecretComputer(GameObject obj)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (!gm.ServerRestored)
            {
                Debug.Log("[COMPUTER] Server needs to be started first.");
                return;
            }

            if (gm.CurrentTask != GameManager.Task.Task9_CollectClue3) return;

            ClueJournal.Instance?.CollectClue3();
            gm.AdvanceTo(GameManager.Task.Task10_FinalDecision);
            Debug.Log("[PUZZLE] Clue 3 collected - Project ECHO revealed.");
        }

        // ==================== WALL PAINTING ====================

        void HandleWallPainting(GameObject obj)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            bool hasKeychainA = PlayerInventory.Instance != null && PlayerInventory.Instance.HasItem("KeychainPieceA");

            if (!hasKeychainA)
            {
                Debug.Log("[PAINTING] The painting doesn't move... yet.");
                return;
            }

            gm.SecretRoomOpen = true;
            gm.AdvanceTo(GameManager.Task.Task9_CollectClue3);

            // Try to slide the wall
            var wall = GameObject.Find("SecretSlidingWall");
            if (wall != null)
            {
                wall.transform.localPosition += new Vector3(3f, 0, 0);
            }

            Debug.Log("[PUZZLE] Secret wall opened!");
        }

        // ==================== FINAL DECISION ====================

        void HandleFinalDecision(GameObject obj)
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.CurrentTask != GameManager.Task.Task10_FinalDecision) return;

            string name = obj.name;
            if (name.Contains("REPORT"))
            {
                Debug.Log("[ENDING] CASE CLOSED - Authorities remain suspicious.");
                gm.AdvanceTo(GameManager.Task.GameComplete);
            }
            else if (name.Contains("PROTECT"))
            {
                Debug.Log("[ENDING] TRUTH PROTECTED - Detective taken into custody.");
                gm.AdvanceTo(GameManager.Task.GameComplete);
            }
            else if (name.Contains("EXPOSE"))
            {
                Debug.Log("[ENDING] TRUTH REVEALED - You escaped with the truth.");
                gm.AdvanceTo(GameManager.Task.GameComplete);
            }
        }
    }
}
