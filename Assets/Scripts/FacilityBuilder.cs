using UnityEngine;

public class FacilityBuilder : MonoBehaviour
{
    [Header("Materials")]
    public Material wallMaterial;
    public Material floorMaterial;
    public Material metalMaterial;
    public Material screenGlowMaterial;
    public Material bloodMaterial;
    public Material ceilingMaterial;

    [Header("Prefabs (optional - leave empty to use primitives)")]
    public GameObject doorPrefab;

    private float wallHeight = 3.5f;
    private float wallThickness = 0.2f;

    void Start()
    {
        BuildFacility();
    }

    public void BuildFacility()
    {
        BuildFloor();
        BuildExteriorWalls();
        BuildInteriorWalls();
        BuildCorridorToMainLab();
        BuildLights();
        BuildFurniture();
    }

    // ==================== FLOOR ====================
    void BuildFloor()
    {
        CreateBox("Floor", new Vector3(11, -0.1f, 16.5f), new Vector3(22, 0.2f, 33), floorMaterial);
        CreateBox("Ceiling", new Vector3(11, wallHeight + 0.1f, 16.5f), new Vector3(22, 0.2f, 33), ceilingMaterial);
    }

    // ==================== EXTERIOR WALLS ====================
    void BuildExteriorWalls()
    {
        // South-Left (Entrance flank)
        CreateWall("SouthLeft", new Vector3(4.5f, 1.75f, 0), 0, new Vector3(9, wallHeight, wallThickness));
        // South-Right (Entrance flank)
        CreateWall("SouthRight", new Vector3(17.5f, 1.75f, 0), 0, new Vector3(9, wallHeight, wallThickness));
        // North wall
        CreateWall("NorthWall", new Vector3(11, 1.75f, 33), 0, new Vector3(22, wallHeight, wallThickness));
        // West wall
        CreateWall("WestWall", new Vector3(0, 1.75f, 16.5f), 90, new Vector3(33, wallHeight, wallThickness));
        // East wall
        CreateWall("EastWall", new Vector3(22, 1.75f, 16.5f), 90, new Vector3(33, wallHeight, wallThickness));
    }

    // ==================== INTERIOR WALLS ====================

    // --- Corridor / West Wing Divider (X = 9) ---
    void BuildCorridorWestDivider()
    {
        CreateWall("CorrWest_A", new Vector3(9, 1.75f, 5.5f), 90, new Vector3(3, wallHeight, wallThickness));
        // Doorway gap Z 7-8
        CreateWall("CorrWest_B", new Vector3(9, 1.75f, 9.5f), 90, new Vector3(3, wallHeight, wallThickness));
        // Doorway gap Z 11-12
        CreateWall("CorrWest_C", new Vector3(9, 1.75f, 14), 90, new Vector3(4, wallHeight, wallThickness));
        // Doorway gap Z 16-17
        CreateWall("CorrWest_D", new Vector3(9, 1.75f, 18.5f), 90, new Vector3(3, wallHeight, wallThickness));
    }

    // --- Corridor / East Wing Divider (X = 13) ---
    void BuildCorridorEastDivider()
    {
        CreateWall("CorrEast_A", new Vector3(13, 1.75f, 5.5f), 90, new Vector3(3, wallHeight, wallThickness));
        // Doorway gap Z 7-8
        CreateWall("CorrEast_B", new Vector3(13, 1.75f, 10.5f), 90, new Vector3(5, wallHeight, wallThickness));
        // Doorway gap Z 13-14
        CreateWall("CorrEast_C", new Vector3(13, 1.75f, 15.5f), 90, new Vector3(3, wallHeight, wallThickness));
    }

