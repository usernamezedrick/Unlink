using System.IO;
using UnityEngine;

public class CubeSaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class CubeSaveData
    {
        public Vector3 cubePosition;
    }

    private void Awake()
    {
        // Make sure this object persists across scenes
        DontDestroyOnLoad(gameObject);
    }

    private string GetSavePath()
    {
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string saveFolderPath = Path.Combine(desktopPath, "Game savefiles");

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        return Path.Combine(saveFolderPath, "cubeData.json");
    }

    public void SavePosition()
    {
        CubeSaveData data = new CubeSaveData
        {
            cubePosition = transform.position
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log("Cube position saved: " + data.cubePosition);
    }

    public void LoadPosition()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CubeSaveData data = JsonUtility.FromJson<CubeSaveData>(json);
            transform.position = data.cubePosition; // Load the position into the cube
            Debug.Log("Cube position loaded: " + data.cubePosition);
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }

    public void DeleteSave()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted: " + path);
        }
        else
        {
            Debug.LogWarning("No save file found to delete!");
        }
    }
}
