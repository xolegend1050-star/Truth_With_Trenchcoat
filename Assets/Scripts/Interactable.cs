using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactPrompt = "Press E to interact";
    public string objectName = "Object";

    public virtual void OnInteract()
    {
        Debug.Log($"Interacted with {objectName}");
    }

    public virtual void OnGrab()
    {
        Debug.Log($"Grabbing {objectName}");
    }

    public virtual void OnRotate()
    {
        Debug.Log($"Rotating {objectName}");
    }

    public virtual void OnZoom()
    {
        Debug.Log($"Zooming into {objectName}");
    }
}
