using UnityEngine;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Interaction
{
    /// <summary>
    /// Doors auto-trigger open once their unlock condition is satisfied
    /// (per the design doc, Section 3.3 — no manual code entry on standard
    /// doors, only on dedicated puzzles). Attach to every hinged/sliding
    /// door except the Office↔SecretRoom wall, which uses SecretWallSwitch.cs.
    /// </summary>
    public class DoorController : MonoBehaviour
    {
        public enum UnlockCondition
        {
            None,               // always unlocked
            RequiresLockpick,   // Task 1
            RequiresMasterkey,  // Task 4
            RequiresServerPower // e.g. Storage door, opens alongside Server Room
        }

        [Header("Setup")]
        public UnlockCondition condition = UnlockCondition.RequiresLockpick;
        public Animator doorAnimator;                 // optional, drives an "IsOpen" bool
        public Transform doorTransform;                // used if no animator (simple slide/rotate)
        public Vector3 openLocalPositionOffset = Vector3.zero;
        public float openSpeed = 1.5f;

        private bool isUnlocked = false;
        private bool isOpening = false;
        private Vector3 closedLocalPos;

        private void Start()
        {
            if (doorTransform != null) closedLocalPos = doorTransform.localPosition;

            var gm = GameManager.Instance;
            gm.OnTaskChanged += _ => EvaluateUnlockState();
            EvaluateUnlockState(); // check immediately in case already satisfied
        }

        private void EvaluateUnlockState()
        {
            if (isUnlocked) return;
            var gm = GameManager.Instance;

            bool conditionMet = condition switch
            {
                UnlockCondition.None => true,
                UnlockCondition.RequiresLockpick => gm.HasLockpick,
                UnlockCondition.RequiresMasterkey => gm.HasMasterkey,
                UnlockCondition.RequiresServerPower => gm.ServerRestored,
                _ => false
            };

            if (conditionMet) Unlock();
        }

        public void Unlock()
        {
            if (isUnlocked) return;
            isUnlocked = true;
            Open();
        }

        private void Open()
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("IsOpen", true);
                return;
            }
            if (doorTransform != null && !isOpening)
            {
                isOpening = true;
                StartCoroutine(SlideOpen());
            }
        }

        private System.Collections.IEnumerator SlideOpen()
        {
            Vector3 target = closedLocalPos + openLocalPositionOffset;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                doorTransform.localPosition = Vector3.Lerp(closedLocalPos, target, t);
                yield return null;
            }
        }
    }
}
