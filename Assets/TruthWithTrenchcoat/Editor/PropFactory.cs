using UnityEngine;

namespace TruthWithTrenchcoat.EditorTools
{
    /// <summary>
    /// Generates detailed furniture and prop primitives for each room.
    /// Uses MaterialLibrary for consistent dark sci-fi aesthetic.
    /// All props are built from cubes/cylinders/spheres — replace with final meshes later.
    /// </summary>
    public static class PropFactory
    {
        // ==================== FURNITURE ====================

        public static GameObject LabTable(Transform parent, Vector3 pos, string name = "LabTable")
        {
            var table = Cube(name, pos, new Vector3(5.5f, 0.9f, 2.1f), MaterialLibrary.DarkMetal, parent);
            var top = Cube("TableTop", pos + Vector3.up * 0.9f, new Vector3(5.6f, 0.08f, 2.2f), MaterialLibrary.DarkWood, parent);
            // Legs
            foreach (float x in new[] { -2.5f, 2.5f })
                foreach (float z in new[] { -0.9f, 0.9f })
                    Cube("Leg", pos + new Vector3(x, 0.45f, z), new Vector3(0.1f, 0.9f, 0.1f), MaterialLibrary.DarkMetal, parent);
            return table;
        }

        public static GameObject OfficeDesk(Transform parent, Vector3 pos, string name = "OfficeDesk")
        {
            var desk = Cube(name, pos, new Vector3(3.5f, 0.75f, 1.4f), MaterialLibrary.DarkWood, parent);
            Cube("DeskTop", pos + Vector3.up * 0.75f, new Vector3(3.6f, 0.06f, 1.5f), MaterialLibrary.DarkWood, parent);
            // Drawer unit
            Cube("DrawerUnit", pos + new Vector3(1.3f, 0.35f, 0), new Vector3(0.8f, 0.7f, 1.2f), MaterialLibrary.DarkMetal, parent);
            for (int i = 0; i < 3; i++)
                Cube("DrawerFace", pos + new Vector3(1.3f, 0.15f + i * 0.22f, -0.62f), new Vector3(0.7f, 0.18f, 0.04f), MaterialLibrary.BrushedSteel, parent);
            // Legs
            foreach (float x in new[] { -1.5f, 1.5f })
                Cube("Leg", pos + new Vector3(x, 0.37f, 0.55f), new Vector3(0.08f, 0.75f, 0.08f), MaterialLibrary.DarkMetal, parent);
            return desk;
        }

        public static GameObject LabChair(Transform parent, Vector3 pos)
        {
            Cyl("ChairSeat", pos + Vector3.up * 0.55f, new Vector3(0.55f, 0.06f, 0.55f), MaterialLibrary.BlackFabric, parent);
            Cube("ChairBack", pos + new Vector3(0, 1.0f, 0.22f), new Vector3(0.55f, 0.8f, 0.06f), MaterialLibrary.BlackFabric, parent);
            Cyl("ChairStem", pos + Vector3.up * 0.28f, new Vector3(0.06f, 0.22f, 0.06f), MaterialLibrary.DarkMetal, parent);
            Cube("ChairBase", pos + new Vector3(0, 0.05f, 0), new Vector3(0.5f, 0.04f, 0.5f), MaterialLibrary.DarkMetal, parent);
            return null;
        }

        public static GameObject OfficeChair(Transform parent, Vector3 pos)
        {
            Cyl("OfficeChairSeat", pos + Vector3.up * 0.5f, new Vector3(0.6f, 0.08f, 0.6f), MaterialLibrary.BlackFabric, parent);
            Cube("OfficeChairBack", pos + new Vector3(0, 1.1f, 0.25f), new Vector3(0.6f, 1.0f, 0.08f), MaterialLibrary.BlackFabric, parent);
            Cyl("OfficeChairStem", pos + Vector3.up * 0.25f, new Vector3(0.05f, 0.2f, 0.05f), MaterialLibrary.DarkMetal, parent);
            // Wheels
            for (int i = 0; i < 5; i++)
            {
                float angle = i * 72f * Mathf.Deg2Rad;
                Cyl("Wheel", pos + new Vector3(Mathf.Cos(angle) * 0.25f, 0.04f, Mathf.Sin(angle) * 0.25f),
                    new Vector3(0.04f, 0.04f, 0.04f), MaterialLibrary.Rubber, parent);
            }
            return null;
        }

