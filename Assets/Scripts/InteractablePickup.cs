using UnityEngine;

public class InteractablePickup : MonoBehaviour
{
    public string itemName; // Name of the item to pick up
    public GameObject CluePanel; // UI panel to show when clue is picked up
    private bool isPlayerNearby = false;
    private Player playerInput;

    public bool isModule = false; // Check if this is a module for mission tracking
    public bool isClue = false;   // Check if this is a clue for mission tracking

    // To track if the item has already contributed to mission progress
    private bool hasProgressed = false;

    private void Awake()
    {
        playerInput = new Player();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Press E to pick up " + itemName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void Update()
    {
        if (isPlayerNearby && playerInput.PlayerMain.Interact.triggered)
        {
            PickUpItem();
        }
    }

    private void PickUpItem()
    {
        Debug.Log("Picked up " + itemName);
        CluePanel.SetActive(true); // Show clue panel or any other UI element

        // Only track interaction for mission progress once
        if (!hasProgressed)
        {
            if (isModule && MissionManager.Instance != null)
            {
                MissionManager.Instance.TrackModuleInteracted(); // Track module interaction
            }

            if (isClue && MissionManager.Instance != null)
            {
                MissionManager.Instance.TrackClueInteracted(); // Track clue interaction
            }

            // Mark this item as interacted for mission purposes (prevents multiple increments)
            hasProgressed = true;
        }

        // Optional: Keep allowing other actions like opening panels, etc.
        // The object can still be interacted with even if the mission progress is not incremented
    }
}
