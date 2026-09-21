using UnityEngine;
using UnityEditor;

namespace TruthWithTrenchcoat.EditorTools
{
    /// <summary>
    /// Dark sci-fi research facility material library.
    /// Creates URP Lit materials with proper colors, metallic, smoothness, and emission.
    /// All materials are saved to Assets/TruthWithTrenchcoat/GeneratedMaterials/
    /// </summary>
    public static class MaterialLibrary
    {
        private const string MatFolder = "Assets/TruthWithTrenchcoat/GeneratedMaterials";

        // --- Architecture ---
        public static Material WallConcrete;
        public static Material FloorDark;
        public static Material CeilingPanel;
        public static Material FloorTile;
        public static Material WallPanel;
        public static Material CorridorWall;

        // --- Metals ---
        public static Material DarkMetal;
        public static Material BrushedSteel;
        public static Material DarkSteel;
        public static Material RustMetal;

        // --- Glass / Screens ---
        public static Material Glass;
        public static Material ScreenBlue;
        public static Material ScreenGreen;
        public static Material ScreenOff;
        public static Material MonitorGlow;

        // --- Wood ---
        public static Material DarkWood;
        public static Material LightWood;

        // --- Fabrics / Soft ---
        public static Material BlackFabric;
        public static Material GreyFabric;
        public static Material Rubber;

        // --- Accents ---
        public static Material CautionYellow;
        public static Material AlertRed;
        public static Material IndicatorGreen;
        public static Material DoorFrame;

        // --- Clue / Story ---
        public static Material Blood;
        public static Material FingerPrint;
        public static Material Chemical;

        // --- Glow / Emissive ---
        public static Material BlueGlow;
        public static Material CyanGlow;
        public static Material WhiteGlow;
        public static Material DimBlueGlow;

        // --- Bathroom ---
        public static Material WhiteTile;
        public static Material Ceramic;
        public static Material Mirror;

        // --- Secret Room ---
        public static Material DarkConcrete;
        public static Material SecretBlueWall;

