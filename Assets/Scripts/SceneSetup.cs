using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    [Header("Assign Materials (or leave empty for defaults)")]
    public Material wallMat;
    public Material floorMat;
    public Material metalMat;
    public Material screenMat;
    public Material bloodMat;
    public Material ceilingMat;

    void Awake()
    {
        // Create default materials if none assigned
        if (wallMat == null) wallMat = CreateMat("Wall", new Color(0.24f, 0.24f, 0.25f));
        if (floorMat == null) floorMat = CreateMat("Floor", new Color(0.16f, 0.16f, 0.18f));
        if (metalMat == null) metalMat = CreateMat("Metal", new Color(0.47f, 0.47f, 0.49f));
        if (screenMat == null) screenMat = CreateMat("Screen", new Color(0f, 0.78f, 1f), true);
        if (bloodMat == null) bloodMat = CreateMat("Blood", new Color(0.47f, 0.04f, 0.04f));
        if (ceilingMat == null) ceilingMat = CreateMat("Ceiling", new Color(0.2f, 0.2f, 0.22f));

        // Build facility
        FacilityBuilder builder = gameObject.AddComponent<FacilityBuilder>();
        builder.wallMaterial = wallMat;
        builder.floorMaterial = floorMat;
        builder.metalMaterial = metalMat;
        builder.screenGlowMaterial = screenMat;
        builder.bloodMaterial = bloodMat;
        builder.ceilingMaterial = ceilingMat;

        // Setup player
        SetupPlayer();

        // Setup game manager
        SetupGameManager();

        // Setup clues
        SetupClues();

        // Setup puzzles
        SetupPuzzles();

        // Post-processing mood
        SetupPostProcessing();
    }

    void SetupPlayer()
    {
        // Create player capsule
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(11, 1f, 2);
        player.transform.localScale = new Vector3(0.6f, 1f, 0.6f);

        // Remove default collider, add CharacterController
        Destroy(player.GetComponent<CapsuleCollider>());
        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.3f;
        cc.center = new Vector3(0, 0.9f, 0);

        // Add FPS Controller
        FPSController fps = player.AddComponent<FPSController>();

        // Create camera
        GameObject camObj = new GameObject("PlayerCamera");
        camObj.transform.SetParent(player.transform);
        camObj.transform.localPosition = new Vector3(0, 1.6f, 0);
        Camera cam = camObj.AddComponent<Camera>();
        cam.nearClipPlane = 0.1f;
        cam.fieldOfView = 70f;
        camObj.AddComponent<AudioListener>();
        fps.playerCamera = cam;

        // Remove renderer on player (invisible capsule)
        Renderer rend = player.GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;
    }

    void SetupGameManager()
    {
        GameObject gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
    }

    void SetupClues()
    {
        // Main Lab clues
        CreateClue("BloodSample", new Vector3(9, 0.5f, 27), "Blood Sample", "Type O-negative - matches Dr. Chen");
        CreateClue("SecurityLog", new Vector3(21, 1.5f, 7), "Security Log", "Entry log shows Director Voss at 2:47 AM");
        CreateClue("TornNote", new Vector3(4, 1f, 7), "Torn Note", "'...the experiment must not become public...'");
        CreateClue("HiddenServer", new Vector3(20, 1f, 21), "Server Data", "Encrypted files showing illegal human testing");
        CreateClue("Fingerprint", new Vector3(11, 1f, 28), "Fingerprint", "Partial prints match Agent Reed");
        CreateClue("CameraFootage", new Vector3(18, 1.5f, 7), "Camera Footage", "Shows Agent Reed entering lab at 2:55 AM");
        CreateClue("VictimJournal", new Vector3(4, 1f, 6), "Victim's Journal", "'Voss threatened me. Chen is scared. Reed watches.'");
        CreateClue("ChemicalResidue", new Vector3(1, 0.5f, 15), "Chemical Residue", "Sedative found in ventilation - from storage");
    }

    void SetupPuzzles()
    {
        CreatePuzzle("LaptopPassword", new Vector3(11, 1f, 28), "Laptop Password", "Find the password on the whiteboard");
        CreatePuzzle("MessageDecrypt", new Vector3(18, 1f, 32), "Message Decryption", "Decrypt the encoded message");
        CreatePuzzle("DrawerLock", new Vector3(12, 0.8f, 28), "Drawer Lock", "Code: 3-1-7");
        CreatePuzzle("PowerPanel", new Vector3(14, 1.5f, 19), "Power Panel", "Match the wires correctly");
        CreatePuzzle("EvidenceBoard", new Vector3(3, 1.8f, 32.7f), "Evidence Board", "Assemble the timeline");
        CreatePuzzle("FinalDecision", new Vector3(11, 1f, 2), "Final Decision", "Accuse the killer");
    }

    void CreateClue(string name, Vector3 pos, string clueName, string desc)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "Clue_" + name;
        obj.transform.position = pos;
        obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        CluePickup clue = obj.AddComponent<CluePickup>();
        clue.clueName = clueName;
        clue.clueDescription = desc;
    }

    void CreatePuzzle(string name, Vector3 pos, string puzzleName, string hint)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "Puzzle_" + name;
        obj.transform.position = pos;
        obj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        PuzzleInteract puzzle = obj.AddComponent<PuzzleInteract>();
        puzzle.puzzleName = puzzleName;
        puzzle.hint = hint;
    }

    void SetupPostProcessing()
    {
        // Add a global directional light (dimmed for mood)
        GameObject dirLight = new GameObject("DirectionalLight");
        Light dl = dirLight.AddComponent<Light>();
        dl.type = LightType.Directional;
        dl.color = new Color(0.8f, 0.85f, 1f);
        dl.intensity = 0.3f;
        dirLight.transform.rotation = Quaternion.Euler(50, -30, 0);
    }

    Material CreateMat(string name, Color color, bool emission = false)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.name = name;
        mat.color = color;
        if (emission)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 2f);
        }
        return mat;
    }
}
