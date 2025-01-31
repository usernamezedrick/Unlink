using UnityEngine;
using System;

public class PersistentMenuManager : MonoBehaviour
{
    public static PersistentMenuManager Instance { get; private set; }

    // Track the total score and highest score
    public int totalScore = 0;
    public int highestScore = 0;

    private string lastResetKey = "LastResetTimestamp";  // Key to store the timestamp of the last reset

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scene loads
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
        }

        // Load the saved score and highest score on startup
        LoadScore();

        // Check if it's a new day, and reset score if necessary
        CheckAndResetScore();
    }

    // Save both current and highest score to PlayerPrefs
    public void SaveScore()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.SetInt("HighestScore", highestScore);
        PlayerPrefs.Save();
        Debug.Log("Scores saved: Total Score = " + totalScore + ", Highest Score = " + highestScore);  // Debug log for checking save
    }

    // Load score from PlayerPrefs
    public void LoadScore()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0); // Default total score is 0 if not set
        highestScore = PlayerPrefs.GetInt("HighestScore", 0); // Default highest score is 0 if not set
        Debug.Log("Scores loaded: Total Score = " + totalScore + ", Highest Score = " + highestScore);  // Debug log for checking load
    }

    // Reset score if it's a new day
    private void CheckAndResetScore()
    {
        // Get the timestamp of the last reset
        string lastReset = PlayerPrefs.GetString(lastResetKey, string.Empty);

        if (string.IsNullOrEmpty(lastReset))
        {
            // If there's no stored timestamp, it's the first time running, set the current time
            PlayerPrefs.SetString(lastResetKey, DateTime.Now.ToString());
        }
        else
        {
            DateTime lastResetDate = DateTime.Parse(lastReset);
            DateTime currentDate = DateTime.Now;

            // Check if it's a new day
            if (currentDate.Date > lastResetDate.Date)
            {
                // It's a new day, reset the total score to 0
                totalScore = 0;
                SaveScore(); // Save the reset score

                // Update the timestamp to the current time
                PlayerPrefs.SetString(lastResetKey, currentDate.ToString());
            }
        }

        PlayerPrefs.Save();
    }

    // Method to set the total score (replaces the old score)
    public void SetTotalScore(int newScore)
    {
        totalScore = newScore;

        // Compare the current totalScore with the highestScore and update if necessary
        if (totalScore > highestScore)
        {
            highestScore = totalScore;
        }

        SaveScore();  // Save the updated scores
    }

    // Reset the score after winning, this can be called when the player wins
    public void ResetScoreAfterWin(int newScore)
    {
        // Replace the total score after the win with the new score
        totalScore = newScore;

        // Compare the current totalScore with the highestScore and update if necessary
        if (totalScore > highestScore)
        {
            highestScore = totalScore;
        }

        SaveScore();  // Save the updated scores
    }
}
