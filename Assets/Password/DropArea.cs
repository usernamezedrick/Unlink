using UnityEngine;
using UnityEngine.EventSystems;

public class DropArea : MonoBehaviour, IDropHandler
{
    public string correctFragmentName; // The correct identifier for this drop area
    public PanelManager panelManager; // Reference to the manager script
    private DraggableItem placedItem; // Tracks the item currently placed here

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();

        if (draggedItem != null)
        {
            if (draggedItem.FragmentName == correctFragmentName)
            {
                draggedItem.transform.position = transform.position; // Snap to drop area
                draggedItem.MarkAsPlaced();
                placedItem = draggedItem; // Store the placed item
                panelManager.RegisterCorrectPlacement(); // Notify manager of success
                panelManager.ShowFeedback("Correct!"); // Display success feedback
            }
            else
            {
                draggedItem.MarkAsNotPlaced(); // Mark as not correctly placed
                draggedItem.ResetPosition(); // Reset to the original position
                panelManager.ShowFeedback("Try Again!"); // Display failure feedback
            }
        }
    }

    public void ResetArea()
    {
        if (placedItem != null)
        {
            placedItem.MarkAsNotPlaced();
            placedItem.ResetPosition(); // Reset the placed item's position
            placedItem = null; // Clear the reference
        }
    }
}
