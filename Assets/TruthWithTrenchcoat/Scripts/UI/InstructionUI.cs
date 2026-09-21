using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.UI
{
    /// <summary>
    /// Displays the short on-screen instruction for whatever task is currently
    /// active (per design doc Section 10). Attach to a world-space TMP canvas
    /// (e.g. a wrist-mounted panel, standard for VR) or a screen-space canvas.
    /// Text updates automatically whenever GameManager.OnTaskChanged fires.
    /// </summary>
    public class InstructionUI : MonoBehaviour
    {
        public TMP_Text instructionText;

        private Dictionary<GameManager.Task, string> instructions;

        private void Awake()
        {
            instructions = new Dictionary<GameManager.Task, string>
            {
                { GameManager.Task.Task1_FindLockpick,      "Find a way to open the doors." },
                { GameManager.Task.Task2_RestoreLights,     "Restore power to the rest of the lab." },
                { GameManager.Task.Task3_InspectOffice,     "Inspect the office. Something is hidden here." },
                { GameManager.Task.Task4_UnlockSecurityRoom,"Needs a masterkey." },
                { GameManager.Task.Task5_RestoreServer,     "Find the code to restore server power." },
                { GameManager.Task.Task6_CollectClue1,      "Review the security footage." },
                { GameManager.Task.Task7_FindWrench,        "Needs a wrench to be fixed." },
                { GameManager.Task.Task8_OpenSecretLab,     "Something is hidden behind this wall." },
                { GameManager.Task.Task9_CollectClue3,      "Uncover the truth." },
                { GameManager.Task.Task10_FinalDecision,    "Decide what to do with what you've found." },
                { GameManager.Task.GameComplete,            "" },
            };
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTaskChanged += UpdateInstruction;
                UpdateInstruction(GameManager.Instance.CurrentTask);
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnTaskChanged -= UpdateInstruction;
        }

        private void UpdateInstruction(GameManager.Task task)
        {
            // "Needs a masterkey" (and similarly gated hints) should only show
            // BEFORE that item is found — per design doc Section 10. Since the
            // masterkey and wrench hints are tied 1:1 to their own task steps,
            // this happens automatically here: once GameManager advances past
            // that task, the dictionary lookup below naturally shows the next
            // task's text instead.
            if (instructions.TryGetValue(task, out string text) && instructionText != null)
            {
                instructionText.text = text;
            }
        }
    }
}
