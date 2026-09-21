using UnityEngine;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Runtime
{
    /// <summary>
    /// Reveals the Masterkey when Task 4 becomes active.
    /// The design document says the Masterkey is progression-gated behind the
    /// earlier office task; this helper makes that gate explicit.
    /// </summary>
    public class MasterkeyRevealController : MonoBehaviour
    {
        public GameObject masterkeyObject;

        private void Start()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnTaskChanged += HandleTaskChanged;
            HandleTaskChanged(GameManager.Instance.CurrentTask);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnTaskChanged -= HandleTaskChanged;
        }

        private void HandleTaskChanged(GameManager.Task task)
        {
            if (masterkeyObject == null) return;
            bool reveal = task >= GameManager.Task.Task4_UnlockSecurityRoom;
            masterkeyObject.SetActive(reveal);
        }
    }
}
