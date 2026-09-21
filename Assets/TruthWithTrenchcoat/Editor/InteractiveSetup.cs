using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TruthWithTrenchcoat.Core;
using TruthWithTrenchcoat.Interaction;
using TruthWithTrenchcoat.Puzzles;
using TruthWithTrenchcoat.Runtime;

namespace TruthWithTrenchcoat.EditorTools
{
    /// <summary>
    /// Wires XR interaction components and game logic onto generated scene objects.
    /// Attach DoorController, CollectibleItem, puzzle scripts, and XR interactables
    /// to the appropriate GameObjects in the generated environment.
    /// </summary>
    public static class InteractiveSetup
    {
        public static void Wire(Transform root)
        {
            WireDoors(root);
            WirePickups(root);
            WirePuzzles(root);
            WireUI(root);
            WireRuntimeHelpers(root);
        }

        // ==================== DOORS ====================

        private static void WireDoors(Transform root)
        {
            WireDoor(root, "Door_MainEntrance",
                DoorController.UnlockCondition.RequiresLockpick,
                new Vector3(0, 0, 1.5f));

            WireDoor(root, "Door_Office",
                DoorController.UnlockCondition.RequiresLockpick,
                new Vector3(0, 0, 1.5f));

            WireDoor(root, "Door_Bathroom",
                DoorController.UnlockCondition.RequiresLockpick,
                new Vector3(0, 0, 1.5f));

            WireDoor(root, "Door_ServerRoom",
                DoorController.UnlockCondition.RequiresMasterkey,
                new Vector3(0, 0, 1.5f));

            WireDoor(root, "Door_SecurityRoom",
                DoorController.UnlockCondition.RequiresMasterkey,
                new Vector3(1.5f, 0, 0));

            WireDoor(root, "Door_Storage",
                DoorController.UnlockCondition.RequiresMasterkey,
                new Vector3(-1.5f, 0, 0));

            // Secret wall (sliding, not a normal door)
            WireSecretWall(root);
        }

        private static void WireDoor(Transform root, string doorName,
            DoorController.UnlockCondition condition, Vector3 openOffset)
        {
            var doorGo = FindDeep(root, doorName);
            if (doorGo == null) return;

            var dc = doorGo.GetComponent<DoorController>();
            if (dc == null) dc = doorGo.AddComponent<DoorController>();

            dc.condition = condition;
            dc.doorTransform = doorGo.transform;
            dc.openLocalPositionOffset = openOffset;
            dc.openSpeed = 1.5f;
        }

        private static void WireSecretWall(Transform root)
        {
            var wallGo = FindDeep(root, "SecretSlidingWall");
            if (wallGo == null) return;

            var paintingGo = FindDeep(root, "WallPainting");
            if (paintingGo == null) return;

            // Add XR interactable to painting
            var xr = paintingGo.GetComponent<XRSimpleInteractable>();
            if (xr == null) xr = paintingGo.AddComponent<XRSimpleInteractable>();

            // Add SecretWallSwitch
            var sw = paintingGo.GetComponent<SecretWallSwitch>();
            if (sw == null) sw = paintingGo.AddComponent<SecretWallSwitch>();
            sw.slidingWallTransform = wallGo.transform;
            sw.openLocalOffset = new Vector3(3f, 0, 0);
            sw.openSpeed = 1f;
        }

        // ==================== PICKUPS ====================

        private static void WirePickups(Transform root)
        {
            WireGrabPickup(root, "Lockpick", "Lockpick");
            WireGrabPickup(root, "Masterkey", "Masterkey");
            WireGrabPickup(root, "Wrench", "Wrench");
            WireGrabPickup(root, "KeychainHalf_A", "KeychainPieceA");
            WireGrabPickup(root, "KeychainHalf_B", "KeychainPieceB");
        }

        private static void WireGrabPickup(Transform root, string objName, string itemId)
        {
            var go = FindDeep(root, objName);
            if (go == null) return;

            // Ensure collider for grab
            if (go.GetComponent<Collider>() == null)
                go.AddComponent<BoxCollider>();

            var grab = go.GetComponent<XRGrabInteractable>();
            if (grab == null) grab = go.AddComponent<XRGrabInteractable>();

            var item = go.GetComponent<CollectibleItem>();
            if (item == null) item = go.AddComponent<CollectibleItem>();
            item.itemId = itemId;
            item.keepAsHeldObject = true;

            // Masterkey starts hidden
            if (itemId == "Masterkey")
                go.SetActive(false);
        }

