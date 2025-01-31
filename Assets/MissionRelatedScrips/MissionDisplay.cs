using TMPro;
using UnityEngine;

public class MissionDisplay : MonoBehaviour
{
    // Singleton instance
    public static MissionDisplay Instance;

    public TextMeshProUGUI missionTextTMP;
    public GameObject missionSuccessPanel; // Public reference to the Mission Success Panel

    private void Awake()
    {
        // Ensure only one instance of MissionDisplay exists
        if (Instance == null)
        {
            Instance = this; // Assign the instance
            DontDestroyOnLoad(gameObject); // Optional: Keep it persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy the duplicate instance
        }
    }

    private void Start()
    {
        UpdateMissionText(); // Initialize the mission text on start
    }

    private void OnEnable()
    {
        MissionManager.OnMissionUpdated += UpdateMissionText; // Subscribe to mission updates
    }

    private void OnDisable()
    {
        MissionManager.OnMissionUpdated -= UpdateMissionText; // Unsubscribe when disabled
    }

    public void UpdateMissionText()
    {
        if (MissionManager.Instance != null)
        {
            // Get current mission status from the MissionManager
            int enemiesDefeated = MissionManager.Instance.EnemiesDefeated;
            int totalEnemies = MissionManager.Instance.TotalEnemiesToDefeat;

            int modulesInteracted = MissionManager.Instance.ModulesInteracted;
            int totalModules = MissionManager.Instance.TotalModulesToInteract;

            int cluesInteracted = MissionManager.Instance.CluesInteracted;
            int totalClues = MissionManager.Instance.TotalCluesToInteract;

            // Update the mission text
            missionTextTMP.text = $"Missions:\n" +
                                  $"- Defeat Enemies: {enemiesDefeated}/{totalEnemies}\n" +
                                  $"- Interact with Modules: {modulesInteracted}/{totalModules}\n" +
                                  $"- Find Clues: {cluesInteracted}/{totalClues}";

            // Change text color for each individual task when it's completed
            if (enemiesDefeated == totalEnemies)
            {
                missionTextTMP.text = missionTextTMP.text.Replace($"- Defeat Enemies: {enemiesDefeated}/{totalEnemies}",
                                                                  $"<color=green>- Defeat Enemies: {enemiesDefeated}/{totalEnemies}</color>");
            }

            if (modulesInteracted == totalModules)
            {
                missionTextTMP.text = missionTextTMP.text.Replace($"- Interact with Modules: {modulesInteracted}/{totalModules}",
                                                                  $"<color=green>- Interact with Modules: {modulesInteracted}/{totalModules}</color>");
            }

            if (cluesInteracted == totalClues)
            {
                missionTextTMP.text = missionTextTMP.text.Replace($"- Find Clues: {cluesInteracted}/{totalClues}",
                                                                  $"<color=green>- Find Clues: {cluesInteracted}/{totalClues}</color>");
            }

            // Check if all missions are completed and show the success panel
            if (enemiesDefeated == totalEnemies && modulesInteracted == totalModules && cluesInteracted == totalClues)
            {
                ShowMissionSuccessPanel();
            }
        }
    }

    // Show the Mission Success Panel
    private void ShowMissionSuccessPanel()
    {
        if (missionSuccessPanel != null)
        {
            ExpManager.Instance.AddExp(100);
            missionSuccessPanel.SetActive(true);
            Invoke("ReturnToMainMenu", 3f); // Wait 3 seconds before returning to main menu
        }
    }

    // Go back to the main menu
    private void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu");
    }
}
