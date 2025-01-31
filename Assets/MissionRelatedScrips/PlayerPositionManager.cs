using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    public Transform playerTransform;  // Reference to your player’s transform

    private void Start()
    {
        LoadPlayerPosition();  // Load player position when the scene starts
    }

    // Save the player position
    public void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat("PlayerPosX", playerTransform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerTransform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerTransform.position.z);
        PlayerPrefs.Save();
    }

    // Load the player position
    public void LoadPlayerPosition()
    {
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");

            playerTransform.position = new Vector3(x, y, z);  // Set the player's position
        }
        else
        {
            // Default position (if no saved data exists)
            playerTransform.position = new Vector3(0, 0, 0);
        }
    }
}