        // ==================== PUZZLES ====================

        private static void WirePuzzles(Transform root)
        {
            WireColorSequence(root);
            WireServerCodePanel(root);
            WireCCTV(root);
            WireDrawer(root);
            WireSecretComputer(root);
            WireFinalDecision(root);
        }

        private static void WireColorSequence(Transform root)
        {
            var puzzleRoot = FindDeep(root, "LightsPuzzle");
            if (puzzleRoot == null) return;

            var puzzle = puzzleRoot.GetComponent<ColorSequencePuzzle>();
            if (puzzle == null) puzzle = puzzleRoot.AddComponent<ColorSequencePuzzle>();

            // Find the 4 color boxes and wire them
            var blueBox = FindDeep(puzzleRoot.transform, "ColorBox_Blue");
            var greenBox = FindDeep(puzzleRoot.transform, "ColorBox_Green");
            var yellowBox = FindDeep(puzzleRoot.transform, "ColorBox_Yellow");
            var redBox = FindDeep(puzzleRoot.transform, "ColorBox_Red");

            var boxes = new System.Collections.Generic.List<ColorSequencePuzzle.ColorBox>();

            if (blueBox != null) { EnsureXRSimple(blueBox); boxes.Add(new ColorSequencePuzzle.ColorBox { color = ColorSequencePuzzle.BoxColor.Blue, interactable = blueBox.GetComponent<XRSimpleInteractable>() }); }
            if (greenBox != null) { EnsureXRSimple(greenBox); boxes.Add(new ColorSequencePuzzle.ColorBox { color = ColorSequencePuzzle.BoxColor.Green, interactable = greenBox.GetComponent<XRSimpleInteractable>() }); }
            if (yellowBox != null) { EnsureXRSimple(yellowBox); boxes.Add(new ColorSequencePuzzle.ColorBox { color = ColorSequencePuzzle.BoxColor.Yellow, interactable = yellowBox.GetComponent<XRSimpleInteractable>() }); }
            if (redBox != null) { EnsureXRSimple(redBox); boxes.Add(new ColorSequencePuzzle.ColorBox { color = ColorSequencePuzzle.BoxColor.Red, interactable = redBox.GetComponent<XRSimpleInteractable>() }); }

            puzzle.boxes = boxes;
        }

        private static void WireServerCodePanel(Transform root)
        {
            var panelGo = FindDeep(root, "PowerRestorationPanel");
            if (panelGo == null) panelGo = FindDeep(root, "ServerCodePanel");
            if (panelGo == null) return;

            var scp = panelGo.GetComponent<ServerCodePanel>();
            if (scp == null) scp = panelGo.AddComponent<ServerCodePanel>();

            // Wire keypad buttons
            for (int i = 0; i < 10; i++)
            {
                var keyGo = FindDeep(panelGo.transform, "Key_" + i);
                if (keyGo == null) continue;
                EnsureXRSimple(keyGo);
                int digit = i;
                var si = keyGo.GetComponent<XRSimpleInteractable>();
                si.selectEntered.AddListener(_ => scp.SubmitDigit(digit));
            }
        }

        private static void WireCCTV(Transform root)
        {
            var cctvGo = FindDeep(root, "CCTV_MonitorBank");
            if (cctvGo == null) cctvGo = FindDeep(root, "CCTV_MonitorWall");
            if (cctvGo == null) return;

            // Find the parent/stand object for the CCTV
            var targetGo = cctvGo.transform.parent != null ? cctvGo.transform.parent.gameObject : cctvGo;

            var cctv = targetGo.GetComponent<CCTVFootagePuzzle>();
            if (cctv == null) cctv = targetGo.AddComponent<CCTVFootagePuzzle>();
            targetGo.AddComponent<XRSimpleInteractable>();

            // Create missing footage display
            var missing = new GameObject("MissingFootageDisplay");
            missing.transform.SetParent(targetGo.transform, false);
            missing.transform.localPosition = new Vector3(0, 1.5f, 0.2f);
            var tm = missing.AddComponent<TMPro.TextMeshPro>();
            tm.text = "CCTV: 1.5 HOUR GAP\n13:38 - 14:52 MISSING";
            tm.fontSize = 0.3f;
            tm.alignment = TMPro.TextAlignmentOptions.Center;
            tm.color = new Color(1f, 0.2f, 0.2f);
            missing.SetActive(false);
            cctv.missingWindowDisplay = missing;
        }

