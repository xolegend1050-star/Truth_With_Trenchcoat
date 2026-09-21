using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TruthWithTrenchcoat.Core;

/// <summary>
/// Master environment builder. Generates the complete dark sci-fi research facility
/// with proper materials, detailed room dressing, lighting, and interactive wiring.
///
/// Menu: Truth With Trenchcoat > Build Detailed Environment
/// </summary>
public static class TruthEnvironmentBuilder
{
    private static Transform root;

    [MenuItem("Truth With Trenchcoat/Build Detailed Environment")]
    public static void Build()
    {
        if (GameObject.Find("TRUTH_ENVIRONMENT"))
            Object.DestroyImmediate(GameObject.Find("TRUTH_ENVIRONMENT"));

        var go = new GameObject("TRUTH_ENVIRONMENT");
        root = go.transform;

        // Initialize systems
        TruthWithTrenchcoat.EditorTools.MaterialLibrary.Initialize();
        var M = TruthWithTrenchcoat.EditorTools.MaterialLibrary;

        // Build everything
        BuildArchitecture(M);
        BuildRoomDressing(M);
        TruthWithTrenchcoat.EditorTools.LightingRig.Build(root);
        TruthWithTrenchcoat.EditorTools.InteractiveSetup.Wire(root);
        AddPreviewCamera();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorUtility.DisplayDialog(
            "Environment Built",
            "Complete environment generated with:\n" +
            "- 8 rooms with proper architecture\n" +
            "- Detailed furniture and props\n" +
            "- Room-specific lighting (blue/dark for Security/Server, warm for Office, dim for Secret)\n" +
            "- Interactive objects wired (doors, pickups, puzzles)\n" +
            "- UI panels (instructions, clue popups)\n\n" +
            "Next: Add XR Origin from XRI Starter Assets, tag it Player, press Play.\n" +
            "Replace primitives with final 3D models as art pass.",
            "OK");
        Selection.activeGameObject = go;
    }

    // ==================== ARCHITECTURE ====================

    private static void BuildArchitecture(var M)
    {
        var arch = new GameObject("Architecture");
        arch.transform.SetParent(root, false);

        // Room positions from GDD coordinates
        // Main Lab: center (0,0,0), 14x14m, 3.5m height
        Room(arch, "MainLab", new Vector3(0, 0, 0), new Vector2(14, 14), 3.5f, M.WallConcrete, M.FloorDark, M.CeilingPanel, false);

        // Entrance: (-20, 0, 0), 6x6m, 3m
        Room(arch, "Entrance", new Vector3(-20, 0, 0), new Vector2(6, 6), 3f, M.CorridorWall, M.FloorTile, M.CeilingPanel, false);

        // Dr's Office: (0, 0, -12), 7x7m, 3m
        Room(arch, "DrOffice", new Vector3(0, 0, -12), new Vector2(7, 7), 3f, M.WallPanel, M.FloorDark, M.CeilingPanel, false);

        // Secret Room: (0, 0, -22), 9x9m, 3m
        Room(arch, "SecretRoom", new Vector3(0, 0, -22), new Vector2(9, 9), 3f, M.SecretBlueWall, M.DarkConcrete, M.DarkMetal, true);

        // Bathroom: (16, 0, 0), 5x5m, 3m
        Room(arch, "Bathroom", new Vector3(16, 0, 0), new Vector2(5, 5), 3f, M.WhiteTile, M.FloorTile, M.CeilingPanel, false);

        // Security Room: (14, 0, 10), 7x7m, 3m
        Room(arch, "SecurityRoom", new Vector3(14, 0, 10), new Vector2(7, 7), 3f, M.WallPanel, M.FloorDark, M.DarkMetal, true);

        // Server Room: (0, 0, 10), 7x7m, 3m
        Room(arch, "ServerRoom", new Vector3(0, 0, 10), new Vector2(7, 7), 3f, M.WallPanel, M.FloorDark, M.DarkMetal, true);

        // Storage: (-14, 0, 10), 7x7m, 3m
        Room(arch, "Storage", new Vector3(-14, 0, 10), new Vector2(7, 7), 3f, M.WallConcrete, M.FloorDark, M.CeilingPanel, true);

        // Corridors (walkable floor strips)
        Corridor(arch, "Corr_Entrance_Lab", new Vector3(-10, -0.05f, 0), new Vector3(10, 0.1f, 2.4f), M.FloorTile);
        Corridor(arch, "Corr_Lab_Bathroom", new Vector3(12.5f, -0.05f, 0), new Vector3(7, 0.1f, 2.4f), M.FloorTile);
        Corridor(arch, "Corr_Lab_Office", new Vector3(0, -0.05f, -9), new Vector3(2.4f, 0.1f, 6), M.FloorTile);
        Corridor(arch, "Corr_Lab_Server", new Vector3(0, -0.05f, 8.2f), new Vector3(2.4f, 0.1f, 3.5f), M.FloorTile);
        Corridor(arch, "Corr_Lab_Security", new Vector3(8, -0.05f, 6.5f), new Vector3(2.4f, 0.1f, 5), M.FloorTile);
        Corridor(arch, "Corr_Server_Storage", new Vector3(-7, -0.05f, 10), new Vector3(7, 0.1f, 2.4f), M.FloorTile);
        Corridor(arch, "Corr_Office_Secret", new Vector3(0, -0.05f, -17), new Vector3(2.4f, 0.1f, 7), M.FloorDark);

        // Doors
        BuildDoors(arch, M);
    }

