using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using TruthWithTrenchcoat.Core;
using TruthWithTrenchcoat.FPS;
using TruthWithTrenchcoat.UI;

namespace TruthWithTrenchcoat.EditorTools
{
    /// <summary>
    /// Builds a complete non-VR (FPS) playable scene.
    /// Uses the same environment as the VR version but with FPS player,
    /// raycast interaction, and no XR dependencies.
    ///
    /// Menu: Truth With Trenchcoat > Build FPS Scene (No VR)
    /// </summary>
    public static class FPSBuilder
    {
        [MenuItem("Truth With Trenchcoat/Build FPS Scene (No VR)")]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                "Build FPS Scene",
                "This creates a complete playable scene with keyboard/mouse controls.\n\nNo VR headset required. You can press Play immediately after.",
                "Build", "Cancel"))
                return;

            // Build the environment using the detailed builder
            TruthEnvironmentBuilder.Build();

            // Find the generated environment
            var env = GameObject.Find("TRUTH_ENVIRONMENT");
            if (env == null) return;

            // Add FPS player
            BuildPlayer(env.transform);

            // Add FPS interaction system
            BuildInteraction(env.transform);

            // Add puzzle bridge
            BuildPuzzleBridge(env.transform);

            // Add game managers
            BuildManagers(env.transform);

            // Add UI
            BuildUI(env.transform);

            // Add ending sequences
            BuildEndings(env.transform);

            // Add crosshair
            BuildCrosshair();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorUtility.DisplayDialog(
                "FPS Scene Ready",
                "Scene built with:\n\n" +
                "- WASD movement + mouse look\n" +
                "- Sprint (Shift) + Jump (Space)\n" +
                "- E key to interact with objects\n" +
                "- F key to drop held items\n" +
                "- Crosshair for aiming\n\n" +
                "Press Play to start playing!\n" +
                "Find lockpick -> solve puzzles -> uncover the truth.",
                "Play!");
        }

        private static void BuildPlayer(Transform parent)
        {
            // Player capsule
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "FPS_Player";
            player.tag = "Player";
            player.transform.SetParent(parent, false);
            player.transform.position = new Vector3(-20, 1f, -2);
            player.transform.localScale = new Vector3(0.6f, 1f, 0.6f);

            // Remove default collider, add CharacterController
            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.3f;
            cc.center = new Vector3(0, 0.9f, 0);

            // Make invisible
            var rend = player.GetComponent<Renderer>();
            if (rend != null) rend.enabled = false;

            // Camera
            var camObj = new GameObject("PlayerCamera");
            camObj.transform.SetParent(player.transform);
            camObj.transform.localPosition = new Vector3(0, 1.6f, 0);
            var cam = camObj.AddComponent<Camera>();
            cam.nearClipPlane = 0.1f;
            cam.fieldOfView = 70f;
            camObj.AddComponent<AudioListener>();

            // FPS Controller
            var fps = player.AddComponent<FPSPlayer>();
            fps.playerCamera = cam;

            // Hold point for picked up items
            var holdPoint = new GameObject("HoldPoint");
            holdPoint.transform.SetParent(camObj.transform);
            holdPoint.transform.localPosition = new Vector3(0, 0, 1.5f);
        }

        private static void BuildInteraction(Transform parent)
        {
            var interactor = new GameObject("FPS_Interaction");
            interactor.transform.SetParent(parent, false);

            var fpsPlayer = Object.FindAnyObjectByType<FPSPlayer>();
            var fps = interactor.AddComponent<FPSInteraction>();
            fps.playerCamera = fpsPlayer != null ? fpsPlayer.playerCamera : Camera.main;
        }

        private static void BuildPuzzleBridge(Transform parent)
        {
            var bridge = new GameObject("PuzzleBridge");
            bridge.transform.SetParent(parent, false);

            var fpsPlayer = Object.FindAnyObjectByType<FPSPlayer>();
            var handler = bridge.AddComponent<FPSPuzzleHandler>();
            handler.playerCamera = fpsPlayer != null ? fpsPlayer.playerCamera : Camera.main;
        }

        private static void BuildManagers(Transform parent)
        {
            var managers = new GameObject("_GameManagers");
            managers.transform.SetParent(parent, false);
            managers.AddComponent<GameManager>();
            managers.AddComponent<ClueJournal>();
            managers.AddComponent<PlayerInventory>();
        }

        private static void BuildUI(Transform parent)
        {
            var canvasGo = new GameObject("GameUI_Canvas");
            canvasGo.transform.SetParent(parent, false);
            canvasGo.transform.localPosition = new Vector3(0, 2.2f, -17);
            canvasGo.transform.localScale = Vector3.one * 0.002f;

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;
            canvasGo.AddComponent<GraphicRaycaster>();

            // Instruction panel
            var panel = new GameObject("InstructionPanel");
            panel.transform.SetParent(canvasGo.transform, false);
            var panelImg = panel.AddComponent<Image>();
            panelImg.rectTransform.sizeDelta = new Vector2(800, 120);
            panelImg.rectTransform.anchoredPosition = new Vector2(0, -50);
            panelImg.color = new Color(0.05f, 0.08f, 0.12f, 0.85f);

            var textGo = new GameObject("InstructionText");
            textGo.transform.SetParent(panel.transform, false);
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.rectTransform.sizeDelta = new Vector2(760, 100);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 32;
            tmp.color = new Color(0.8f, 0.9f, 1f);
            tmp.text = "Find a way to open the doors.";

            var iui = canvasGo.AddComponent<InstructionUI>();
            iui.instructionText = tmp;

            // Clue popup
            var popup = new GameObject("CluePopup");
            popup.transform.SetParent(canvasGo.transform, false);
            var popImg = popup.AddComponent<Image>();
            popImg.rectTransform.sizeDelta = new Vector2(650, 180);
            popImg.rectTransform.anchoredPosition = new Vector2(0, 100);
            popImg.color = new Color(0.08f, 0.12f, 0.20f, 0.9f);

            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(popup.transform, false);
            var title = titleObj.AddComponent<TextMeshProUGUI>();
            title.rectTransform.sizeDelta = new Vector2(600, 50);
            title.rectTransform.anchoredPosition = new Vector2(0, 45);
            title.fontSize = 28;
            title.alignment = TextAlignmentOptions.Center;
            title.color = new Color(0.3f, 0.7f, 1f);

            var bodyObj = new GameObject("Body");
            bodyObj.transform.SetParent(popup.transform, false);
            var body = bodyObj.AddComponent<TextMeshProUGUI>();
            body.rectTransform.sizeDelta = new Vector2(600, 100);
            body.rectTransform.anchoredPosition = new Vector2(0, -15);
            body.fontSize = 22;
            body.alignment = TextAlignmentOptions.Center;
            body.color = new Color(0.85f, 0.88f, 0.92f);

            var cjui = canvasGo.AddComponent<ClueJournalUI>();
            cjui.popupRoot = popup;
            cjui.popupTitleText = title;
            cjui.popupBodyText = body;
            popup.SetActive(false);

            // FPS prompt text (bottom center of screen)
            var promptCanvas = new GameObject("PromptCanvas");
            promptCanvas.transform.SetParent(parent, false);
            promptCanvas.transform.localPosition = new Vector3(0, 1.2f, -17);
            promptCanvas.transform.localScale = Vector3.one * 0.001f;

            var promptCvs = promptCanvas.AddComponent<Canvas>();
            promptCvs.renderMode = RenderMode.WorldSpace;
            promptCanvas.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 10;

            var promptPanel = new GameObject("PromptPanel");
            promptPanel.transform.SetParent(promptCanvas.transform, false);
            var promptImg = promptPanel.AddComponent<Image>();
            promptImg.rectTransform.sizeDelta = new Vector2(500, 60);
            promptImg.rectTransform.anchoredPosition = new Vector2(0, 0);
            promptImg.color = new Color(0.0f, 0.0f, 0.0f, 0.7f);

            var promptTextObj = new GameObject("PromptText");
            promptTextObj.transform.SetParent(promptPanel.transform, false);
            var promptTMP = promptTextObj.AddComponent<TextMeshProUGUI>();
            promptTMP.rectTransform.sizeDelta = new Vector2(480, 50);
            promptTMP.alignment = TextAlignmentOptions.Center;
            promptTMP.fontSize = 24;
            promptTMP.color = new Color(0.9f, 0.95f, 1f);
            promptTMP.text = "";

            // Wire prompt to FPS interaction
            var fpsInteraction = Object.FindAnyObjectByType<FPSInteraction>();
            if (fpsInteraction != null)
                fpsInteraction.promptText = promptTMP;

            promptPanel.SetActive(false);
            if (fpsInteraction != null)
            {
                // Store reference for activation
                var promptSetter = promptCanvas.AddComponent<PromptActivator>();
                promptSetter.promptPanel = promptPanel;
                promptSetter.promptText = promptTMP;
                fpsInteraction.promptText = promptTMP;
            }
        }

        private static void BuildEndings(Transform parent)
        {
            // Report ending
            var report = new GameObject("Ending_Report");
            report.transform.SetParent(parent, false);
            report.transform.localPosition = new Vector3(0, 2, -18);
            var reportTM = report.AddComponent<TextMeshPro>();
            reportTM.text = "CASE CLOSED\n\nAuthorities remain suspicious.\nYour investigation is over, but the truth remains hidden.";
            reportTM.fontSize = 0.35f;
            reportTM.alignment = TextAlignmentOptions.Center;
            reportTM.color = new Color(0.8f, 0.6f, 0.2f);
            report.SetActive(false);

            // Protect ending
            var protect = new GameObject("Ending_Protect");
            protect.transform.SetParent(parent, false);
            protect.transform.localPosition = new Vector3(0, 2, -18);
            var protectTM = protect.AddComponent<TextMeshPro>();
            protectTM.text = "TRUTH PROTECTED\n\nYou chose to hide the evidence.\nDr. Arjun's work remains safe, but at what cost?";
            protectTM.fontSize = 0.35f;
            protectTM.alignment = TextAlignmentOptions.Center;
            protectTM.color = new Color(0.3f, 0.7f, 1f);
            protect.SetActive(false);

            // Expose ending
            var expose = new GameObject("Ending_Expose");
            expose.transform.SetParent(parent, false);
            expose.transform.localPosition = new Vector3(0, 2, -18);
            var exposeTM = expose.AddComponent<TextMeshPro>();
            exposeTM.text = "TRUTH REVEALED\n\nYou exposed the organization to the world.\nDr. Arjun's sacrifice was not in vain.";
            exposeTM.fontSize = 0.35f;
            exposeTM.alignment = TextAlignmentOptions.Center;
            exposeTM.color = new Color(0.2f, 1f, 0.4f);
            expose.SetActive(false);

            // Decision console buttons - make them respond to FPSInteraction
            var reportBtn = GameObject.Find("Button_REPORT");
            var protectBtn = GameObject.Find("Button_PROTECT");
            var exposeBtn = GameObject.Find("Button_EXPOSE");

            if (reportBtn != null) reportBtn.AddComponent<FPSInteractable>().interactPrompt = "Press E: REPORT - Authorities get the evidence";
            if (protectBtn != null) protectBtn.AddComponent<FPSInteractable>().interactPrompt = "Press E: PROTECT - Hide the evidence";
            if (exposeBtn != null) exposeBtn.AddComponent<FPSInteractable>().interactPrompt = "Press E: EXPOSE - Release to the public";
        }

        private static void BuildCrosshair()
        {
            var crosshairCanvas = new GameObject("CrosshairCanvas");
            crosshairCanvas.transform.SetParent(Camera.main != null ? Camera.main.transform : GameObject.Find("PlayerCamera").transform, false);
            crosshairCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            crosshairCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            crosshairCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);

            // Center dot
            var dot = new GameObject("CrosshairDot");
            dot.transform.SetParent(crosshairCanvas.transform, false);
            var dotImg = dot.AddComponent<Image>();
            dotImg.rectTransform.sizeDelta = new Vector2(4, 4);
            dotImg.rectTransform.anchoredPosition = Vector2.zero;
            dotImg.color = new Color(1f, 1f, 1f, 0.8f);

            // Horizontal lines
            for (int i = -1; i <= 1; i += 2)
            {
                var line = new GameObject("CrosshairLine");
                line.transform.SetParent(crosshairCanvas.transform, false);
                var lineImg = line.AddComponent<Image>();
                lineImg.rectTransform.sizeDelta = new Vector2(12, 2);
                lineImg.rectTransform.anchoredPosition = new Vector2(i * 15, 0);
                lineImg.color = new Color(1f, 1f, 1f, 0.6f);
            }

            // Vertical lines
            for (int i = -1; i <= 1; i += 2)
            {
                var line = new GameObject("CrosshairVLine");
                line.transform.SetParent(crosshairCanvas.transform, false);
                var lineImg = line.AddComponent<Image>();
                lineImg.rectTransform.sizeDelta = new Vector2(2, 12);
                lineImg.rectTransform.anchoredPosition = new Vector2(0, i * 15);
                lineImg.color = new Color(1f, 1f, 1f, 0.6f);
            }
        }
    }

    /// <summary>
    /// Helper to show/hide the interaction prompt panel.
    /// </summary>
    public class PromptActivator : MonoBehaviour
    {
        public GameObject promptPanel;
        public TextMeshProUGUI promptText;
    }
}
