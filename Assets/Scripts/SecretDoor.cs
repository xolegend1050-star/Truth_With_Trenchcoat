using UnityEngine;

public class SecretDoor : Interactable
{
    public GameObject doorLeft;
    public GameObject doorRight;
    public int cluesRequired = 5;
    public float openSpeed = 2f;
    public float slideDistance = 4f;
    public Light indicatorLight;
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;

    private bool isOpen = false;
    private bool isMoving = false;
    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private Vector3 leftTargetPos;
    private Vector3 rightTargetPos;

    void Start()
    {
        interactPrompt = $"Press E to open secret door ({cluesRequired} clues required)";
        objectName = "Secret Door";

        if (doorLeft != null) leftStartPos = doorLeft.transform.position;
        if (doorRight != null) rightStartPos = doorRight.transform.position;

        leftTargetPos = leftStartPos + Vector3.left * slideDistance;
        rightTargetPos = rightStartPos + Vector3.right * slideDistance;

        UpdateIndicator();
    }

    void Update()
    {
        UpdateIndicator();

        if (!isMoving) return;

        float step = openSpeed * Time.deltaTime;

        if (doorLeft != null)
            doorLeft.transform.position = Vector3.MoveTowards(doorLeft.transform.position, leftTargetPos, step);
        if (doorRight != null)
            doorRight.transform.position = Vector3.MoveTowards(doorRight.transform.position, rightTargetPos, step);

        bool leftDone = doorLeft == null || Vector3.Distance(doorLeft.transform.position, leftTargetPos) < 0.01f;
        bool rightDone = doorRight == null || Vector3.Distance(doorRight.transform.position, rightTargetPos) < 0.01f;

        if (leftDone && rightDone)
            isMoving = false;
    }

    void UpdateIndicator()
    {
        if (indicatorLight != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.cluesCollected >= cluesRequired)
                indicatorLight.color = unlockedColor;
            else
                indicatorLight.color = lockedColor;
        }
    }

    public override void OnInteract()
    {
        if (isOpen || isMoving) return;

        if (GameManager.Instance != null && GameManager.Instance.cluesCollected >= cluesRequired)
        {
            OpenDoor();
        }
        else
        {
            int needed = cluesRequired - (GameManager.Instance?.cluesCollected ?? 0);
            Debug.Log($"Door locked. Need {needed} more clues.");
            GameManager.Instance?.ShowMessage($"Secret door locked! Need {needed} more clues.");
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        isMoving = true;
        interactPrompt = "";
        Debug.Log("Secret door opening...");
        GameManager.Instance?.ShowMessage("Secret door opened! The truth awaits.");
    }
}
