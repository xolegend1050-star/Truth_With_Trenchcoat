using UnityEngine;

namespace TruthWithTrenchcoat.EditorTools
{
    /// <summary>
    /// Room-specific lighting per the build plan:
    /// - Main Lab: normal white, bright
    /// - Security: blue/dark
    /// - Server: blue/dark
    /// - Storage: dark, normal temperature
    /// - Bathroom: normal
    /// - Dr's Office: normal, desk-lamp emphasis
    /// - Secret Room: dim blue/grey
    /// - Entrance: clean, normal
    /// </summary>
    public static class LightingRig
    {
        // Room lighting specs: (color, intensity, range)
        private static readonly (Color color, float intensity, float range) MainLabLight =
            (new Color(1f, 0.95f, 0.88f), 1.2f, 14f);

        private static readonly (Color color, float intensity, float range) SecurityLight =
            (new Color(0.15f, 0.30f, 0.85f), 0.9f, 10f);

        private static readonly (Color color, float intensity, float range) ServerLight =
            (new Color(0.12f, 0.28f, 0.80f), 0.85f, 9f);

        private static readonly (Color color, float intensity, float range) StorageLight =
            (new Color(0.95f, 0.88f, 0.75f), 0.7f, 8f);

        private static readonly (Color color, float intensity, float range) BathroomLight =
            (new Color(0.95f, 0.92f, 0.88f), 1.0f, 7f);

        private static readonly (Color color, float intensity, float range) OfficeLight =
            (new Color(1f, 0.90f, 0.72f), 1.1f, 9f);

        private static readonly (Color color, float intensity, float range) SecretLight =
            (new Color(0.18f, 0.30f, 0.65f), 0.6f, 8f);

        private static readonly (Color color, float intensity, float range) EntranceLight =
            (new Color(1f, 0.96f, 0.90f), 1.0f, 8f);

        private static readonly (Color color, float intensity, float range) CorridorLight =
            (new Color(0.95f, 0.92f, 0.85f), 0.8f, 7f);

