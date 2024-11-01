using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;       // Reference to the main menu panel
    public Slider bgmVolumeSlider;         // Slider for BGM volume control
    public Slider sfxVolumeSlider;         // Slider for SFX volume control
    public AudioSource bgmSource;          // Audio Source for BGM
    public AudioSource sfxSource;          // Audio Source for SFX, used for SFX sounds like button click
    public AudioClip buttonClickSound;     // Audio clip for button click sound
    private bool isPaused = false;         // To track the paused state

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    void Start()
    {
        // Ensure the main menu panel is initially inactive
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        // Load saved BGM and SFX volumes from PlayerPrefs
        float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f); // Default to 0.5 if not set
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f); // Default to 0.5 if not set

        // Initialize the BGM volume
        if (bgmSource != null)
        {
            bgmSource.volume = savedBGMVolume;
        }

        // Initialize the SFX volume
        if (sfxSource != null)
        {
            sfxSource.volume = savedSFXVolume;
        }

        // Initialize sliders and set listeners
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = savedBGMVolume;
            bgmVolumeSlider.onValueChanged.AddListener(AdjustBGMVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = savedSFXVolume;
            sfxVolumeSlider.onValueChanged.AddListener(AdjustSFXVolume);
        }
    }

    // Call this method to toggle the pause state
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }
        }
        else
        {
            Time.timeScale = 1;
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }
        }
    }

    // Method to load the Main_Menu scene
    public void LoadMainMenu()
    {
        PlayButtonClickSound(); // Play click sound before loading new scene
        Time.timeScale = 1;
        SceneManager.LoadScene("Main_Menu");
    }

    // Method to adjust BGM volume and save it
    public void AdjustBGMVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        }
    }

    // Method to adjust SFX volume and save it
    public void AdjustSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
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

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
