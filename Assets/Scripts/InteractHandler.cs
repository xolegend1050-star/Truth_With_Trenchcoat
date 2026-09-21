using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    public static InteractHandler Instance;
    public float interactRange = 3f;
    public Camera playerCamera;
    public TMPro.TextMeshProUGUI promptText;

    private GameObject currentTarget;

    void Start()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            Interactable obj = hit.collider.GetComponent<Interactable>();
            if (obj != null)
            {
                currentTarget = obj.gameObject;
                if (promptText != null)
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = obj.interactPrompt;
                }

                if (Input.GetKeyDown(KeyCode.E))
                    obj.OnInteract();

                if (Input.GetMouseButtonDown(0))
                    obj.OnInteract();

                if (Input.GetKey(KeyCode.F))
                    obj.OnGrab();

                if (Input.GetKey(KeyCode.R))
                    obj.OnRotate();

                if (Input.GetKey(KeyCode.Z))
                    obj.OnZoom();
            }
        }
        else
        {
            currentTarget = null;
            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }
}
