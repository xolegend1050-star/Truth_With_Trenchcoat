using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TruthWithTrenchcoat.Core;

namespace TruthWithTrenchcoat.Puzzles
{
    /// <summary>
    /// Task 2 — Restore the Lights.
    /// Fixed correct order: Blue, Green, Yellow, Red.
    /// The 4 boxes are physically jumbled in the scene (place them in any
    /// visual order you like); the CORRECT click order is always Blue→Green→Yellow→Red.
    /// A hint panel elsewhere in the room should display this order to the player
    /// (e.g. as 4 colored dots/icons) — wire hintPanelText or a UI Image array to
    /// ShowHint() if you want it revealed only after inspecting a specific note.
    /// </summary>
    public class ColorSequencePuzzle : MonoBehaviour
    {
        public enum BoxColor { Blue, Green, Yellow, Red }

        [System.Serializable]
        public class ColorBox
        {
            public BoxColor color;
            public XRSimpleInteractable interactable;
        }

        [Header("Setup — assign the 4 boxes in ANY physical/jumbled order")]
        public List<ColorBox> boxes;

        [Header("Fixed correct sequence (design-locked, do not randomize)")]
        private readonly BoxColor[] correctSequence =
        {
            BoxColor.Blue, BoxColor.Green, BoxColor.Yellow, BoxColor.Red
        };

        [Header("Feedback")]
        public GameObject lightsOffRoot;   // objects representing the "off/flickering" state
        public GameObject lightsOnRoot;    // objects representing the "restored" state
        public AudioSource correctSfx;
        public AudioSource wrongSfx;

        private readonly List<BoxColor> playerInput = new List<BoxColor>();
        private bool solved = false;

        private void Awake()
        {
            foreach (var box in boxes)
            {
                var capturedColor = box.color; // local copy for closure
                box.interactable.selectEntered.AddListener(_ => OnBoxPressed(capturedColor));
            }
        }

        private void OnBoxPressed(BoxColor color)
        {
            if (solved) return;

            playerInput.Add(color);
            int idx = playerInput.Count - 1;

            if (playerInput[idx] != correctSequence[idx])
            {
                // Wrong entry — reset immediately
                wrongSfx?.Play();
                playerInput.Clear();
                return;
            }

            if (playerInput.Count == correctSequence.Length)
            {
                Solve();
            }
        }

        private void Solve()
        {
            solved = true;
            correctSfx?.Play();

            if (lightsOffRoot != null) lightsOffRoot.SetActive(false);
            if (lightsOnRoot != null) lightsOnRoot.SetActive(true);

            var gm = GameManager.Instance;
            gm.LightsRestored = true;
            gm.AdvanceTo(GameManager.Task.Task3_InspectOffice);
        }
    }
}
