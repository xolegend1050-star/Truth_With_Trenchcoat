using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TruthWithTrenchcoat.Core;
using TruthWithTrenchcoat.Interaction;
using TruthWithTrenchcoat.Puzzles;
using TruthWithTrenchcoat.UI;
using TruthWithTrenchcoat.Runtime;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
#endif

namespace TruthWithTrenchcoat.EditorTools
{
#if UNITY_EDITOR
    /// <summary>
    /// Builds a playable blockout/scaffold from the supplied GDD coordinates.
    /// It deliberately uses Unity primitives, not final art assets.
    ///
    /// Menu: Tools > Truth With Trenchcoat > Build Prototype Scene
    /// </summary>
    public static class TruthWithTrenchcoatBuilder
    {
        private const string RootName = "TWT_AUTO_GENERATED";

        [MenuItem("Tools/Truth With Trenchcoat/Build Prototype Scene")]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                "Truth With Trenchcoat",
                "This creates a new blockout scene using primitives and wires the supplied scripts/UI. Continue?",
                "Build", "Cancel"))
                return;

            NewScene();
            var root = new GameObject(RootName);

            BuildManagers(root.transform);
            BuildLighting(root.transform);
            BuildRooms(root.transform);
            BuildDoors(root.transform);
            BuildInteractives(root.transform);
            BuildUI(root.transform);
            BuildEndingObjects(root.transform);

            var scene = SceneManager.GetActiveScene();
            EditorSceneManager.SaveScene(scene, "Assets/TruthWithTrenchcoat/Scenes/TruthWithTrenchcoat_Prototype.unity");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Build complete",
                "Prototype scene generated.\n\n" +
                "Next: install XR Interaction Toolkit + Starter Assets, add an XR Origin, " +
                "then press Play and replace primitive props with real models.\n\n" +
                "Audio was intentionally not generated.",
                "OK");
        }

        private static void NewScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            System.IO.Directory.CreateDirectory("Assets/TruthWithTrenchcoat/Scenes");
            if (SceneManager.GetActiveScene().name != "Untitled")
                SceneManager.SetActiveScene(SceneManager.GetActiveScene());
        }

        private static GameObject MakeCube(string name, Transform parent, Vector3 pos, Vector3 scale, bool collider = true)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            if (!collider)
            {
                var c = go.GetComponent<Collider>();
                if (c != null) Object.DestroyImmediate(c);
            }
            return go;
        }

        private static GameObject MakeSphere(string name, Transform parent, Vector3 pos, float radius)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * radius * 2f;
            return go;
        }

        private static void BuildManagers(Transform parent)
        {
            var gm = new GameObject("_GameManagers");
            gm.transform.SetParent(parent, false);
            gm.AddComponent<GameManager>();
            gm.AddComponent<ClueJournal>();
            gm.AddComponent<PlayerInventory>();
        }

        private static void BuildLighting(Transform parent)
        {
            var lights = new GameObject("Lighting");
            lights.transform.SetParent(parent, false);

            var main = new GameObject("MainLab_Light");
            main.transform.SetParent(lights.transform, false);
            main.transform.position = new Vector3(0, 2.8f, 0);
            var ml = main.AddComponent<Light>();
            ml.type = LightType.Area;
            ml.intensity = 800f;
            ml.range = 20f;
            ml.transform.localScale = new Vector3(5, 1, 5);

            foreach (var data in new[]
            {
                ("Security_Blue", new Vector3(14,2.5f,10), new Color(0.2f,0.35f,1f)),
                ("Server_Blue", new Vector3(0,2.5f,10), new Color(0.2f,0.35f,1f)),
                ("Storage", new Vector3(-14,2.5f,10), new Color(1f,0.9f,0.75f)),
                ("Office", new Vector3(0,2.5f,-12), new Color(1f,0.85f,0.65f)),
                ("Secret", new Vector3(0,2.5f,-22), new Color(0.35f,0.45f,0.7f))
            })
            {
                var go = new GameObject(data.Item1);
                go.transform.SetParent(lights.transform, false);
                go.transform.position = data.Item2;
                var l = go.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 10f;
                l.intensity = 250f;
                l.color = data.Item3;
            }
        }

        private static void BuildRooms(Transform parent)
        {
            var rooms = new[]
            {
                ("Entrance", new Vector3(-20,0,0), new Vector2(6,6), 3f),
                ("MainLab", new Vector3(0,0,0), new Vector2(14,14), 3.5f),
                ("DrsOffice", new Vector3(0,0,-12), new Vector2(7,7), 3f),
                ("SecretRoom", new Vector3(0,0,-22), new Vector2(9,9), 3f),
                ("Bathroom", new Vector3(16,0,0), new Vector2(5,5), 3f),
                ("SecurityRoom", new Vector3(14,0,10), new Vector2(7,7), 3f),
                ("ServerRoom", new Vector3(0,0,10), new Vector2(7,7), 3f),
                ("Storage", new Vector3(-14,0,10), new Vector2(7,7), 3f)
            };

            foreach (var r in rooms)
            {
                var room = new GameObject(r.Item1);
                room.transform.SetParent(parent, false);

                MakeCube(r.Item1 + "_Floor", room.transform,
                    r.Item2 + Vector3.down * 0.1f,
                    new Vector3(r.Item3.x, 0.2f, r.Item3.y));

                // Low placeholder walls keep the blockout readable while still allowing
                // the generated scene to be walked through via the provided topology.
                float wallH = r.Item4;
                float wallT = 0.15f;
                MakeCube("NorthWall", room.transform,
                    r.Item2 + new Vector3(0, wallH/2f, r.Item3.y/2f),
                    new Vector3(r.Item3.x, wallH, wallT));
                MakeCube("SouthWall", room.transform,
                    r.Item2 + new Vector3(0, wallH/2f, -r.Item3.y/2f),
                    new Vector3(r.Item3.x, wallH, wallT));
                MakeCube("EastWall", room.transform,
                    r.Item2 + new Vector3(r.Item3.x/2f, wallH/2f, 0),
                    new Vector3(wallT, wallH, r.Item3.y));
                MakeCube("WestWall", room.transform,
                    r.Item2 + new Vector3(-r.Item3.x/2f, wallH/2f, 0),
                    new Vector3(wallT, wallH, r.Item3.y));

                var label = new GameObject("ROOM_LABEL_" + r.Item1);
                label.transform.SetParent(room.transform, false);
                label.transform.position = r.Item2 + Vector3.up * 2.2f;
                var tm = label.AddComponent<TextMeshPro>();
                tm.text = r.Item1.Replace("Drs", "Dr.'s ");
                tm.fontSize = 0.6f;
                tm.alignment = TextAlignmentOptions.Center;
                tm.color = Color.white;
                tm.transform.rotation = Quaternion.Euler(0,180,0);
            }

            // Walkable connector strips following the GDD topology.
            MakeCube("Corridor_Entrance_MainLab", parent, new Vector3(-13.5f, -0.1f, 0), new Vector3(13f,0.2f,2.2f));
            MakeCube("Corridor_MainLab_Bathroom", parent, new Vector3(11.5f, -0.1f, 0), new Vector3(9f,0.2f,2.2f));
            MakeCube("Corridor_MainLab_Office", parent, new Vector3(0,-0.1f,-9.5f), new Vector3(2.2f,0.2f,6f));
            MakeCube("Corridor_MainLab_Server", parent, new Vector3(0,-0.1f,8.2f), new Vector3(2.2f,0.2f,3f));
            MakeCube("Corridor_Server_Storage", parent, new Vector3(-7,-0.1f,10), new Vector3(7f,0.2f,2.2f));
            MakeCube("Corridor_MainLab_Security", parent, new Vector3(7,-0.1f,7), new Vector3(2.2f,0.2f,4.5f));
            MakeCube("Corridor_Office_Secret", parent, new Vector3(0,-0.1f,-17), new Vector3(2.2f,0.2f,7f));
        }

        private static GameObject AddDoor(string name, Vector3 pos, DoorController.UnlockCondition condition, Vector3 offset)
        {
            var go = MakeCube(name, GameObject.Find(RootName).transform, pos, new Vector3(1.2f,2.4f,0.25f));
            var dc = go.AddComponent<DoorController>();
            dc.condition = condition;
            dc.doorTransform = go.transform;
            dc.openLocalPositionOffset = offset;
            dc.openSpeed = 1.5f;
            return go;
        }

        private static void BuildDoors(Transform parent)
        {
            AddDoor("Door_Entrance_MainLab", new Vector3(-9,1.2f,0),
                DoorController.UnlockCondition.RequiresLockpick, new Vector3(0,0,1.5f));
            AddDoor("Door_MainLab_Office", new Vector3(0,1.2f,-6),
                DoorController.UnlockCondition.RequiresLockpick, new Vector3(0,0,1.5f));
            AddDoor("Door_MainLab_Bathroom", new Vector3(9,1.2f,0),
                DoorController.UnlockCondition.RequiresLockpick, new Vector3(0,0,1.5f));
            AddDoor("Door_MainLab_SecurityRoom", new Vector3(8,1.2f,6.5f),
                DoorController.UnlockCondition.RequiresMasterkey, new Vector3(1.5f,0,0));
            AddDoor("Door_MainLab_ServerRoom", new Vector3(0,1.2f,6.5f),
                DoorController.UnlockCondition.RequiresMasterkey, new Vector3(0,0,1.5f));
            AddDoor("Door_ServerRoom_Storage", new Vector3(-4.5f,1.2f,10),
                DoorController.UnlockCondition.RequiresMasterkey, new Vector3(-1.5f,0,0));

            // Task 4 completes when the player actually enters Security Room.
            var securityGate = MakeCube("SecurityRoom_Task4_Gate", parent, new Vector3(10.2f,1f,7f),
                new Vector3(1.5f,2f,2.5f));
            var gateCollider = securityGate.GetComponent<BoxCollider>();
            gateCollider.isTrigger = true;
            securityGate.AddComponent<RoomEntryTaskTrigger>();

            var wall = MakeCube("Wall_Office_SecretRoom", parent, new Vector3(0,1.2f,-16),
                new Vector3(3f,2.4f,0.25f));
            var switchGo = MakeCube("SlidingWallPaintingSwitch", parent, new Vector3(0,1.4f,-15.4f),
                new Vector3(1.4f,1f,0.08f));
            switchGo.AddComponent<XRSimpleInteractable>();
            var sw = switchGo.AddComponent<SecretWallSwitch>();
            sw.slidingWallTransform = wall.transform;
            sw.openLocalOffset = new Vector3(3f,0,0);
            sw.openSpeed = 1f;
        }

        private static void BuildInteractives(Transform parent)
        {
            // Lockpick
            var lockpick = MakeCube("Lockpick", parent, new Vector3(-18.5f,0.9f,1), new Vector3(0.55f,0.08f,0.08f));
            AddGrabPickup(lockpick, "Lockpick");

            // Color boxes
            var puzzleRoot = new GameObject("LightsPuzzle");
            puzzleRoot.transform.SetParent(parent, false);
            var puzzle = puzzleRoot.AddComponent<ColorSequencePuzzle>();
            var boxes = new List<ColorSequencePuzzle.ColorBox>();
            var colors = new[] { ColorSequencePuzzle.BoxColor.Red, ColorSequencePuzzle.BoxColor.Blue,
                                 ColorSequencePuzzle.BoxColor.Yellow, ColorSequencePuzzle.BoxColor.Green };
            for (int i=0;i<4;i++)
            {
                var b = MakeCube("ColorBox_" + colors[i], puzzleRoot.transform,
                    new Vector3(-3 + i*2,1,-2), new Vector3(1,1,1));
                b.GetComponent<Renderer>().sharedMaterial = MakeMaterial(colors[i].ToString());
                var si = b.AddComponent<XRSimpleInteractable>();
                boxes.Add(new ColorSequencePuzzle.ColorBox { color=colors[i], interactable=si });
            }
            var lightsOff = new GameObject("LightsOffRoot");
            lightsOff.transform.SetParent(puzzleRoot.transform, false);
            var lightsOn = new GameObject("LightsOnRoot");
            lightsOn.transform.SetParent(puzzleRoot.transform, false);
            lightsOn.SetActive(false);
            puzzle.lightsOffRoot = lightsOff;
            puzzle.lightsOnRoot = lightsOn;
            SetColorBoxes(puzzle, boxes);

            // Hint
            var hint = MakeCube("SequenceHintPanel", parent, new Vector3(0,1.6f,-2.3f), new Vector3(2.8f,1f,0.08f));
            AddWorldText(hint.transform, "BLUE  →  GREEN  →  YELLOW  →  RED", 0.18f);

            // Watch
            var watch = MakeSphere("StoppedWatch_14_23", parent, new Vector3(2,0.95f,3), 0.25f);
            AddWorldText(watch.transform, "14:23", 0.22f, new Vector3(0,0.35f,0));

            // Masterkey (revealed at Task 4)
            var masterkey = MakeCube("Masterkey", parent, new Vector3(2.2f,0.95f,3.1f), new Vector3(0.5f,0.08f,0.08f));
            AddGrabPickup(masterkey, "Masterkey");
            masterkey.SetActive(false);
            var reveal = parent.gameObject.AddComponent<MasterkeyRevealController>();
            reveal.masterkeyObject = masterkey;

            // Server panel
            var server = MakeCube("ServerCodePanel", parent, new Vector3(-2.5f,1.2f,12.5f), new Vector3(2.2f,1.4f,0.15f));
            var scp = server.AddComponent<ServerCodePanel>();
            AddWorldText(server.transform, "SERVER POWER\nKEYPAD: 1 4 2 3", 0.18f, new Vector3(0,0,0.12f));

            // 3D keypad buttons
            for (int i=0;i<10;i++)
            {
                var button = MakeCube("Key_" + i, server.transform,
                    new Vector3(-0.8f + (i%5)*0.4f, -0.3f + (i/5)*0.5f, -0.2f),
                    new Vector3(0.3f,0.3f,0.15f));
                var si = button.AddComponent<XRSimpleInteractable>();
                int digit=i;
                si.selectEntered.AddListener(_ => scp.SubmitDigit(digit));
            }

            // CCTV
            var cctv = MakeCube("CCTV_MonitorWall", parent, new Vector3(14,1.5f,12.8f), new Vector3(5f,2.4f,0.15f));
            var cctvPuzzle = cctv.AddComponent<CCTVFootagePuzzle>();
            var missing = new GameObject("MissingFootageDisplay");
            missing.transform.SetParent(cctv.transform, false);
            AddWorldText(missing.transform, "CCTV: 1.5 HOUR GAP", 0.2f, new Vector3(0,0,0.2f));
            missing.SetActive(false);
            cctvPuzzle.missingWindowDisplay = missing;
            cctv.AddComponent<XRSimpleInteractable>();

            // Wrench
            var wrench = MakeCube("Wrench", parent, new Vector3(-13,0.9f,9), new Vector3(0.9f,0.12f,0.12f));
            AddGrabPickup(wrench, "Wrench");

            // Drawer
            var drawer = MakeCube("StuckDrawer", parent, new Vector3(-2,0.7f,-10.5f), new Vector3(1.8f,0.7f,0.8f));
            var dc = drawer.AddComponent<DrawerController>();
            drawer.AddComponent<XRSimpleInteractable>();
            var pieceA = MakeCube("KeychainPieceA", parent, new Vector3(-2,1.05f,-10.5f), new Vector3(0.3f,0.12f,0.12f));
            AddGrabPickup(pieceA, "KeychainPieceA");
            pieceA.SetActive(false);
            dc.keychainPieceAObject = pieceA;

            // Secret room keychain piece B
            var pieceB = MakeCube("KeychainPieceB", parent, new Vector3(2,0.9f,-24), new Vector3(0.3f,0.12f,0.12f));
            AddGrabPickup(pieceB, "KeychainPieceB");

            // Secret computer
            var computer = MakeCube("SecretComputer", parent, new Vector3(0,1,-25), new Vector3(2.5f,1.5f,0.4f));
            var comp = computer.AddComponent<SecretComputer>();
            computer.AddComponent<XRSimpleInteractable>();
            var docs = new GameObject("ECHO_Documents_Reveal");
            docs.transform.SetParent(parent, false);
            docs.transform.position = new Vector3(0,1.4f,-23);
            AddWorldText(docs.transform, "PROJECT ECHO\nCLASSIFIED DOCUMENTS\n[ADD YOUR FINAL ART/TEXT HERE]", 0.18f);
            docs.SetActive(false);
            comp.echoDocumentsRevealUI = docs;

            // Decision console
            var console = MakeCube("FinalDecisionConsole", parent, new Vector3(0,1.1f,-20), new Vector3(3f,1.2f,0.5f));
            var fdm = console.AddComponent<FinalDecisionManager>();
            var root = new GameObject("DecisionButtons");
            root.transform.SetParent(console.transform, false);
            fdm.decisionConsoleInteractableRoot = root;
            CreateDecisionButton(root.transform, "REPORT EVERYTHING", new Vector3(-1,0.2f,0), fdm.ChooseReport);
            CreateDecisionButton(root.transform, "PROTECT ARJUN", new Vector3(0,0.2f,0), fdm.ChooseProtect);
            CreateDecisionButton(root.transform, "EXPOSE THE TRUTH", new Vector3(1,0.2f,0), fdm.ChooseExpose);
        }

        private static void AddGrabPickup(GameObject go, string itemId)
        {
            var grab = go.AddComponent<XRGrabInteractable>();
            var item = go.AddComponent<CollectibleItem>();
            item.itemId = itemId;
            item.keepAsHeldObject = true;
        }

        private static void SetColorBoxes(ColorSequencePuzzle puzzle, List<ColorSequencePuzzle.ColorBox> boxes)
        {
            var so = new SerializedObject(puzzle);
            var prop = so.FindProperty("boxes");
            prop.arraySize = boxes.Count;
            for (int i=0;i<boxes.Count;i++)
            {
                var el = prop.GetArrayElementAtIndex(i);
                el.FindPropertyRelative("color").enumValueIndex = (int)boxes[i].color;
                el.FindPropertyRelative("interactable").objectReferenceValue = boxes[i].interactable;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateDecisionButton(Transform parent, string label, Vector3 pos, UnityEngine.Events.UnityAction action)
        {
            var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.name = "Button_" + label.Replace(" ","_");
            b.transform.SetParent(parent, false);
            b.transform.localPosition = pos;
            b.transform.localScale = new Vector3(0.9f,0.35f,0.3f);
            var si = b.AddComponent<XRSimpleInteractable>();
            si.selectEntered.AddListener(_ => action());
            AddWorldText(b.transform, label, 0.12f, new Vector3(0,0.2f,0));
        }

        private static void BuildUI(Transform parent)
        {
            var canvasGo = new GameObject("GameUI");
            canvasGo.transform.SetParent(parent, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasGo.transform.position = new Vector3(0,2.2f,2.5f);
            canvasGo.transform.localScale = Vector3.one * 0.0025f;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;
            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("InstructionPanel");
            panel.transform.SetParent(canvasGo.transform, false);
            var image = panel.AddComponent<Image>();
            image.rectTransform.sizeDelta = new Vector2(700,120);
            image.rectTransform.anchoredPosition = new Vector2(0,-50);

            var textGo = new GameObject("InstructionText");
            textGo.transform.SetParent(panel.transform, false);
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.rectTransform.sizeDelta = new Vector2(660,100);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 30;
            tmp.text = "Find a way to open the doors.";

            var ui = canvasGo.AddComponent<InstructionUI>();
            ui.instructionText = tmp;

            // Clue popup
            var popup = new GameObject("CluePopup");
            popup.transform.SetParent(canvasGo.transform, false);
            var popImg = popup.AddComponent<Image>();
            popImg.rectTransform.sizeDelta = new Vector2(600,180);
            popImg.rectTransform.anchoredPosition = new Vector2(0,100);

            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(popup.transform, false);
            var title = titleGo.AddComponent<TextMeshProUGUI>();
            title.rectTransform.sizeDelta = new Vector2(560,50);
            title.rectTransform.anchoredPosition = new Vector2(0,45);
            title.fontSize = 28;
            title.alignment = TextAlignmentOptions.Center;

            var bodyGo = new GameObject("Body");
            bodyGo.transform.SetParent(popup.transform, false);
            var body = bodyGo.AddComponent<TextMeshProUGUI>();
            body.rectTransform.sizeDelta = new Vector2(560,100);
            body.rectTransform.anchoredPosition = new Vector2(0,-20);
            body.fontSize = 22;
            body.alignment = TextAlignmentOptions.Center;

            var clueUI = canvasGo.AddComponent<ClueJournalUI>();
            clueUI.popupRoot = popup;
            clueUI.popupTitleText = title;
            clueUI.popupBodyText = body;
            popup.SetActive(false);
        }

        private static void BuildEndingObjects(Transform parent)
        {
            var console = GameObject.Find("FinalDecisionConsole");
            if (console == null) return;
            var fdm = console.GetComponent<FinalDecisionManager>();

            var report = new GameObject("Ending_Report_CaseClosed");
            report.transform.SetParent(parent, false);
            report.transform.position = new Vector3(0,2,-18);
            AddWorldText(report.transform, "CASE CLOSED\nAuthorities remain suspicious.", 0.25f);
            report.SetActive(false);

            var protect = new GameObject("Ending_Protect_TruthProtected");
            protect.transform.SetParent(parent, false);
            protect.transform.position = new Vector3(0,2,-18);
            AddWorldText(protect.transform, "TRUTH PROTECTED\nDetective taken into custody.", 0.25f);
            protect.SetActive(false);

            var expose = new GameObject("Ending_Expose_TruthRevealed");
            expose.transform.SetParent(parent, false);
            expose.transform.position = new Vector3(0,2,-18);
            AddWorldText(expose.transform, "TRUTH REVEALED\nYou escaped with the truth.", 0.25f);
            expose.SetActive(false);

            fdm.reportEndingSequence = report;
            fdm.protectEndingSequence = protect;
            fdm.exposeEndingSequence = expose;
        }

        private static TextMeshPro AddWorldText(Transform parent, string text, float size, Vector3 localPos = default)
        {
            var go = new GameObject("WorldText");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            var tm = go.AddComponent<TextMeshPro>();
            tm.text = text;
            tm.fontSize = size;
            tm.alignment = TextAlignmentOptions.Center;
            tm.color = Color.white;
            return tm;
        }

        private static Material MakeMaterial(string key)
        {
            string path = "Assets/TruthWithTrenchcoat/GeneratedMaterials/" + key + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var mat = new Material(shader);
            switch (key)
            {
                case "Blue": mat.color = Color.blue; break;
                case "Green": mat.color = Color.green; break;
                case "Yellow": mat.color = Color.yellow; break;
                case "Red": mat.color = Color.red; break;
                default: mat.color = Color.gray; break;
            }
            System.IO.Directory.CreateDirectory("Assets/TruthWithTrenchcoat/GeneratedMaterials");
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }
    }
#endif
}
