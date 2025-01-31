using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject defaultPanel;         // Reference to the default panel (Pause and Sound buttons)
    public GameObject soundPanel;           // Reference to the sound panel (BGM/SFX volume sliders, etc.)
    public Slider bgmVolumeSlider;          // Slider for BGM volume control
    public Slider sfxVolumeSlider;          // Slider for SFX volume control
    public Slider masterVolumeSlider;       // Slider for Master volume control
    public AudioSource bgmSource;           // Audio Source for BGM
    public AudioSource sfxSource;           // Audio Source for SFX, used for SFX sounds like button click
    public AudioClip buttonClickSound;      // Audio clip for button click sound

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MASTER_VOLUME_KEY = "MasterVolume";

    private bool isPaused = false;          // To track if the game is paused

    void Start()
    {
        // Ensure the default panel is initially active
        if (defaultPanel != null)
        {
            defaultPanel.SetActive(true);
        }

        // Ensure the sound panel is initially inactive
        if (soundPanel != null)
        {
            soundPanel.SetActive(false);
        }

        // Load saved volumes from PlayerPrefs
        float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f); // Default to 0.5 if not set
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f); // Default to 0.5 if not set
        float savedMasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 0.5f); // Default to 0.5 if not set

        // Initialize the BGM and SFX volumes based on master volume
        SetVolume(bgmSource, savedBGMVolume, savedMasterVolume);
        SetVolume(sfxSource, savedSFXVolume, savedMasterVolume);

        // Initialize sliders and set listeners
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = savedBGMVolume;
            bgmVolumeSlider.onValueChanged.AddListener((value) => AdjustBGMVolume(value, masterVolumeSlider.value));
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = savedSFXVolume;
            sfxVolumeSlider.onValueChanged.AddListener((value) => AdjustSFXVolume(value, masterVolumeSlider.value));
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = savedMasterVolume;
            masterVolumeSlider.onValueChanged.AddListener((value) => AdjustMasterVolume(value));
        }
    }

    // Call this method to toggle the pause state
    public void TogglePause()
    {
        isPaused = !isPaused; // Toggle the pause state

        if (isPaused)
        {
            Time.timeScale = 0; // Pause the game
            if (defaultPanel != null)
            {
                defaultPanel.SetActive(true); // Show the default panel
            }
        }
        else
        {
            Time.timeScale = 1; // Unpause the game
            if (defaultPanel != null)
            {
                defaultPanel.SetActive(false); // Hide the default panel
            }
        }
    }

    // Method to go to the Main_Menu scene
    public void LoadMainMenu()
    {
        PlayButtonClickSound(); // Play click sound before loading new scene
        Time.timeScale = 1;
        SceneManager.LoadScene("Main_Menu");
    }

    // Method to unpause the game (hide the pause menu)
    public void Unpause()
    {
        Time.timeScale = 1;
        if (defaultPanel != null)
        {
            defaultPanel.SetActive(false);   // Hide the default panel
        }
    }

    // Method to adjust BGM volume and save it
    public void AdjustBGMVolume(float volume, float masterVolume)
    {
        SetVolume(bgmSource, volume, masterVolume);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
    }

    // Method to adjust SFX volume and save it
    public void AdjustSFXVolume(float volume, float masterVolume)
    {
        SetVolume(sfxSource, volume, masterVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
    }

    // Method to adjust Master volume and apply to BGM/SFX
    public void AdjustMasterVolume(float volume)
    {
        SetVolume(bgmSource, bgmVolumeSlider.value, volume);
        SetVolume(sfxSource, sfxVolumeSlider.value, volume);
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
    }

    // Helper method to apply master volume multiplier to audio sources
    private void SetVolume(AudioSource source, float volume, float masterVolume)
    {
        if (source != null)
        {
            source.volume = volume * masterVolume;
        }
    }

    // Method to play button click sound at the current SFX volume level
    public void PlayButtonClickSound()
    {
        if (sfxSource != null && buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound, sfxSource.volume); // Play the button click sound at current SFX volume
        }
    }

    // Method to open the Sound Panel
    public void OpenSoundPanel()
    {
        if (defaultPanel != null)
        {
            defaultPanel.SetActive(false); // Hide the default panel
        }

        if (soundPanel != null)
        {
            soundPanel.SetActive(true);   // Show the sound panel
        }
    }

    // Method to return to the Default Panel from the Sound Panel
    public void ReturnToDefaultPanel()
    {
        if (soundPanel != null)
        {
            soundPanel.SetActive(false);  // Hide the sound panel
        }

        if (defaultPanel != null)
        {
            defaultPanel.SetActive(true);  // Show the default panel
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
