using System;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    // Singleton instance
    public static MissionManager Instance;

    // Mission data
    public int EnemiesDefeated { get; private set; }
    public int TotalEnemiesToDefeat = 6;

    public int ModulesInteracted { get; private set; }
    public int TotalModulesToInteract = 6;

    public int CluesInteracted { get; private set; }
    public int TotalCluesToInteract = 3;

    // Event to update mission progress
    public static event Action OnMissionUpdated;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to track enemy defeat
    public void IncrementEnemiesDefeated()
    {
        EnemiesDefeated++;
        OnMissionUpdated?.Invoke(); // Trigger mission update event
    }

    // Method to track module interaction
    public void TrackModuleInteracted()
    {
        ModulesInteracted++;
        OnMissionUpdated?.Invoke(); // Trigger mission update event
    }

    // Method to track clue interaction
    public void TrackClueInteracted()
    {
        CluesInteracted++;
        OnMissionUpdated?.Invoke(); // Trigger mission update event
    }
}