        public static void Build(Transform root)
        {
            var lighting = new GameObject("Lighting");
            lighting.transform.SetParent(root, false);

            // === Ambient / Directional fill ===
            var dirLight = new GameObject("Directional_Fill");
            dirLight.transform.SetParent(lighting.transform, false);
            dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var dirComp = dirLight.AddComponent<Light>();
            dirComp.type = LightType.Directional;
            dirComp.color = new Color(0.6f, 0.65f, 0.75f);
            dirComp.intensity = 0.15f;

            // === Main Lab ===
            CreateAreaLight(lighting.transform, "MainLab_Area", new Vector3(0, 3.1f, 0), MainLabLight, new Vector3(6, 1, 6));
            // Supplemental spots on work areas
            CreateSpotLight(lighting.transform, "MainLab_DeskSpot", new Vector3(0, 3f, 0),
                new Color(1f, 0.95f, 0.85f), 2.0f, 6f, 50f, Quaternion.Euler(90, 0, 0));
            CreateSpotLight(lighting.transform, "MainLab_BenchSpot", new Vector3(-3, 3f, 2),
                new Color(1f, 0.93f, 0.82f), 1.5f, 5f, 40f, Quaternion.Euler(90, 0, 0));

            // === Security Room (blue/dark) ===
            CreatePointLight(lighting.transform, "Security_Main", new Vector3(5.5f, 2.8f, 10), SecurityLight);
            CreatePointLight(lighting.transform, "Security_CCTV", new Vector3(5.5f, 2.5f, 12.5f),
                new Color(0.1f, 0.25f, 0.7f), 0.7f, 6f);
            // Monitor glow accent
            CreatePointLight(lighting.transform, "Security_MonitorGlow", new Vector3(5.5f, 2.2f, 12.8f),
                new Color(0.15f, 0.35f, 1f), 0.5f, 4f);

            // === Server Room (blue/dark) ===
            CreatePointLight(lighting.transform, "Server_Main", new Vector3(0, 2.8f, 10), ServerLight);
            CreatePointLight(lighting.transform, "Server_RackGlow_A", new Vector3(-2, 2f, 10),
                new Color(0.1f, 0.2f, 0.6f), 0.5f, 4f);
            CreatePointLight(lighting.transform, "Server_RackGlow_B", new Vector3(2, 2f, 10),
                new Color(0.1f, 0.2f, 0.6f), 0.5f, 4f);

            // === Storage (dark, warm) ===
            CreatePointLight(lighting.transform, "Storage_Main", new Vector3(-14, 2.8f, 10), StorageLight);
            CreatePointLight(lighting.transform, "Storage_Secondary", new Vector3(-14, 2.5f, 12),
                new Color(0.9f, 0.82f, 0.7f), 0.5f, 5f);

            // === Bathroom (normal, bright) ===
            CreatePointLight(lighting.transform, "Bathroom_Main", new Vector3(16, 2.8f, 0), BathroomLight);
            // Mirror accent
            CreatePointLight(lighting.transform, "Bathroom_Mirror", new Vector3(16, 2.5f, -1.5f),
                new Color(1f, 0.95f, 0.9f), 0.6f, 4f);

            // === Dr's Office (warm, desk lamp emphasis) ===
            CreatePointLight(lighting.transform, "Office_Main", new Vector3(0, 2.8f, -12), OfficeLight);
            // Desk lamp - the key practical light
            CreateSpotLight(lighting.transform, "Office_DeskLamp", new Vector3(3.1f, 1.6f, -12.2f),
                new Color(1f, 0.88f, 0.65f), 3.0f, 4f, 55f, Quaternion.Euler(60, 0, 0));
            // Warm ambient
            CreatePointLight(lighting.transform, "Office_Warm", new Vector3(0, 2.5f, -12),
                new Color(1f, 0.85f, 0.65f), 0.4f, 5f);

            // === Secret Room (dim blue/grey) ===
            CreatePointLight(lighting.transform, "Secret_Main", new Vector3(0, 2.8f, -22), SecretLight);
            // Computer glow
            CreateSpotLight(lighting.transform, "Secret_ComputerGlow", new Vector3(0, 2.5f, -25),
                new Color(0.15f, 0.35f, 0.75f), 1.5f, 5f, 50f, Quaternion.Euler(70, 0, 0));
            // Eerie blue accent
            CreatePointLight(lighting.transform, "Secret_BlueAccent", new Vector3(2, 2f, -22),
                new Color(0.1f, 0.2f, 0.55f), 0.4f, 6f);

            // === Entrance (clean, normal) ===
            CreatePointLight(lighting.transform, "Entrance_Main", new Vector3(-20, 2.8f, 0), EntranceLight);
            // Door light
            CreatePointLight(lighting.transform, "Entrance_Door", new Vector3(-9, 2.5f, 0),
                new Color(1f, 0.95f, 0.88f), 0.8f, 5f);

            // === Corridors ===
            CreatePointLight(lighting.transform, "Corridor_A", new Vector3(-10, 2.8f, 0), CorridorLight);
            CreatePointLight(lighting.transform, "Corridor_B", new Vector3(5, 2.8f, 0), CorridorLight);
            CreatePointLight(lighting.transform, "Corridor_C", new Vector3(0, 2.8f, 5), CorridorLight);
            CreatePointLight(lighting.transform, "Corridor_D", new Vector3(0, 2.8f, -8), CorridorLight);
        }

        private static void CreatePointLight(Transform parent, string name, Vector3 pos,
            (Color color, float intensity, float range) spec)
        {
            CreatePointLight(parent, name, pos, spec.color, spec.intensity, spec.range);
        }

        private static void CreatePointLight(Transform parent, string name, Vector3 pos,
            Color color, float intensity, float range)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.Soft;
        }

        private static void CreateSpotLight(Transform parent, string name, Vector3 pos,
            Color color, float intensity, float range, float spotAngle, Quaternion rotation)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.rotation = rotation;
            var light = go.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.spotAngle = spotAngle;
            light.shadows = LightShadows.Soft;
        }

        private static void CreateAreaLight(Transform parent, string name, Vector3 pos,
            (Color color, float intensity, float range) spec, Vector3 scale)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            var light = go.AddComponent<Light>();
            light.type = LightType.Rectangle;
            light.color = spec.color;
            light.intensity = spec.intensity;
            light.range = spec.range;
        }
    }
}