    // --- Internal Room Dividers ---
    void BuildRoomDividers()
    {
        // Office / Bathroom divider
        CreateWall("OfficeBathDiv", new Vector3(4.5f, 1.75f, 10), 0, new Vector3(9, wallHeight, wallThickness));
        // Bathroom / Storage divider
        CreateWall("BathStorageDiv", new Vector3(4.5f, 1.75f, 13), 0, new Vector3(9, wallHeight, wallThickness));
        // Security / Server divider
        CreateWall("SecServerDiv", new Vector3(17.5f, 1.75f, 11), 0, new Vector3(9, wallHeight, wallThickness));
        // Server / Secret Room wall (part A)
        CreateWall("ServerSecret_A", new Vector3(15, 1.75f, 17), 0, new Vector3(4, wallHeight, wallThickness));
        // Server / Secret Room wall (part B)
        CreateWall("ServerSecret_B", new Vector3(20.5f, 1.75f, 17), 0, new Vector3(3, wallHeight, wallThickness));
    }

    // --- Corridor to Main Lab Opening (Z = 24) ---
    void BuildCorridorToMainLab()
    {
        BuildCorridorWestDivider();
        BuildCorridorEastDivider();
        BuildRoomDividers();

        // Left of opening
        CreateWall("MainLabLeft", new Vector3(4.5f, 1.75f, 24), 0, new Vector3(9, wallHeight, wallThickness));
        // Right of opening
        CreateWall("MainLabRight", new Vector3(17.5f, 1.75f, 24), 0, new Vector3(9, wallHeight, wallThickness));
    }

    // ==================== ALL WALLS ====================
    void BuildInteriorWalls()
    {
        BuildCorridorWestDivider();
        BuildCorridorEastDivider();
        BuildRoomDividers();
    }

    // ==================== LIGHTS ====================
    void BuildLights()
    {
        // Entrance
        CreatePointLight("LightEntrance", new Vector3(11, 3.2f, 2), Color.white, 1.0f, 6f);
        // Corridor
        CreatePointLight("LightCorr_A", new Vector3(11, 3.2f, 7), Color.white, 1.0f, 8f);
        CreatePointLight("LightCorr_B", new Vector3(11, 3.2f, 13), Color.white, 1.0f, 8f);
        CreatePointLight("LightCorr_C", new Vector3(11, 3.2f, 19), Color.white, 1.0f, 8f);
        // Office
        CreatePointLight("LightOffice", new Vector3(4.5f, 3.2f, 7), Color.white, 1.2f, 7f);
        // Bathroom
        CreatePointLight("LightBathroom", new Vector3(2, 3.2f, 11.5f), new Color(0.8f, 0.9f, 1f), 0.8f, 5f);
        // Storage
        CreatePointLight("LightStorage_A", new Vector3(2, 3.2f, 15), Color.white, 1.0f, 7f);
        CreatePointLight("LightStorage_B", new Vector3(6, 3.2f, 18), Color.white, 1.0f, 7f);
        // Security Room
        CreatePointLight("LightSecurity", new Vector3(18, 3.2f, 7), new Color(0.7f, 0.8f, 1f), 1.0f, 7f);
        // Server Room
        CreatePointLight("LightServer_A", new Vector3(15, 3.2f, 14), new Color(0.6f, 0.9f, 1f), 0.9f, 6f);
        CreatePointLight("LightServer_B", new Vector3(19, 3.2f, 14), new Color(0.6f, 0.9f, 1f), 0.9f, 6f);
        // Secret Room - Spotlight
        CreateSpotLight("LightSecret", new Vector3(20, 3f, 21), new Color(0f, 0.8f, 1f), 1.5f, 6f, 45f);
        // Main Lab - general
        CreatePointLight("LightMainLab_A", new Vector3(4, 3.2f, 30), Color.white, 1.0f, 9f);
        CreatePointLight("LightMainLab_B", new Vector3(18, 3.2f, 30), Color.white, 1.0f, 9f);
        // Main Lab - crime scene spotlight
        CreateSpotLight("LightCrimeScene", new Vector3(9, 3.2f, 27), Color.white, 1.8f, 5f, 35f);
    }

