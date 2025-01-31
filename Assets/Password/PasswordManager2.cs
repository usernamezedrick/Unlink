using UnityEngine;
using TMPro; // Import for TextMeshPro support

public class PasswordManager2 : MonoBehaviour
{
    public GameObject panelToEnable; // Panel to enable when completed
    public GameObject panelToDisable; // Panel to disable when completed
    public DropArea[] dropAreas; // Reference to all DropArea objects
    public int requiredCorrectItems; // Number of correct items needed to unlock
    public TextMeshProUGUI feedbackText; // UI element for feedback messages

    private int correctPlacements = 0; // Counter for correct item placements

    void Start()
    {
        if (requiredCorrectItems > dropAreas.Length)
        {
            Debug.LogWarning("Required correct items exceed the number of drop areas!");
            requiredCorrectItems = dropAreas.Length; // Ensure no overcounting
        }
    }

    public void RegisterCorrectPlacement()
    {
        correctPlacements++;

        if (correctPlacements >= requiredCorrectItems)
        {
            UnlockPanels(); // Trigger panel unlock
        }
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message; // Update feedback text
        }
    }

    public void ResetGame()
    {
        correctPlacements = 0; // Reset correct placement counter

        foreach (var dropArea in dropAreas)
        {
            dropArea.ResetArea();
        }

        if (panelToEnable != null)
            panelToEnable.SetActive(false);

        if (panelToDisable != null)
            panelToDisable.SetActive(true);

        ShowFeedback(""); // Clear feedback text
    }

    private void UnlockPanels()
    {
        if (panelToEnable != null)
            panelToEnable.SetActive(true);

        if (panelToDisable != null)
            panelToDisable.SetActive(false);
    }
}