    private static void Room(GameObject parent, string name, Vector3 center, Vector2 size, float height,
        Material wallMat, Material floorMat, Material ceilingMat, bool hasBlueLights)
    {
        var room = new GameObject(name);
        room.transform.SetParent(parent.transform, false);
        room.transform.localPosition = center;

        // Floor
        Cube("Floor", Vector3.zero, new Vector3(size.x, 0.18f, size.y), floorMat, room.transform);
        // Ceiling
        Cube("Ceiling", new Vector3(0, height, 0), new Vector3(size.x, 0.18f, size.y), ceilingMat, room.transform);

        float w = 0.18f;
        // Walls
        Cube("Wall_N", new Vector3(0, height / 2f, size.y / 2f), new Vector3(size.x, height, w), wallMat, room.transform);
        Cube("Wall_S", new Vector3(0, height / 2f, -size.y / 2f), new Vector3(size.x, height, w), wallMat, room.transform);
        Cube("Wall_E", new Vector3(size.x / 2f, height / 2f, 0), new Vector3(w, height, size.y), wallMat, room.transform);
        Cube("Wall_W", new Vector3(-size.x / 2f, height / 2f, 0), new Vector3(w, height, size.y), wallMat, room.transform);

        // Room label
        var label = new GameObject("ROOM_LABEL_" + name);
        label.transform.SetParent(room.transform, false);
        label.transform.localPosition = new Vector3(0, height - 0.3f, 0);
        var tm = label.AddComponent<TMPro.TextMeshPro>();
        tm.text = name.Replace("DrOffice", "Dr.'s Office").Replace("MainLab", "Main Laboratory");
        tm.fontSize = 0.5f;
        tm.alignment = TMPro.TextAlignmentOptions.Center;
        tm.color = new Color(0.5f, 0.55f, 0.6f);
        tm.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private static void Corridor(GameObject parent, string name, Vector3 pos, Vector3 scale, Material mat)
    {
        Cube(name, pos, scale, mat, parent.transform);
    }

    private static void BuildDoors(GameObject arch, var M)
    {
        var doors = new GameObject("Doors");
        doors.transform.SetParent(arch.transform, false);

        Door(doors, "Door_MainEntrance", new Vector3(-6.5f, 1.25f, 0), new Vector3(0.25f, 2.5f, 2.2f), M.DoorFrame, M.DarkMetal);
        Door(doors, "Door_Office", new Vector3(0, 1.25f, -6), new Vector3(2.2f, 2.5f, 0.25f), M.DoorFrame, M.DarkMetal);
        Door(doors, "Door_Bathroom", new Vector3(9.5f, 1.25f, 0), new Vector3(0.25f, 2.5f, 2.2f), M.DoorFrame, M.DarkMetal);
        Door(doors, "Door_ServerRoom", new Vector3(0, 1.25f, 6.5f), new Vector3(2.2f, 2.5f, 0.25f), M.DoorFrame, M.DarkMetal);
        Door(doors, "Door_SecurityRoom", new Vector3(8, 1.25f, 6.5f), new Vector3(2.2f, 2.5f, 0.25f), M.DoorFrame, M.DarkMetal);
        Door(doors, "Door_Storage", new Vector3(-4.5f, 1.25f, 10), new Vector3(0.25f, 2.5f, 2.2f), M.DoorFrame, M.DarkMetal);

        // Secret sliding wall
        var wall = Cube("SecretSlidingWall", new Vector3(0, 1.5f, -16), new Vector3(2.4f, 3f, 0.2f), M.DarkSteel, doors.transform);
        // Painting on the wall (interactive switch)
        var painting = PropFactory.WallPainting(doors.transform, new Vector3(0, 1.6f, -15.9f));
        painting.name = "WallPainting";

        // Security Room entry trigger
        var gate = Cube("SecurityRoom_Task4_Gate", new Vector3(8, 1f, 7f), new Vector3(2f, 2.5f, 3f), null, doors.transform);
        gate.GetComponent<Renderer>().enabled = false;
        gate.GetComponent<BoxCollider>().isTrigger = true;
    }

    private static void Door(GameObject parent, string name, Vector3 pos, Vector3 scale, Material frameMat, Material handleMat)
    {
        Cube(name, pos, scale, frameMat, parent.transform);
        // Handle
        Vector3 handleOffset = scale.x > scale.z
            ? new Vector3(scale.x * 0.4f, 0, 0.15f)
            : new Vector3(0.15f, 0, scale.z * 0.4f);
        Cube(name + "_Handle", pos + handleOffset, new Vector3(0.06f, 0.3f, 0.06f), handleMat, parent.transform);
    }

    // ==================== ROOM DRESSING ====================

    private static void BuildRoomDressing(var M)
    {
        var dressing = new GameObject("RoomDressing");
        dressing.transform.SetParent(root, false);

        MainLabDressing(dressing, M);
        OfficeDressing(dressing, M);
        SecurityDressing(dressing, M);
        ServerDressing(dressing, M);
        StorageDressing(dressing, M);
        BathroomDressing(dressing, M);
        SecretRoomDressing(dressing, M);
        EntranceDressing(dressing, M);
    }

    private static void MainLabDressing(GameObject parent, var M)
    {
        var lab = new GameObject("MainLab_Dressing");
        lab.transform.SetParent(parent.transform, false);
        lab.transform.localPosition = new Vector3(0, 0, 0);

        // Central work table
        PropFactory.LabTable(lab.transform, new Vector3(0, 0, 0));

        // Chairs around table
        PropFactory.LabChair(lab.transform, new Vector3(-2, 0, -1.7f));
        PropFactory.LabChair(lab.transform, new Vector3(0, 0, -1.7f));
        PropFactory.LabChair(lab.transform, new Vector3(2, 0, -1.7f));
        PropFactory.LabChair(lab.transform, new Vector3(-2, 0, 1.7f));

        // Monitors on table
        PropFactory.Monitor(lab.transform, new Vector3(0, 1.35f, 0));
        PropFactory.Monitor(lab.transform, new Vector3(-1.8f, 1.3f, 0.2f), new Vector3(0.9f, 0.6f, 0.06f), "SideMonitor");

        // Laptop
        PropFactory.Laptop(lab.transform, new Vector3(2, 0.96f, 0.2f));

        // Cabinets along walls
        PropFactory.Cabinet(lab.transform, new Vector3(-5.5f, 0, 5.5f), "LabCabinet_A");
        PropFactory.Cabinet(lab.transform, new Vector3(5.5f, 0, 5.5f), "LabCabinet_B");

        // Metal shelves
        PropFactory.MetalShelf(lab.transform, new Vector3(-5.5f, 0, 3.5f), "LabShelf_A");
        PropFactory.MetalShelf(lab.transform, new Vector3(5.5f, 0, 3.5f), "LabShelf_B");

        // Chemical shelf (lab equipment)
        var chemShelf = PropFactory.MetalShelf(lab.transform, new Vector3(-5.5f, 0, -4f), "ChemicalShelf");
        // Bottles on shelf
        for (int i = 0; i < 4; i++)
        {
            var bottle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bottle.name = "ChemicalBottle_" + i;
            bottle.transform.SetParent(chemShelf.transform, false);
            bottle.transform.localPosition = new Vector3(-0.6f + i * 0.4f, 0.65f, 0);
            bottle.transform.localScale = new Vector3(0.08f, 0.15f, 0.08f);
            Color[] chemColors = { M.Chemical.color, new Color(0.6f, 0.1f, 0.1f), new Color(0.1f, 0.1f, 0.6f), M.Chemical.color };
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = chemColors[i];
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", chemColors[i] * 0.5f);
            bottle.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Whiteboard
        Cube("Whiteboard", new Vector3(0, 1.6f, -6.5f), new Vector3(4, 2, 0.08f), M.WhiteTile, lab.transform);
        Cube("WhiteboardFrame", new Vector3(0, 1.6f, -6.52f), new Vector3(4.15f, 2.15f, 0.04f), M.DarkMetal, lab.transform);
        // Marker tray
        Cube("MarkerTray", new Vector3(0, 0.55f, -6.4f), new Vector3(1.5f, 0.04f, 0.1f), M.DarkMetal, lab.transform);

        // Stopped watch on table (gameplay clue)
        PropFactory.StoppedWatch(lab.transform, new Vector3(0.5f, 0.98f, 0.5f));

        // Broken phone (gameplay clue)
        PropFactory.BrokenPhone(lab.transform, new Vector3(-2.5f, 0.96f, 0.3f));

        // Blood stain (crime scene dressing)
        PropFactory.BloodStain(lab.transform, new Vector3(3, 0.01f, 3));

        // Clue mark / evidence marker
        Cube("EvidenceMarker_1", new Vector3(3, 0.1f, 2.5f), new Vector3(0.15f, 0.15f, 0.02f), M.CautionYellow, lab.transform);
        Cube("EvidenceMarker_2", new Vector3(-1, 0.1f, 4), new Vector3(0.15f, 0.15f, 0.02f), M.CautionYellow, lab.transform);

        // Side computer desk
        var sideDesk = PropFactory.OfficeDesk(lab.transform, new Vector3(5.5f, 0, -4f), "SideDesk");
        PropFactory.Monitor(lab.transform, new Vector3(5.5f, 1.0f, -4f), new Vector3(1.1f, 0.7f, 0.06f), "SideComputer");
    }

    private static void OfficeDressing(GameObject parent, var M)
    {
        var office = new GameObject("Office_Dressing");
        office.transform.SetParent(parent.transform, false);
        office.transform.localPosition = new Vector3(0, 0, -12);

        // Main desk
        PropFactory.OfficeDesk(office.transform, new Vector3(0, 0, -0.5f));
        PropFactory.OfficeChair(office.transform, new Vector3(0, 0, -1.8f));

        // Monitor on desk
        PropFactory.Monitor(office.transform, new Vector3(0, 0.95f, -0.5f), new Vector3(1.4f, 0.85f, 0.06f), "OfficeMonitor");

        // Desk lamp (key practical light)
        PropFactory.DeskLamp(office.transform, new Vector3(1.5f, 0.78f, -0.2f));

        // Bookshelf
        PropFactory.Bookshelf(office.transform, new Vector3(-2.5f, 0, 1.5f));

        // Cabinet
        PropFactory.Cabinet(office.transform, new Vector3(2.5f, 0, 1.5f), "OfficeCabinet");

        // Stuck drawer (gameplay object)
        Cube("StuckDrawer", new Vector3(2.3f, 0.55f, -1.7f), new Vector3(1.0f, 0.5f, 0.7f), M.DarkWood, office.transform);
        Cube("DrawerHandle", new Vector3(2.3f, 0.55f, -2.06f), new Vector3(0.2f, 0.04f, 0.04f), M.BushedSteel, office.transform);

        // Personal items / papers
        Cube("Papers", new Vector3(-0.5f, 0.78f, -0.3f), new Vector3(0.3f, 0.01f, 0.4f), M.WhiteTile, office.transform);
        Cube("CoffeeMug", new Vector3(1f, 0.82f, 0.2f), new Vector3(0.08f, 0.1f, 0.08f), M.Ceramic, office.transform);

        // Wall painting (secret switch)
        PropFactory.WallPainting(office.transform, new Vector3(0, 1.6f, -3.35f));
    }

    private static void SecurityDressing(GameObject parent, var M)
    {
        var sec = new GameObject("Security_Dressing");
        sec.transform.SetParent(parent.transform, false);
        sec.transform.localPosition = new Vector3(14, 0, 10);

        // CCTV monitor wall
        PropFactory.CCTVMonitorWall(sec.transform, new Vector3(0, 1.5f, 2.8f));

        // Console desk
        PropFactory.OfficeDesk(sec.transform, new Vector3(0, 0, 0), "SecurityDesk");
        PropFactory.OfficeChair(sec.transform, new Vector3(0, 0, -1.2f));

        // Additional monitors on desk
        PropFactory.Monitor(sec.transform, new Vector3(-0.8f, 0.95f, 0), new Vector3(0.7f, 0.5f, 0.05f), "DeskMonitor_A");
        PropFactory.Monitor(sec.transform, new Vector3(0.8f, 0.95f, 0), new Vector3(0.7f, 0.5f, 0.05f), "DeskMonitor_B");

        // Keyboard/mouse
        Cube("Keyboard", new Vector3(0, 0.78f, -0.2f), new Vector3(0.4f, 0.01f, 0.15f), M.BlackFabric, sec.transform);
        Cube("Mouse", new Vector3(0.6f, 0.78f, -0.2f), new Vector3(0.06f, 0.01f, 0.1f), M.BlackFabric, sec.transform);

        // Coffee cup
        Cube("CoffeeMug", new Vector3(-0.5f, 0.82f, 0.3f), new Vector3(0.07f, 0.09f, 0.07f), M.Ceramic, sec.transform);
    }

    private static void ServerDressing(GameObject parent, var M)
    {
        var srv = new GameObject("Server_Dressing");
        srv.transform.SetParent(parent.transform, false);
        srv.transform.localPosition = new Vector3(0, 0, 10);

        // Server racks
        for (int i = -2; i <= 2; i++)
            PropFactory.ServerRack(srv.transform, new Vector3(i * 2, 0, 0), "ServerRack_" + (i + 2));

        // Power restoration panel (gameplay object)
        PropFactory.PowerPanel(srv.transform, new Vector3(3, 0, 2));

        // Cable tray along floor
        Cube("CableTray_Main", new Vector3(0, 0.05f, 3), new Vector3(6, 0.08f, 0.2f), M.Rubber, srv.transform);

        // Temperature monitor
        Cube("TempMonitor", new Vector3(-3, 1.5f, 2.8f), new Vector3(0.6f, 0.4f, 0.05f), M.ScreenOff, srv.transform);
        Cube("TempDisplay", new Vector3(-3, 1.5f, 2.77f), new Vector3(0.5f, 0.3f, 0.01f), M.ScreenGreen, srv.transform);
    }

    private static void StorageDressing(GameObject parent, var M)
    {
        var stor = new GameObject("Storage_Dressing");
        stor.transform.SetParent(parent.transform, false);
        stor.transform.localPosition = new Vector3(-14, 0, 10);

        // Metal shelves
        PropFactory.MetalShelf(stor.transform, new Vector3(-2.5f, 0, -1.5f), "StorageShelf_A");
        PropFactory.MetalShelf(stor.transform, new Vector3(2.5f, 0, -1.5f), "StorageShelf_B");

        // Storage boxes
        Color[] boxColors = { new Color(0.35f, 0.25f, 0.15f), new Color(0.3f, 0.3f, 0.32f), new Color(0.25f, 0.2f, 0.15f) };
        for (int i = 0; i < 6; i++)
        {
            int row = i / 3;
            int col = i % 3;
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = "StorageBox_" + i;
            box.transform.SetParent(stor.transform, false);
            box.transform.localPosition = new Vector3(-2 + col * 1.5f, 0.35f + row * 0.7f, 1.5f);
            box.transform.localScale = new Vector3(0.8f, 0.55f, 0.6f);
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = boxColors[i % boxColors.Length];
            box.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Wrench (gameplay object - Task 7)
        PropFactory.Wrench(stor.transform, new Vector3(1, 0.5f, -1.5f));

        // Keychain half A (gameplay object)
        PropFactory.KeychainHalf(stor.transform, new Vector3(-1, 0.5f, -1.5f), "KeychainHalf_A");

        // Misc stored items
        Cube("OldMonitor", new Vector3(-2, 0.5f, 2), new Vector3(0.8f, 0.6f, 0.1f), M.ScreenOff, stor.transform);
        Cube("CardboardBox", new Vector3(2, 0.4f, 2), new Vector3(0.6f, 0.6f, 0.6f), M.LightWood, stor.transform);
    }

    private static void BathroomDressing(GameObject parent, var M)
    {
        var bath = new GameObject("Bathroom_Dressing");
        bath.transform.SetParent(parent.transform, false);
        bath.transform.localPosition = new Vector3(16, 0, 0);

        // Sink
        PropFactory.Sink(bath.transform, new Vector3(-1.5f, 0, 1.5f));

        // Mirror
        PropFactory.Mirror(bath.transform, new Vector3(-1.5f, 1.8f, 2.05f));

        // Toilet
        PropFactory.Toilet(bath.transform, new Vector3(1, 0, 1.5f));

        // Stall partitions
        PropFactory.StallPartition(bath.transform, new Vector3(0.5f, 0, -0.8f));
        PropFactory.StallPartition(bath.transform, new Vector3(1.8f, 0, -0.8f));
        PropFactory.StallPartition(bath.transform, new Vector3(1.15f, 0, -1.7f), true);

        // Towel rack
        Cube("TowelRack", new Vector3(-2.2f, 1.2f, 0), new Vector3(0.04f, 0.04f, 0.8f), M.BushedSteel, bath.transform);
        Cube("Towel", new Vector3(-2.2f, 1.1f, 0), new Vector3(0.03f, 0.2f, 0.6f), M.WhiteTile, bath.transform);
    }

    private static void SecretRoomDressing(GameObject parent, var M)
    {
        var secret = new GameObject("Secret_Dressing");
        secret.transform.SetParent(parent.transform, false);
        secret.transform.localPosition = new Vector3(0, 0, -22);

        // Central workstation
        PropFactory.OfficeDesk(secret.transform, new Vector3(0, 0, -2.5f), "SecretDesk");
        PropFactory.OfficeChair(secret.transform, new Vector3(0, 0, -3.8f));

        // Main computer (gameplay - Task 9)
        var compGo = Cube("SecretComputer", new Vector3(0, 0.82f, -2.5f), new Vector3(1.8f, 1.0f, 0.08f), M.ScreenOff, secret.transform);
        Cube("SecretComputerScreen", new Vector3(0, 0.82f, -2.54f), new Vector3(1.6f, 0.9f, 0.01f), M.ScreenBlue, secret.transform);

        // Project ECHO documents
        Cube("ECHO_Documents", new Vector3(1.5f, 0.82f, -2.3f), new Vector3(0.5f, 0.02f, 0.35f), M.WhiteTile, secret.transform);
        Cube("ECHO_File1", new Vector3(1.3f, 0.84f, -2.3f), new Vector3(0.3f, 0.01f, 0.2f), M.CyanGlow, secret.transform);
        Cube("ECHO_File2", new Vector3(1.7f, 0.84f, -2.3f), new Vector3(0.3f, 0.01f, 0.2f), M.CyanGlow, secret.transform);

        // Audio recorder
        Cube("AudioRecorder", new Vector3(-1.5f, 0.82f, -2.3f), new Vector3(0.3f, 0.12f, 0.2f), M.DarkMetal, secret.transform);
        Cube("RecorderButton", new Vector3(-1.5f, 0.89f, -2.2f), new Vector3(0.06f, 0.02f, 0.02f), M.AlertRed, secret.transform);

        // Power panel
        PropFactory.PowerPanel(secret.transform, new Vector3(3, 0, -1));

        // Second keychain half
        PropFactory.KeychainHalf(secret.transform, new Vector3(3.5f, 0.82f, -2.5f), "KeychainHalf_B");

        // Server rack
        PropFactory.ServerRack(secret.transform, new Vector3(-3, 0, 0), "SecretServerRack");

        // Eerie equipment dressing
        Cube("MysteryDevice", new Vector3(3, 0.5f, 1), new Vector3(0.8f, 1.0f, 0.6f), M.DarkMetal, secret.transform);
        Sphere("MysteryOrb", new Vector3(3, 1.1f, 1), new Vector3(0.25f, 0.25f, 0.25f), M.DimBlueGlow, secret.transform);

        // Final decision console (gameplay - Task 10)
        Cube("FinalDecisionConsole", new Vector3(0, 0.8f, 0), new Vector3(2.5f, 1.0f, 0.5f), M.DarkMetal, secret.transform);
        Cube("ConsoleScreen", new Vector3(0, 1.1f, 0.26f), new Vector3(2.2f, 0.6f, 0.01f), M.ScreenBlue, secret.transform);
    }

    private static void EntranceDressing(GameObject parent, var M)
    {
        var entrance = new GameObject("Entrance_Dressing");
        entrance.transform.SetParent(parent.transform, false);
        entrance.transform.localPosition = new Vector3(-20, 0, 0);

        // Reception desk
        PropFactory.OfficeDesk(entrance.transform, new Vector3(0, 0, 1), "ReceptionDesk");
        PropFactory.OfficeChair(entrance.transform, new Vector3(0, 0, -0.5f));

        // Reception computer
        PropFactory.Monitor(entrance.transform, new Vector3(0, 0.95f, 1), new Vector3(1.0f, 0.65f, 0.06f), "ReceptionMonitor");
        PropFactory.Laptop(entrance.transform, new Vector3(-1.2f, 0.82f, 1.2f));

        // Waiting bench
        Cube("WaitingBench", new Vector3(0, 0.3f, -1.5f), new Vector3(2f, 0.6f, 0.5f), M.DarkWood, entrance.transform);

        // Lockpick (gameplay - Task 1)
        PropFactory.Lockpick(entrance.transform, new Vector3(1.5f, 0.85f, 1));

        // Facility sign
        Cube("FacilitySign", new Vector3(0, 2.5f, -2.8f), new Vector3(3f, 0.6f, 0.05f), M.DarkMetal, entrance.transform);
        var signText = new GameObject("SignText");
        signText.transform.SetParent(entrance.transform, false);
        signText.transform.localPosition = new Vector3(0, 2.5f, -2.75f);
        var tm = signText.AddComponent<TMPro.TextMeshPro>();
        tm.text = "CLASSIFIED RESEARCH FACILITY\nAUTHORIZED PERSONNEL ONLY";
        tm.fontSize = 0.2f;
        tm.alignment = TMPro.TextAlignmentOptions.Center;
        tm.color = new Color(0.8f, 0.2f, 0.2f);
    }

    // ==================== HELPERS ====================

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

    private static void AddPreviewCamera()
    {
        var cam = new GameObject("EnvironmentPreviewCamera");
        cam.transform.SetParent(root, false);
        cam.transform.position = new Vector3(0, 20, -30);
        cam.transform.rotation = Quaternion.Euler(38, 0, 0);
        var c = cam.AddComponent<Camera>();
        c.fieldOfView = 55;
        c.nearClipPlane = 0.3f;
        c.farClipPlane = 100f;
    }
}