    // ==================== FURNITURE ====================
    void BuildFurniture()
    {
        BuildMainLabFurniture();
        BuildSecretRoomFurniture();
        BuildSecurityRoomFurniture();
        BuildServerRoomFurniture();
        BuildOfficeFurniture();
        BuildStorageFurniture();
        BuildBathroomFurniture();
        BuildEntranceFurniture();
    }

    void BuildMainLabFurniture()
    {
        // Central table
        CreateFurniture("LabTable", new Vector3(11, 0.45f, 28), new Vector3(2, 0.9f, 1.2f));
        // Laptop (Puzzle 1)
        CreateFurniture("Laptop", new Vector3(11, 0.95f, 28), new Vector3(0.4f, 0.05f, 0.3f), screenGlowMaterial);
        // Broken phone
        CreateFurniture("BrokenPhone", new Vector3(10.4f, 0.95f, 27.6f), new Vector3(0.15f, 0.03f, 0.08f), metalMaterial);
        // Blood stain
        CreateFurniture("BloodStain", new Vector3(9, 0.02f, 27), new Vector3(1.5f, 0.01f, 1f), bloodMaterial);
        // Chemical shelf
        CreateFurniture("ChemicalShelf", new Vector3(1, 1.2f, 30), new Vector3(0.6f, 2.4f, 0.4f), metalMaterial);
        // Drawer (Puzzle 3)
        CreateFurniture("Drawer", new Vector3(12, 0.5f, 28), new Vector3(0.5f, 1f, 0.4f));
        // Chair
        CreateFurniture("LabChair", new Vector3(11, 0.45f, 26.7f), new Vector3(0.5f, 0.9f, 0.5f));
        // Whiteboard
        CreateFurniture("Whiteboard", new Vector3(3, 1.5f, 32.7f), new Vector3(3, 1.5f, 0.05f));
        // Side computer
        CreateFurniture("SideComputer", new Vector3(18, 0.9f, 32), new Vector3(1.5f, 0.9f, 0.8f));
    }

    void BuildSecretRoomFurniture()
    {
        // Main computer (Puzzle 5)
        CreateFurniture("SecretComputer", new Vector3(20, 1f, 21), new Vector3(1.5f, 1f, 0.8f), screenGlowMaterial);
        // Audio recorder (Puzzle 7)
        CreateFurniture("AudioRecorder", new Vector3(18, 0.9f, 21), new Vector3(0.3f, 0.1f, 0.2f), metalMaterial);
        // Project files on shelf
        CreateFurniture("ProjectFiles", new Vector3(16, 1.1f, 21.5f), new Vector3(0.4f, 0.3f, 0.3f));
        // Power panel (Puzzle 6)
        CreateFurniture("PowerPanel", new Vector3(14, 1.3f, 19), new Vector3(0.8f, 1f, 0.1f), metalMaterial);
        // Server rack
        CreateFurniture("SecretServer", new Vector3(15, 1.2f, 20), new Vector3(0.8f, 2f, 0.6f), metalMaterial);
    }

    void BuildSecurityRoomFurniture()
    {
        // CCTV monitor bank
        CreateFurniture("CCTVMonitor", new Vector3(21, 1.4f, 7), new Vector3(2, 1f, 0.5f), screenGlowMaterial);
        // Desk + chair
        CreateFurniture("SecurityDesk", new Vector3(18, 0.45f, 8), new Vector3(2, 0.9f, 1f));
        CreateFurniture("SecurityChair", new Vector3(18, 0.45f, 9), new Vector3(0.5f, 0.9f, 0.5f));
    }

    void BuildServerRoomFurniture()
    {
        // Server racks x4
        CreateFurniture("ServerRack_1", new Vector3(15, 1.2f, 14), new Vector3(0.8f, 2f, 0.6f), metalMaterial);
        CreateFurniture("ServerRack_2", new Vector3(17, 1.2f, 14), new Vector3(0.8f, 2f, 0.6f), metalMaterial);
        CreateFurniture("ServerRack_3", new Vector3(19, 1.2f, 14), new Vector3(0.8f, 2f, 0.6f), metalMaterial);
        CreateFurniture("ServerRack_4", new Vector3(21, 1.2f, 14), new Vector3(0.8f, 2f, 0.6f), metalMaterial);
    }

