using System;
using UnityEngine;

namespace TruthWithTrenchcoat.Core
{
    /// <summary>
    /// The single source of truth for game progression. Strictly linear:
    /// only one task is ever active. Other systems (doors, puzzles, UI)
    /// read CurrentTask and subscribe to OnTaskChanged rather than tracking
    /// their own progression state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum Task
        {
            Task1_FindLockpick = 1,
            Task2_RestoreLights = 2,
            Task3_InspectOffice = 3,
            Task4_UnlockSecurityRoom = 4,
            Task5_RestoreServer = 5,
            Task6_CollectClue1 = 6,
            Task7_FindWrench = 7,
            Task8_OpenSecretLab = 8,
            Task9_CollectClue3 = 9,
            Task10_FinalDecision = 10,
            GameComplete = 11
        }

        [Header("Current Progress")]
        [SerializeField] private Task currentTask = Task.Task1_FindLockpick;

        // Discovery flags — used by UI to decide which hint text to show
        [Header("Discovery Flags")]
        public bool HasLockpick = false;
        public bool HasMasterkey = false;
        public bool HasWrench = false;
        public bool ServerRestored = false;
        public bool LightsRestored = false;
        public bool SecretRoomOpen = false;

        public Task CurrentTask => currentTask;

        public event Action<Task> OnTaskChanged;
        public event Action OnGameComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Fire once at boot so UI initializes to Task 1 text.
            OnTaskChanged?.Invoke(currentTask);
        }

        /// <summary>
        /// Advances to a specific task. Puzzle scripts call this directly
        /// when their condition is met (e.g. ColorSequencePuzzle calls
        /// AdvanceTo(Task3_InspectOffice) once lights are restored).
        /// Advancing is idempotent/guarded: you can't skip backwards and
        /// you can't double-fire the same transition.
        /// </summary>
        public void AdvanceTo(Task next)
        {
            if (next <= currentTask) return; // never move backwards, never re-fire same step
            currentTask = next;
            Debug.Log($"[GameManager] Task advanced -> {currentTask}");
            OnTaskChanged?.Invoke(currentTask);

            if (currentTask == Task.GameComplete)
            {
                OnGameComplete?.Invoke();
            }
        }

        public void AdvanceToNext()
        {
            int nextIndex = (int)currentTask + 1;
            if (Enum.IsDefined(typeof(Task), nextIndex))
            {
                AdvanceTo((Task)nextIndex);
            }
        }

        public bool IsTaskActive(Task t) => currentTask == t;
        public bool IsTaskCompleteOrPast(Task t) => currentTask > t;
    }
}