        private static void WireDrawer(Transform root)
        {
            var drawerGo = FindDeep(root, "StuckDrawer");
            if (drawerGo == null) return;

            var drawer = drawerGo.GetComponent<DrawerController>();
            if (drawer == null) drawer = drawerGo.AddComponent<DrawerController>();
            EnsureXRSimple(drawerGo);

            // Find keychain piece A and wire it
            var pieceA = FindDeep(root, "KeychainHalf_A");
            if (pieceA != null)
            {
                drawer.keychainPieceAObject = pieceA;
                pieceA.SetActive(false);
            }
        }

        private static void WireSecretComputer(Transform root)
        {
            var compGo = FindDeep(root, "SecretComputer");
            if (compGo == null) return;

            var comp = compGo.GetComponent<SecretComputer>();
            if (comp == null) comp = compGo.AddComponent<SecretComputer>();
            EnsureXRSimple(compGo);

            // Create ECHO documents reveal
            var docs = new GameObject("ECHO_Documents_Reveal");
            docs.transform.SetParent(root, false);
            docs.transform.localPosition = new Vector3(0, 1.5f, -23);
            var tm = docs.AddComponent<TMPro.TextMeshPro>();
            tm.text = "PROJECT ECHO\nCLASSIFIED DOCUMENTS\n[ADD FINAL ART HERE]";
            tm.fontSize = 0.25f;
            tm.alignment = TMPro.TextAlignmentOptions.Center;
            tm.color = new Color(0.2f, 0.6f, 1f);
            docs.SetActive(false);
            comp.echoDocumentsRevealUI = docs;
        }

        private static void WireFinalDecision(Transform root)
        {
            var consoleGo = FindDeep(root, "FinalDecisionConsole");
            if (consoleGo == null) return;

            var fdm = consoleGo.GetComponent<FinalDecisionManager>();
            if (fdm == null) fdm = consoleGo.AddComponent<FinalDecisionManager>();

            // Create decision buttons
            var btnRoot = new GameObject("DecisionButtons");
            btnRoot.transform.SetParent(consoleGo.transform, false);
            fdm.decisionConsoleInteractableRoot = btnRoot;

            CreateDecisionButton(btnRoot.transform, "REPORT", new Vector3(-1, 0.2f, 0), fdm.ChooseReport);
            CreateDecisionButton(btnRoot.transform, "PROTECT", new Vector3(0, 0.2f, 0), fdm.ChooseProtect);
            CreateDecisionButton(btnRoot.transform, "EXPOSE", new Vector3(1, 0.2f, 0), fdm.ChooseExpose);

            // Create ending sequences
            CreateEnding(root, "Ending_Report", "CASE CLOSED\nAuthorities remain suspicious.", fdm, "report");
            CreateEnding(root, "Ending_Protect", "TRUTH PROTECTED\nDetective taken into custody.", fdm, "protect");
            CreateEnding(root, "Ending_Expose", "TRUTH REVEALED\nYou escaped with the truth.", fdm, "expose");
        }

        private static void CreateDecisionButton(Transform parent, string label, Vector3 pos, UnityEngine.Events.UnityAction action)
        {
            var btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btn.name = "Button_" + label;
            btn.transform.SetParent(parent, false);
            btn.transform.localPosition = pos;
            btn.transform.localScale = new Vector3(0.9f, 0.35f, 0.3f);

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.15f, 0.25f, 0.45f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", new Color(0.1f, 0.2f, 0.5f) * 1.5f);
            btn.GetComponent<Renderer>().sharedMaterial = mat;

            var si = btn.AddComponent<XRSimpleInteractable>();
            si.selectEntered.AddListener(_ => action());

            var tmObj = new GameObject("Label");
            tmObj.transform.SetParent(btn.transform, false);
            tmObj.transform.localPosition = new Vector3(0, 0.25f, 0);
            var tm = tmObj.AddComponent<TMPro.TextMeshPro>();
            tm.text = label;
            tm.fontSize = 0.15f;
            tm.alignment = TMPro.TextAlignmentOptions.Center;
            tm.color = Color.white;
        }

