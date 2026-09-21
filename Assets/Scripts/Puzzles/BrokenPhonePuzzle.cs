using UnityEngine;
using UnityEngine.UI;

public class BrokenPhonePuzzle : Interactable
{
    public GameObject phoneModel;
    public GameObject dataChip;
    public GameObject crackOverlay;
    public Slider rotationSlider;
    public float totalRotationNeeded = 360f;
    public float requiredSpeed = 50f;
    public string password = "DATA";

    private float currentRotation = 0f;
    private float lastMouseX;
    private bool isRotating = false;
    private bool solved = false;
    private int charactersFound = 0;

    void Start()
    {
        interactPrompt = "Press E to inspect phone";
        objectName = "Broken Phone";
        if (crackOverlay != null) crackOverlay.SetActive(false);
        if (dataChip != null) dataChip.SetActive(false);
    }

    public override void OnInteract()
    {
        if (solved) return;
        isRotating = true;
        lastMouseX = Input.mousePosition.x;
        if (crackOverlay != null) crackOverlay.SetActive(true);
    }

    void Update()
    {
        if (!isRotating || solved) return;

        float deltaX = Input.mousePosition.x - lastMouseX;
        lastMouseX = Input.mousePosition.x;

        currentRotation += Mathf.Abs(deltaX);
        phoneModel.transform.Rotate(0, 0, deltaX * 0.5f);

        float progress = currentRotation / totalRotationNeeded;

        if (progress > 0.25f && charactersFound < 1)
        {
            charactersFound++;
            Debug.Log("Found character: D");
        }
        if (progress > 0.5f && charactersFound < 2)
        {
            charactersFound++;
            Debug.Log("Found character: A");
        }
        if (progress > 0.75f && charactersFound < 3)
        {
            charactersFound++;
            Debug.Log("Found character: T");
        }
        if (progress >= 1f && charactersFound < 4)
        {
            charactersFound++;
            Debug.Log("Found character: A");
            Solved();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isRotating = false;
            if (crackOverlay != null) crackOverlay.SetActive(false);
        }
    }

    void Solved()
    {
        solved = true;
        isRotating = false;
        Debug.Log("Phone puzzle solved! Password: DATA");
        if (dataChip != null) dataChip.SetActive(true);
        if (crackOverlay != null) crackOverlay.SetActive(false);
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved("Phone");
    }
}