    void BuildOfficeFurniture()
    {
        // Desk
        CreateFurniture("OfficeDesk", new Vector3(4, 0.45f, 6), new Vector3(1.5f, 0.9f, 0.8f));
        // Bookshelf
        CreateFurniture("Bookshelf", new Vector3(1, 1f, 8), new Vector3(0.5f, 2f, 1.5f));
    }

    void BuildStorageFurniture()
    {
        // Shelving units x3
        CreateFurniture("StorageShelf_1", new Vector3(1, 1.2f, 15), new Vector3(1f, 2f, 0.6f), metalMaterial);
        CreateFurniture("StorageShelf_2", new Vector3(3, 1.2f, 17), new Vector3(1f, 2f, 0.6f), metalMaterial);
        CreateFurniture("StorageShelf_3", new Vector3(5, 1.2f, 19), new Vector3(1f, 2f, 0.6f), metalMaterial);
        // Storage boxes
        CreateFurniture("StorageBox_1", new Vector3(7, 0.3f, 15), new Vector3(0.6f, 0.6f, 0.6f));
        CreateFurniture("StorageBox_2", new Vector3(7.5f, 0.3f, 16), new Vector3(0.5f, 0.5f, 0.5f));
    }

    void BuildBathroomFurniture()
    {
        // Sink
        CreateFurniture("Sink", new Vector3(2, 0.8f, 11), new Vector3(0.8f, 0.8f, 0.5f));
        // Mirror above sink
        CreateFurniture("Mirror", new Vector3(2, 1.8f, 10.85f), new Vector3(0.7f, 0.8f, 0.05f), screenGlowMaterial);
        // Toilet
        CreateFurniture("Toilet", new Vector3(3.5f, 0.4f, 12.5f), new Vector3(0.4f, 0.8f, 0.5f));
    }

    void BuildEntranceFurniture()
    {
        // Reception desk
        CreateFurniture("ReceptionDesk", new Vector3(11, 0.45f, 2.5f), new Vector3(2, 0.9f, 0.8f));
        // Reception computer
        CreateFurniture("ReceptionComputer", new Vector3(11, 0.95f, 2.5f), new Vector3(0.4f, 0.3f, 0.3f), screenGlowMaterial);
        // Bench
        CreateFurniture("WaitingBench", new Vector3(11, 0.3f, 3.5f), new Vector3(1.5f, 0.6f, 0.5f));
    }

    // ==================== HELPER METHODS ====================

    void CreateWall(string name, Vector3 position, float rotY, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = position;
        wall.transform.rotation = Quaternion.Euler(0, rotY, 0);
        wall.transform.localScale = scale;
        if (wallMaterial != null)
            wall.GetComponent<Renderer>().material = wallMaterial;
    }

    GameObject CreateBox(string name, Vector3 position, Vector3 scale, Material mat)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.position = position;
        box.transform.localScale = scale;
        if (mat != null)
            box.GetComponent<Renderer>().material = mat;
        return box;
    }

    void CreateFurniture(string name, Vector3 position, Vector3 scale, Material mat = null)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        if (mat != null)
            obj.GetComponent<Renderer>().material = mat;
    }

    void CreatePointLight(string name, Vector3 position, Color color, float intensity, float range)
    {
        GameObject lightObj = new GameObject(name);
        lightObj.transform.position = position;
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
    }

    void CreateSpotLight(string name, Vector3 position, Color color, float intensity, float range, float spotAngle)
    {
        GameObject lightObj = new GameObject(name);
        lightObj.transform.position = position;
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Spot;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.spotAngle = spotAngle;
        lightObj.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
