using TMPro;
using UnityEngine;

public class UpdateChallenge : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;  // Reference to the TextMeshProUGUI that will display the score
    public TextMeshProUGUI highestScoreText;  // Reference to display highest score

    void Start()
    {
        // Initial update of the score when the game starts
        UpdateScoreDisplay();
    }

    // Method to update the score on the UI
    public void UpdateScoreDisplay()
    {
        // Update the current score display
        scoreText.text = "" + PersistentMenuManager.Instance.totalScore.ToString();

        // Update the highest score display
        highestScoreText.text = "" + PersistentMenuManager.Instance.highestScore.ToString();
    }

    // Example method to call when the score changes (e.g., when the player earns points)
    public void OnScoreChange()
    {
        // Call this method when the score is updated
        UpdateScoreDisplay();
    }
}
