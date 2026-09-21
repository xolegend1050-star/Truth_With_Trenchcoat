using UnityEngine;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Puzzles
{
    /// <summary>
    /// Task 10 — Final Decision. Wire the 3 buttons on your decision console
    /// UI to the three public methods below (Unity UI Button OnClick, or
    /// XRSimpleInteractable selectEntered on 3D buttons — either works).
    /// This is the ONLY branch point in the whole game; nothing earlier
    /// affects which endings are available.
    /// </summary>
    public class FinalDecisionManager : MonoBehaviour
    {
        public enum Ending { Report, Protect, Expose }

        [Header("Ending Scenes / Sequences")]
        public GameObject reportEndingSequence;   // "Case Closed" — authorities remain suspicious
        public GameObject protectEndingSequence;  // "Truth Protected" — detective taken into custody
        public GameObject exposeEndingSequence;   // "Truth Revealed" — player escapes

        [Header("Gate")]
        [Tooltip("Decision console should be inert until Task 10 is actually active.")]
        public GameObject decisionConsoleInteractableRoot;

        private bool decisionMade = false;

        private void Start()
        {
            GameManager.Instance.OnTaskChanged += OnTaskChanged;
            OnTaskChanged(GameManager.Instance.CurrentTask);
        }

        private void OnTaskChanged(GameManager.Task task)
        {
            bool active = task == GameManager.Task.Task10_FinalDecision;
            if (decisionConsoleInteractableRoot != null)
                decisionConsoleInteractableRoot.SetActive(active);
        }

        // ---- Hook these to your 3 UI/3D buttons ----

        public void ChooseReport()  => ResolveEnding(Ending.Report);
        public void ChooseProtect() => ResolveEnding(Ending.Protect);
        public void ChooseExpose()  => ResolveEnding(Ending.Expose);

        private void ResolveEnding(Ending ending)
        {
            if (decisionMade) return;
            if (GameManager.Instance.CurrentTask != GameManager.Task.Task10_FinalDecision) return;
            decisionMade = true;

            reportEndingSequence?.SetActive(false);
            protectEndingSequence?.SetActive(false);
            exposeEndingSequence?.SetActive(false);

            switch (ending)
            {
                case Ending.Report:
                    // Consequence: authorities remain suspicious of the detective.
                    reportEndingSequence?.SetActive(true);
                    break;
                case Ending.Protect:
                    // Consequence: the detective is taken into custody.
                    protectEndingSequence?.SetActive(true);
                    break;
                case Ending.Expose:
                    // Consequence: the player successfully escapes with the truth.
                    exposeEndingSequence?.SetActive(true);
                    break;
            }

            GameManager.Instance.AdvanceTo(GameManager.Task.GameComplete);
        }
    }
}
