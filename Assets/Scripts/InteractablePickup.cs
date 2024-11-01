using UnityEngine;

public class InteractablePickup : MonoBehaviour
{
    public string itemName; // Name of the item to pick up
    private bool isPlayerNearby = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Press E to pick up " + itemName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exits the trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void Update()
    {
        // Check if the player presses E and is near the item
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickUpItem();
        }
    }

    private void PickUpItem()
    {
        Debug.Log("Picked up " + itemName);
        // Add logic here for adding the item to inventory or other effects
        Destroy(gameObject); // Remove item from the scene
    }
}
