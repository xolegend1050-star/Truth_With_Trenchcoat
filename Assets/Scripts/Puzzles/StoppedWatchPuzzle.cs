using UnityEngine;

public class StoppedWatchPuzzle : Interactable
{
    public GameObject watchModel;
    public GameObject backCover;
    public GameObject hiddenMessage;
    public float rotationSpeed = 2f;
    public float requiredAngle = 180f;

    private float currentAngle = 0f;
    private bool isInspecting = false;
    private bool solved = false;
    private bool backOpened = false;

    void Start()
    {
        interactPrompt = "Press E to inspect watch";
        objectName = "Stopped Watch";
        if (backCover != null) backCover.SetActive(true);
        if (hiddenMessage != null) hiddenMessage.SetActive(false);
    }

    public override void OnInteract()
    {
        if (solved) return;
        isInspecting = !isInspecting;
        if (isInspecting)
            Debug.Log("Inspecting watch... Use mouse to rotate. Press R to check back.");
    }

    public override void OnRotate()
    {
        if (!isInspecting || solved) return;

        currentAngle += rotationSpeed;
        watchModel.transform.Rotate(Vector3.up, rotationSpeed);

        if (currentAngle >= requiredAngle && !backOpened)
        {
            Debug.Log("Scratch marks found on back! Press E to open.");
        }
    }

    void Update()
    {
        if (!isInspecting || solved) return;

        if (Input.GetKeyDown(KeyCode.R) && currentAngle >= requiredAngle && !backOpened)
        {
            OpenBack();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            isInspecting = false;
    }

    void OpenBack()
    {
        backOpened = true;
        if (backCover != null) backCover.SetActive(false);
        if (hiddenMessage != null) hiddenMessage.SetActive(true);
        Debug.Log("Watch back opened! Found hidden message: 'Project Echo'");
        Solved();
    }

    void Solved()
    {
        solved = true;
        isInspecting = false;
        Debug.Log("Watch puzzle solved!");
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved("Watch");
    }
}