        // ==================== COMPUTER KIT ====================

        public static GameObject Monitor(Transform parent, Vector3 pos, Vector3? scale = null, string name = "Monitor")
        {
            var s = scale ?? new Vector3(1.3f, 0.85f, 0.08f);
            Cube(name, pos, s, MaterialLibrary.ScreenOff, parent);
            Cube("MonitorScreen", pos + new Vector3(0, 0, -0.04f), s * 0.85f, MaterialLibrary.ScreenBlue, parent);
            Cube("MonitorStand", pos + new Vector3(0, -s.y * 0.5f - 0.15f, 0), new Vector3(0.08f, 0.3f, 0.08f), MaterialLibrary.DarkMetal, parent);
            Cube("MonitorBase", pos + new Vector3(0, -s.y * 0.5f - 0.3f, 0), new Vector3(0.35f, 0.03f, 0.25f), MaterialLibrary.DarkMetal, parent);
            return null;
        }

        public static GameObject Laptop(Transform parent, Vector3 pos, string name = "Laptop")
        {
            Cube("LaptopBase", pos, new Vector3(0.55f, 0.02f, 0.38f), MaterialLibrary.DarkMetal, parent);
            Cube("LaptopScreen", pos + new Vector3(0, 0.22f, 0.18f), new Vector3(0.52f, 0.35f, 0.02f), MaterialLibrary.ScreenOff, parent);
            Cube("LaptopDisplay", pos + new Vector3(0, 0.22f, 0.16f), new Vector3(0.44f, 0.28f, 0.01f), MaterialLibrary.ScreenBlue, parent);
            // Keyboard area
            Cube("Keyboard", pos + new Vector3(0, 0.015f, 0.02f), new Vector3(0.44f, 0.005f, 0.22f), MaterialLibrary.BlackFabric, parent);
            return null;
        }

        public static GameObject CCTVMonitorWall(Transform parent, Vector3 pos)
        {
            var root = new GameObject("CCTV_MonitorBank");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = pos;

            // 2 rows of 5 monitors
            for (int row = 0; row < 2; row++)
                for (int col = 0; col < 5; col++)
                {
                    var mpos = new Vector3(-3f + col * 1.5f, row * 1.0f, 0);
                    Cube("CCTV_Screen_" + row + "_" + col, mpos, new Vector3(1.2f, 0.8f, 0.06f), MaterialLibrary.ScreenOff, root.transform);
                    Cube("CCTV_Glow_" + row + "_" + col, mpos + new Vector3(0, 0, -0.03f), new Vector3(1.1f, 0.7f, 0.01f), MaterialLibrary.ScreenBlue, root.transform);
                }

            // Desk/console below
            Cube("CCTV_Console", new Vector3(0, -0.8f, 0.6f), new Vector3(7f, 0.7f, 0.8f), MaterialLibrary.DarkMetal, root.transform);
            Cube("ConsoleTop", new Vector3(0, -0.45f, 0.6f), new Vector3(7.1f, 0.04f, 0.85f), MaterialLibrary.DarkSteel, root.transform);

            // Keyboard/mouse props
            Cube("Keyboard", new Vector3(0, -0.42f, 0.4f), new Vector3(0.5f, 0.01f, 0.18f), MaterialLibrary.BlackFabric, root.transform);
            Cube("Mouse", new Vector3(0.7f, -0.42f, 0.4f), new Vector3(0.08f, 0.01f, 0.12f), MaterialLibrary.BlackFabric, root.transform);

            return root;
        }

        // ==================== SERVER KIT ====================

        public static GameObject ServerRack(Transform parent, Vector3 pos, string name = "ServerRack")
        {
            Cube(name, pos + Vector3.up * 1.1f, new Vector3(0.8f, 2.2f, 0.6f), MaterialLibrary.DarkMetal, parent);

            // Server units
            for (int i = 0; i < 6; i++)
            {
                float y = 0.25f + i * 0.32f;
                Cube("ServerUnit", pos + new Vector3(0, y, -0.32f), new Vector3(0.7f, 0.22f, 0.02f), MaterialLibrary.BlackFabric, parent);
                // LEDs
                for (int j = 0; j < 4; j++)
                    Sphere("LED", pos + new Vector3(-0.25f + j * 0.15f, y + 0.08f, -0.34f), new Vector3(0.03f, 0.03f, 0.03f), MaterialLibrary.BlueGlow, parent);
            }

            // Cable tray
            Cube("CableTray", pos + new Vector3(0, 0.1f, 0.35f), new Vector3(0.6f, 0.08f, 0.15f), MaterialLibrary.Rubber, parent);

            return null;
        }

