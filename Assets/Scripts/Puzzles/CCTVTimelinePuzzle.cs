using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CCTVTimelinePuzzle : Interactable
{
    public GameObject timelineUI;
    public TextMeshProUGUI[] slotTexts;
    public Button[] eventButtons;
    public string[] correctOrder = {
        "10:30 - Dr. Arjun enters Lab",
        "11:15 - Power outage in Wing B",
        "11:20 - Unknown figure enters",
        "11:30 - Dr. Arjun leaves for Corridor",
        "11:45 - Alarm triggers briefly"
    };
    public GameObject[] videoClips;

    private string[] placedEvents;
    private int nextSlot = 0;
    private bool solved = false;

    void Start()
    {
        interactPrompt = "Press E to access CCTV";
        objectName = "CCTV System";
        if (timelineUI != null) timelineUI.SetActive(false);
        placedEvents = new string[correctOrder.Length];
        if (videoClips != null)
            foreach (var clip in videoClips) clip.SetActive(false);

        for (int i = 0; i < eventButtons.Length; i++)
        {
            int idx = i;
            eventButtons[i].onClick.AddListener(() => PlaceEvent(idx));
        }
    }

    public override void OnInteract()
    {
        if (solved) return;
        timelineUI.SetActive(true);
        nextSlot = 0;
        placedEvents = new string[correctOrder.Length];
        Debug.Log("Arrange the CCTV events in correct chronological order.");
        UpdateSlots();
    }

    public void PlaceEvent(int eventIndex)
    {
        if (nextSlot >= correctOrder.Length || solved) return;

        placedEvents[nextSlot] = correctOrder[eventIndex];
        eventButtons[eventIndex].interactable = false;
        nextSlot++;
        UpdateSlots();

        if (nextSlot >= correctOrder.Length)
            CheckOrder();
    }

    void UpdateSlots()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            if (i < placedEvents.Length && placedEvents[i] != null)
                slotTexts[i].text = placedEvents[i];
            else
                slotTexts[i].text = "Empty";
        }
    }

    void CheckOrder()
    {
        bool correct = true;
        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (placedEvents[i] != correctOrder[i])
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            Solved();
        }
        else
        {
            Debug.Log("Timeline incorrect! Try again.");
            foreach (var btn in eventButtons) btn.interactable = true;
            nextSlot = 0;
            placedEvents = new string[correctOrder.Length];
            UpdateSlots();
        }
    }

    void Solved()
    {
        solved = true;
        timelineUI.SetActive(false);
        if (videoClips != null)
            foreach (var clip in videoClips) clip.SetActive(true);
        Debug.Log("CCTV Timeline reconstructed!真相revealed.");
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuzzleSolved("CCTV");
    }
}
