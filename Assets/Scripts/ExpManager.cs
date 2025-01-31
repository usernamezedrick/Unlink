using System.IO;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
    public static ExpManager Instance { get; private set; }
    private string filePath;

    public int Experience { get; private set; }
    public int ExpCap { get; private set; }
    public int Level { get; private set; }

    private void Awake()
    {
        // Ensure there is only one instance of ExpManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if there's already one
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep the ExpManager across scenes
        filePath = Path.Combine(Application.persistentDataPath, "playerExp.json");

        LoadExp();
    }

    public void AddExp(int amount)
    {
        Experience += amount;
        if (Experience >= ExpCap)
        {
            LevelUp();
        }
        SaveExp();
    }

    private void LevelUp()
    {
        Experience -= ExpCap;
        Level++;
        ExpCap = Mathf.RoundToInt(ExpCap * 1.5f); // Customizable level-up logic
    }

    public void SaveExp()
    {
        try
        {
            PlayerExpData data = new PlayerExpData(Experience, ExpCap, Level);
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save EXP data: {e.Message}");
        }
    }

    public void LoadExp()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                PlayerExpData data = JsonUtility.FromJson<PlayerExpData>(json);
                Experience = data.experience;
                ExpCap = data.expCap;
                Level = data.level;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load EXP data: {e.Message}");
                ResetExp(); // Reset if loading fails
            }
        }
        else
        {
            Experience = 0;
            ExpCap = 100;
            Level = 1;
        }
    }

    // Method to reset the save data
    public void ResetExp()
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to delete EXP data: {e.Message}");
            }
        }

        // Reset variables to their initial values
        Experience = 0;
        ExpCap = 100;
        Level = 1;

        // Save the reset data (optional)
        SaveExp();
    }
}