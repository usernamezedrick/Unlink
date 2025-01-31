using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform dragRectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPos;

    public string FragmentName; // Identifier for this draggable item
    public bool IsPlacedCorrectly { get; private set; } = false; // Tracks correct placement
    private bool isOverDropArea = false; // Tracks whether the item is over a drop area

    void Awake()
    {
        dragRectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = dragRectTransform.position; // Record the initial position
        canvasGroup.alpha = 0.6f; // Make the item semi-transparent
        canvasGroup.blocksRaycasts = false; // Allow raycasting to pass through
        isOverDropArea = false; // Reset the drop area flag
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.position = Input.mousePosition; // Follow the cursor
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isOverDropArea) // If not over a valid drop area, reset position
        {
            ResetPosition();
        }

        canvasGroup.alpha = 1f; // Return to full opacity
        canvasGroup.blocksRaycasts = true; // Enable raycasting
    }

    public void ResetPosition()
    {
        dragRectTransform.position = startPos; // Snap back to the original position
    }

    public void MarkAsPlaced()
    {
        IsPlacedCorrectly = true; // Mark as correctly placed
        isOverDropArea = true; // Ensure it doesn't reset when dropped correctly
    }

    public void MarkAsNotPlaced()
    {
        IsPlacedCorrectly = false; // Allow resetting if moved
        isOverDropArea = false;
    }
}
