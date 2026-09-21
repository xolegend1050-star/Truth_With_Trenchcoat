using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioReconstructionPuzzle : Interactable
{
    public GameObject audioUI;
    public Slider[] frequencySliders;
    public TextMeshProUGUI[] sliderLabels;
    public Button playButton;
    public Button submitButton;
    public AudioSource audioSource;
    public AudioClip[] audioSegments;
    public float[] targetFrequencies = { 440f, 880f, 660f, 1100f, 220f };
    public float tolerance = 50f;

    private bool solved = false;
    private bool isPlaying = false;

    void Start()
    {
        interactPrompt = "Press E to access audio recorder";
        objectName = "Audio Recorder";
        if (audioUI != null) audioUI.SetActive(false);

        if (playButton != null)
            playButton.onClick.AddListener(PlayAudio);
        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitFrequencies);

        string[] labels = { "Low", "Mid-Low", "Mid", "Mid-High", "High" };
        for (int i = 0; i < frequencySliders.Length && i < labels.Length; i++)
        {
            frequencySliders[i].minValue = 100f;
            frequencySliders[i].maxValue = 1500f;
            frequencySliders[i].value = 500f;
            if (sliderLabels != null && i < sliderLabels.Length)
                sliderLabels[i].text = labels[i];
        }
    }

    public override void OnInteract()
    {
        if (solved) return;
        audioUI.SetActive(true);
        Debug.Log("Reconstruct the audio by matching frequency sliders.");
    }

    public void PlayAudio()
    {
        if (audioSource != null && audioSegments.Length > 0)
        {
            int segmentIndex = Random.Range(0, audioSegments.Length);
            audioSource.clip = audioSegments[segmentIndex];
            audioSource.Play();
            isPlaying = true;
        }
    }

    public void SubmitFrequencies()
    {
        bool correct = true;
        for (int i = 0; i < frequencySliders.Length && i < targetFrequencies.Length; i++)
        {
            if (Mathf.Abs(frequencySliders[i].value - targetFrequencies[i]) > tolerance)
            {
                correct = false;
                Debug.Log($"Slider {i + 1} incorrect. Current: {frequencySliders[i].value:F0}, Target: ~{targetFrequencies[i]:F0}");
                break;
            }
        }

        if (correct)
        {
            Solved();
        }
        else
        {
            Debug.Log("Audio reconstruction failed. Try adjusting frequencies.");
        }
    }

    void Solved()
    {
        solved = true;
        audioUI.SetActive(false);
        Debug.Log("Audio reconstructed! Reveals: 'They know about Project Echo'");
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved("Audio");
    }
}
