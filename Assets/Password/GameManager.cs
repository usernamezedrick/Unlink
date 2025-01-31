using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI feedbackText; // Use TextMeshProUGUI instead of Text

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update the feedback text
    public void UpdateFeedback(string message)
    {
        feedbackText.text = message; // Update feedback text
    }
}