        private static void CreateEnding(Transform root, string name, string text,
            FinalDecisionManager fdm, string type)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root, false);
            go.transform.localPosition = new Vector3(0, 2, -18);
            var tm = go.AddComponent<TMPro.TextMeshPro>();
            tm.text = text;
            tm.fontSize = 0.35f;
            tm.alignment = TMPro.TextAlignmentOptions.Center;
            tm.color = Color.white;
            go.SetActive(false);

            switch (type)
            {
                case "report": fdm.reportEndingSequence = go; break;
                case "protect": fdm.protectEndingSequence = go; break;
                case "expose": fdm.exposeEndingSequence = go; break;
            }
        }

        // ==================== UI ====================

        private static void WireUI(Transform root)
        {
            var canvasGo = new GameObject("GameUI");
            canvasGo.transform.SetParent(root, false);
            canvasGo.transform.localPosition = new Vector3(0, 2.2f, 2.5f);
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
            var tmp = textGo.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.rectTransform.sizeDelta = new Vector2(760, 100);
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
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
            var title = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            title.rectTransform.sizeDelta = new Vector2(600, 50);
            title.rectTransform.anchoredPosition = new Vector2(0, 45);
            title.fontSize = 28;
            title.alignment = TMPro.TextAlignmentOptions.Center;
            title.color = new Color(0.3f, 0.7f, 1f);

            var bodyObj = new GameObject("Body");
            bodyObj.transform.SetParent(popup.transform, false);
            var body = bodyObj.AddComponent<TMPro.TextMeshProUGUI>();
            body.rectTransform.sizeDelta = new Vector2(600, 100);
            body.rectTransform.anchoredPosition = new Vector2(0, -15);
            body.fontSize = 22;
            body.alignment = TMPro.TextAlignmentOptions.Center;
            body.color = new Color(0.85f, 0.88f, 0.92f);

            var cjui = canvasGo.AddComponent<ClueJournalUI>();
            cjui.popupRoot = popup;
            cjui.popupTitleText = title;
            cjui.popupBodyText = body;
            popup.SetActive(false);
        }

        // ==================== RUNTIME HELPERS ====================

        private static void WireRuntimeHelpers(Transform root)
        {
            // Masterkey reveal controller
            var masterkeyGo = FindDeep(root, "Masterkey");
            if (masterkeyGo != null)
            {
                var reveal = root.GetComponentInChildren<MasterkeyRevealController>();
                if (reveal == null)
                {
                    var helperGo = new GameObject("RuntimeHelpers");
                    helperGo.transform.SetParent(root, false);
                    reveal = helperGo.AddComponent<MasterkeyRevealController>();
                }
                reveal.masterkeyObject = masterkeyGo;
            }

            // Room entry trigger for Security Room
            var securityGate = FindDeep(root, "SecurityRoom_Task4_Gate");
            if (securityGate != null)
            {
                var trigger = securityGate.GetComponent<RoomEntryTaskTrigger>();
                if (trigger == null) trigger = securityGate.AddComponent<RoomEntryTaskTrigger>();
                trigger.taskToAdvanceTo = GameManager.Task.Task5_RestoreServer;
            }
        }

        // ==================== HELPERS ====================

        private static void EnsureXRSimple(GameObject go)
        {
            if (go.GetComponent<XRSimpleInteractable>() == null)
                go.AddComponent<XRSimpleInteractable>();
        }

        private static GameObject FindDeep(Transform root, string name)
        {
            foreach (Transform child in root)
            {
                if (child.name == name) return child.gameObject;
                var found = FindDeep(child, name);
                if (found != null) return found;
            }
            return null;
        }
    }
}
