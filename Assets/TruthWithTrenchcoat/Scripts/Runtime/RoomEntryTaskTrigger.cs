using UnityEngine;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Runtime
{
    /// <summary>
    /// Advances the linear task when the player enters a specified progression room.
    /// Put a trigger volume at the Security Room entrance to move Task 4 -> Task 5.
    /// </summary>
    public class RoomEntryTaskTrigger : MonoBehaviour
    {
        public GameManager.Task taskToAdvanceTo = GameManager.Task.Task5_RestoreServer;
        private bool fired;

        private void OnTriggerEnter(Collider other)
        {
            if (fired || GameManager.Instance == null) return;

            // XR Origin commonly has a CharacterController on its root, while
            // the collider entering the trigger may be a child. We accept
            // the XR Origin tag if present, otherwise any collider tagged Player.
            if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
            {
                fired = true;
                GameManager.Instance.AdvanceTo(taskToAdvanceTo);
            }
        }
    }
}