        public static GameObject PowerPanel(Transform parent, Vector3 pos)
        {
            Cube("PowerPanel", pos, new Vector3(1.2f, 1.5f, 0.12f), MaterialLibrary.DarkMetal, parent);
            Cube("PanelScreen", pos + new Vector3(0, 0.3f, -0.07f), new Vector3(0.8f, 0.5f, 0.01f), MaterialLibrary.ScreenBlue, parent);
            // Indicator lights
            for (int i = 0; i < 4; i++)
                Sphere("PowerLED_" + i, pos + new Vector3(-0.3f + i * 0.2f, -0.4f, -0.07f), new Vector3(0.04f, 0.04f, 0.04f), MaterialLibrary.IndicatorGreen, parent);
            // Cables
            for (int i = 0; i < 3; i++)
                Cube("Cable_" + i, pos + new Vector3(0.3f, -0.1f - i * 0.15f, 0.08f), new Vector3(0.03f, 0.3f, 0.03f), MaterialLibrary.Rubber, parent);

            return null;
        }

        // ==================== OFFICE KIT ====================

        public static GameObject Bookshelf(Transform parent, Vector3 pos, string name = "Bookshelf")
        {
            Cube(name, pos + Vector3.up * 1.0f, new Vector3(1.8f, 2.0f, 0.4f), MaterialLibrary.DarkWood, parent);
            // Shelves
            for (int i = 0; i < 4; i++)
                Cube("Shelf_" + i, pos + new Vector3(0, 0.2f + i * 0.5f, 0), new Vector3(1.7f, 0.04f, 0.38f), MaterialLibrary.DarkWood, parent);
            // Books (colored blocks)
            Color[] bookColors = { new Color(0.4f, 0.1f, 0.1f), new Color(0.1f, 0.3f, 0.1f), new Color(0.1f, 0.1f, 0.4f), new Color(0.3f, 0.3f, 0.1f) };
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 3; j++)
                {
                    var bookMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    bookMat.color = bookColors[(i + j) % bookColors.Length];
                    Cube("Book_" + i + "_" + j, pos + new Vector3(-0.5f + j * 0.4f, 0.25f + i * 0.5f, 0),
                        new Vector3(0.12f, 0.35f, 0.25f), bookMat, parent);
                }
            return null;
        }

        public static GameObject DeskLamp(Transform parent, Vector3 pos)
        {
            Cube("LampBase", pos, new Vector3(0.2f, 0.03f, 0.2f), MaterialLibrary.DarkMetal, parent);
            Cube("LampArm", pos + Vector3.up * 0.35f, new Vector3(0.03f, 0.7f, 0.03f), MaterialLibrary.BrushedSteel, parent);
            Cube("LampHead", pos + new Vector3(0, 0.7f, 0), new Vector3(0.25f, 0.06f, 0.18f), MaterialLibrary.DarkMetal, parent);
            Cube("LampBulb", pos + new Vector3(0, 0.67f, 0), new Vector3(0.15f, 0.02f, 0.1f), MaterialLibrary.WhiteGlow, parent);
            return null;
        }

        public static GameObject Cabinet(Transform parent, Vector3 pos, string name = "Cabinet")
        {
            Cube(name, pos + Vector3.up * 0.9f, new Vector3(1.0f, 1.8f, 0.6f), MaterialLibrary.DarkMetal, parent);
            for (int i = 0; i < 3; i++)
                Cube("CabinetDrawer_" + i, pos + new Vector3(0, 0.35f + i * 0.5f, -0.31f), new Vector3(0.85f, 0.4f, 0.04f), MaterialLibrary.BrushedSteel, parent);
            return null;
        }

        public static GameObject MetalShelf(Transform parent, Vector3 pos, string name = "MetalShelf")
        {
            Cube(name, pos + Vector3.up * 1.2f, new Vector3(2.2f, 2.4f, 0.5f), MaterialLibrary.DarkMetal, parent);
            for (int i = 0; i < 4; i++)
                Cube("Shelf_" + i, pos + new Vector3(0, 0.2f + i * 0.6f, 0), new Vector3(2.1f, 0.04f, 0.48f), MaterialLibrary.DarkMetal, parent);
            return null;
        }

