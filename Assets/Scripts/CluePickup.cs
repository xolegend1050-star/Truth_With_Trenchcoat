using UnityEngine;
using TMPro;

public class CluePickup : Interactable
{
    public string clueName = "Clue";
    public string clueDescription = "A mysterious clue";
    public GameObject clueModel;
    public GameObject glowEffect;
    public GameObject interactPrompt3D;

    private bool collected = false;

    void Start()
    {
        interactPrompt = $"Press E to pick up {clueName}";
        objectName = clueName;
        if (glowEffect != null) glowEffect.SetActive(true);
        if (interactPrompt3D != null) interactPrompt3D.SetActive(false);
    }

    public override void OnInteract()
    {
        if (collected) return;
        Collect();
    }

    void Collect()
    {
        collected = true;
        Debug.Log($"Collected: {clueName} - {clueDescription}");

        if (GameManager.Instance != null)
            GameManager.Instance.OnClueCollected();

        if (glowEffect != null) glowEffect.SetActive(false);
        if (interactPrompt3D != null) interactPrompt3D.SetActive(false);

        StartCoroutine(CollectAnimation());
    }

    System.Collections.IEnumerator CollectAnimation()
    {
        float duration = 0.5f;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 2f, t);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