        public static void Initialize()
        {
            System.IO.Directory.CreateDirectory(MatFolder);

            // Architecture
            WallConcrete = Create("WallConcrete", new Color(0.18f, 0.19f, 0.21f), 0.1f, 0.6f);
            FloorDark = Create("FloorDark", new Color(0.08f, 0.09f, 0.10f), 0.0f, 0.3f);
            CeilingPanel = Create("CeilingPanel", new Color(0.22f, 0.23f, 0.24f), 0.0f, 0.4f);
            FloorTile = Create("FloorTile", new Color(0.12f, 0.13f, 0.14f), 0.0f, 0.2f);
            WallPanel = Create("WallPanel", new Color(0.15f, 0.16f, 0.18f), 0.0f, 0.5f);
            CorridorWall = Create("CorridorWall", new Color(0.20f, 0.21f, 0.22f), 0.0f, 0.5f);

            // Metals
            DarkMetal = Create("DarkMetal", new Color(0.10f, 0.11f, 0.13f), 0.8f, 0.7f);
            BrushedSteel = Create("BrushedSteel", new Color(0.45f, 0.47f, 0.50f), 0.9f, 0.4f);
            DarkSteel = Create("DarkSteel", new Color(0.14f, 0.15f, 0.17f), 0.85f, 0.6f);
            RustMetal = Create("RustMetal", new Color(0.35f, 0.18f, 0.10f), 0.6f, 0.5f);

            // Glass / Screens
            Glass = Create("Glass", new Color(0.10f, 0.15f, 0.18f), 0.9f, 0.95f, true, new Color(0.05f, 0.12f, 0.18f) * 0.5f);
            ScreenBlue = Create("ScreenBlue", new Color(0.02f, 0.15f, 0.35f), 0.0f, 0.3f, true, new Color(0.03f, 0.25f, 0.65f) * 3f);
            ScreenGreen = Create("ScreenGreen", new Color(0.02f, 0.25f, 0.10f), 0.0f, 0.3f, true, new Color(0.03f, 0.4f, 0.15f) * 3f);
            ScreenOff = Create("ScreenOff", new Color(0.02f, 0.02f, 0.03f), 0.0f, 0.2f);
            MonitorGlow = Create("MonitorGlow", new Color(0.05f, 0.12f, 0.22f), 0.0f, 0.3f, true, new Color(0.08f, 0.2f, 0.45f) * 2f);

            // Wood
            DarkWood = Create("DarkWood", new Color(0.18f, 0.10f, 0.06f), 0.0f, 0.4f);
            LightWood = Create("LightWood", new Color(0.35f, 0.22f, 0.12f), 0.0f, 0.35f);

            // Fabrics
            BlackFabric = Create("BlackFabric", new Color(0.04f, 0.04f, 0.05f), 0.0f, 0.8f);
            GreyFabric = Create("GreyFabric", new Color(0.25f, 0.25f, 0.26f), 0.0f, 0.7f);
            Rubber = Create("Rubber", new Color(0.06f, 0.06f, 0.07f), 0.0f, 0.9f);

            // Accents
            CautionYellow = Create("CautionYellow", new Color(0.85f, 0.75f, 0.10f), 0.0f, 0.3f, true, new Color(0.9f, 0.8f, 0.1f) * 0.5f);
            AlertRed = Create("AlertRed", new Color(0.7f, 0.05f, 0.05f), 0.0f, 0.3f, true, new Color(0.8f, 0.08f, 0.08f) * 1.5f);
            IndicatorGreen = Create("IndicatorGreen", new Color(0.05f, 0.55f, 0.15f), 0.0f, 0.3f, true, new Color(0.08f, 0.7f, 0.2f) * 2f);
            DoorFrame = Create("DoorFrame", new Color(0.12f, 0.13f, 0.15f), 0.7f, 0.5f);

            // Clue / Story
            Blood = Create("Blood", new Color(0.35f, 0.02f, 0.02f), 0.0f, 0.6f);
            FingerPrint = Create("FingerPrint", new Color(0.3f, 0.3f, 0.32f), 0.0f, 0.3f);
            Chemical = Create("Chemical", new Color(0.15f, 0.45f, 0.20f), 0.0f, 0.2f, true, new Color(0.1f, 0.5f, 0.15f) * 0.8f);

            // Glow / Emissive
            BlueGlow = Create("BlueGlow", new Color(0.03f, 0.20f, 0.50f), 0.0f, 0.3f, true, new Color(0.05f, 0.35f, 0.8f) * 4f);
            CyanGlow = Create("CyanGlow", new Color(0.03f, 0.35f, 0.45f), 0.0f, 0.3f, true, new Color(0.05f, 0.5f, 0.7f) * 4f);
            WhiteGlow = Create("WhiteGlow", new Color(0.6f, 0.65f, 0.7f), 0.0f, 0.2f, true, new Color(0.8f, 0.85f, 0.9f) * 2f);
            DimBlueGlow = Create("DimBlueGlow", new Color(0.04f, 0.12f, 0.30f), 0.0f, 0.3f, true, new Color(0.06f, 0.18f, 0.45f) * 1.5f);

            // Bathroom
            WhiteTile = Create("WhiteTile", new Color(0.75f, 0.78f, 0.80f), 0.0f, 0.15f);
            Ceramic = Create("Ceramic", new Color(0.85f, 0.87f, 0.88f), 0.0f, 0.1f);
            Mirror = Create("Mirror", new Color(0.7f, 0.75f, 0.80f), 0.95f, 0.05f);

            // Secret Room
            DarkConcrete = Create("DarkConcrete", new Color(0.10f, 0.11f, 0.13f), 0.0f, 0.7f);
            SecretBlueWall = Create("SecretBlueWall", new Color(0.06f, 0.10f, 0.18f), 0.0f, 0.5f, true, new Color(0.08f, 0.15f, 0.30f) * 0.8f);
        }

        private static Material Create(string name, Color color, float metallic, float smoothness,
            bool emission = false, Color emissionColor = default)
        {
            string path = MatFolder + "/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);

            if (emission)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor);
            }

            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }
    }
}