        // ==================== BATHROOM KIT ====================

        public static GameObject Sink(Transform parent, Vector3 pos)
        {
            Cube("SinkBase", pos, new Vector3(0.8f, 0.8f, 0.5f), MaterialLibrary.Ceramic, parent);
            Cube("SinkBasin", pos + new Vector3(0, 0.42f, 0), new Vector3(0.6f, 0.08f, 0.35f), MaterialLibrary.WhiteTile, parent);
            Cube("SinkFaucet", pos + new Vector3(0, 0.55f, -0.15f), new Vector3(0.04f, 0.15f, 0.04f), MaterialLibrary.BrushedSteel, parent);
            Cube("SinkSpout", pos + new Vector3(0, 0.55f, 0.05f), new Vector3(0.03f, 0.03f, 0.2f), MaterialLibrary.BrushedSteel, parent);
            return null;
        }

        public static GameObject Toilet(Transform parent, Vector3 pos)
        {
            Cube("ToiletBase", pos, new Vector3(0.45f, 0.4f, 0.55f), MaterialLibrary.Ceramic, parent);
            Cube("ToiletBowl", pos + new Vector3(0, 0.4f, 0.05f), new Vector3(0.4f, 0.15f, 0.45f), MaterialLibrary.Ceramic, parent);
            Cube("ToiletSeat", pos + new Vector3(0, 0.48f, 0.05f), new Vector3(0.38f, 0.03f, 0.43f), MaterialLibrary.WhiteTile, parent);
            Cube("ToiletTank", pos + new Vector3(0, 0.65f, -0.22f), new Vector3(0.35f, 0.35f, 0.15f), MaterialLibrary.Ceramic, parent);
            return null;
        }

        public static GameObject Mirror(Transform parent, Vector3 pos)
        {
            Cube("MirrorFrame", pos, new Vector3(1.0f, 1.2f, 0.05f), MaterialLibrary.DarkMetal, parent);
            Cube("MirrorGlass", pos + new Vector3(0, 0, 0.03f), new Vector3(0.85f, 1.05f, 0.01f), MaterialLibrary.Mirror, parent);
            return null;
        }

        public static GameObject StallPartition(Transform parent, Vector3 pos, bool isDoor = false)
        {
            if (isDoor)
                Cube("StallDoor", pos, new Vector3(0.9f, 2.0f, 0.04f), MaterialLibrary.WhiteTile, parent);
            else
                Cube("StallWall", pos, new Vector3(0.04f, 2.0f, 1.8f), MaterialLibrary.WhiteTile, parent);
            return null;
        }

        // ==================== CLUE KIT ====================

        public static GameObject StoppedWatch(Transform parent, Vector3 pos)
        {
            Cyl("WatchBody", pos, new Vector3(0.12f, 0.02f, 0.12f), MaterialLibrary.BrushedSteel, parent);
            Cube("WatchFace", pos + Vector3.up * 0.025f, new Vector3(0.1f, 0.005f, 0.1f), MaterialLibrary.WhiteTile, parent);
            Cube("WatchHands", pos + Vector3.up * 0.03f, new Vector3(0.06f, 0.003f, 0.003f), MaterialLibrary.DarkMetal, parent);
            // Time text 14:23
            Cube("TimeMark1", pos + new Vector3(0, 0.03f, -0.03f), new Vector3(0.005f, 0.003f, 0.02f), MaterialLibrary.DarkMetal, parent);
            Cube("TimeMark2", pos + new Vector3(0.025f, 0.03f, 0), new Vector3(0.02f, 0.003f, 0.005f), MaterialLibrary.DarkMetal, parent);
            return null;
        }

        public static GameObject BrokenPhone(Transform parent, Vector3 pos)
        {
            Cube("PhoneBody", pos, new Vector3(0.15f, 0.02f, 0.3f), MaterialLibrary.ScreenOff, parent);
            Cube("PhoneScreen", pos + new Vector3(0, 0.015f, 0), new Vector3(0.13f, 0.003f, 0.22f), MaterialLibrary.ScreenOff, parent);
            // Crack lines
            Cube("PhoneCrack1", pos + new Vector3(0.02f, 0.02f, 0.05f), new Vector3(0.08f, 0.002f, 0.002f), MaterialLibrary.Glass, parent);
            Cube("PhoneCrack2", pos + new Vector3(-0.01f, 0.02f, -0.03f), new Vector3(0.002f, 0.002f, 0.1f), MaterialLibrary.Glass, parent);
            return null;
        }

