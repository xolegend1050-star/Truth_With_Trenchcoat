using UnityEngine;

public class PuzzleInteract : MonoBehaviour, IInteractable
{
    public string puzzleName = "Puzzle";
    [TextArea(2, 5)]
    public string hint = "Interact to solve";
    public Color indicatorColor = Color.yellow;

    private bool solved = false;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        // Add glow indicator
        GameObject lightObj = new GameObject("PuzzleGlow_" + puzzleName);
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.up * 0.3f;
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = indicatorColor;
        light.intensity = 0.6f;
        light.range = 2f;
    }

    public void Interact()
    {
        if (solved) return;

        Debug.Log($"[PUZZLE] Solving: {puzzleName} — {hint}");
        solved = true;

        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved(puzzleName);

        // Change color to green when solved
        if (rend != null)
            rend.material.color = Color.green;
    }
}