        public static GameObject Wrench(Transform parent, Vector3 pos)
        {
            Cube("WrenchHandle", pos, new Vector3(0.4f, 0.04f, 0.04f), MaterialLibrary.BrushedSteel, parent);
            Cube("WrenchHead", pos + new Vector3(0.22f, 0, 0), new Vector3(0.08f, 0.08f, 0.04f), MaterialLibrary.DarkSteel, parent);
            Cube("WrenchJaw", pos + new Vector3(0.26f, 0.02f, 0), new Vector3(0.04f, 0.04f, 0.03f), MaterialLibrary.DarkSteel, parent);
            return null;
        }

        public static GameObject KeychainHalf(Transform parent, Vector3 pos, string name = "KeychainHalf")
        {
            Cube(name, pos, new Vector3(0.12f, 0.03f, 0.06f), MaterialLibrary.BrushedSteel, parent);
            Cyl("KeychainRing", pos + new Vector3(-0.07f, 0, 0), new Vector3(0.03f, 0.005f, 0.03f), MaterialLibrary.DarkMetal, parent);
            return null;
        }

        public static GameObject BloodStain(Transform parent, Vector3 pos)
        {
            Cube("BloodPool", pos, new Vector3(1.2f, 0.005f, 0.8f), MaterialLibrary.Blood, parent);
            // Splatter drops
            for (int i = 0; i < 5; i++)
            {
                float ox = Random.Range(-0.8f, 0.8f);
                float oz = Random.Range(-0.5f, 0.5f);
                float size = Random.Range(0.05f, 0.15f);
                Sphere("BloodDrop_" + i, pos + new Vector3(ox, 0.003f, oz), new Vector3(size, 0.003f, size), MaterialLibrary.Blood, parent);
            }
            return null;
        }

        public static GameObject Lockpick(Transform parent, Vector3 pos)
        {
            Cube("LockpickHandle", pos, new Vector3(0.15f, 0.02f, 0.02f), MaterialLibrary.BrushedSteel, parent);
            Cube("LockpickTip", pos + new Vector3(0.1f, 0.01f, 0), new Vector3(0.06f, 0.01f, 0.015f), MaterialLibrary.DarkSteel, parent);
            Cube("LockpickBend", pos + new Vector3(0.12f, 0.015f, 0), new Vector3(0.02f, 0.015f, 0.015f), MaterialLibrary.DarkSteel, parent);
            return null;
        }

        public static GameObject Masterkey(Transform parent, Vector3 pos)
        {
            Cube("MasterkeyHandle", pos, new Vector3(0.18f, 0.03f, 0.04f), MaterialLibrary.CautionYellow, parent);
            Cube("MasterkeyBlade", pos + new Vector3(0.12f, 0, 0), new Vector3(0.1f, 0.02f, 0.03f), MaterialLibrary.BrushedSteel, parent);
            Cube("MasterkeyTeeth", pos + new Vector3(0.18f, -0.01f, 0), new Vector3(0.03f, 0.015f, 0.025f), MaterialLibrary.BrushedSteel, parent);
            return null;
        }

        public static GameObject WallPainting(Transform parent, Vector3 pos)
        {
            Cube("PaintingFrame", pos, new Vector3(1.4f, 1.0f, 0.06f), MaterialLibrary.DarkWood, parent);
            Cube("PaintingCanvas", pos + new Vector3(0, 0, 0.035f), new Vector3(1.2f, 0.8f, 0.01f), MaterialLibrary.WallPanel, parent);
            // Abstract art pattern
            Cube("ArtLine1", pos + new Vector3(-0.2f, 0.1f, 0.045f), new Vector3(0.6f, 0.02f, 0.005f), MaterialLibrary.BlueGlow, parent);
            Cube("ArtLine2", pos + new Vector3(0.1f, -0.1f, 0.045f), new Vector3(0.4f, 0.02f, 0.005f), MaterialLibrary.CyanGlow, parent);
            return null;
        }

        // ==================== HELPER PRIMITIVES ====================

        private static GameObject Cube(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }

        private static GameObject Cyl(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }

        private static GameObject Sphere(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }
    }
}
