using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    // UI Panels
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject loadingScreenPanel;
    public GameObject challengePanel;
    public GameObject startPanel;

    // Audio Settings
    public Slider backgroundMusicSlider;
    public Slider soundEffectsSlider;
    public AudioSource backgroundMusicAudioSource;
    public AudioSource sfxAudioSource;

    // Loading Screen
    public Slider loadingBar;
    public LoadingScreen loadingScreen;  // Reference to the LoadingScreen script

    // Mute Button and Sprites
    public Button muteButton;
    public Sprite normalSprite;
    public Sprite mutedSprite;

    private bool isMuted = false;
    private float originalBackgroundMusicVolume;
    private float originalSoundEffectsVolume;

    private void Start()
    {
        // Load saved volume settings
        originalBackgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 1f);
        originalSoundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);

        backgroundMusicSlider.value = originalBackgroundMusicVolume;
        soundEffectsSlider.value = originalSoundEffectsVolume;

        // Set the audio volumes based on saved settings
        SetBackgroundMusicVolume(backgroundMusicSlider.value);
        SetSoundEffectsVolume(soundEffectsSlider.value);

        // Add listeners for sliders
        backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundMusicVolume);
        soundEffectsSlider.onValueChanged.AddListener(SetSoundEffectsVolume);

        // Set initial mute button sprite
        UpdateMuteButtonSprite();

        // Show the main menu on start
        ShowMainMenu();
    }

    // Method to toggle mute/unmute
    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            // Store original volume levels before muting
            originalBackgroundMusicVolume = backgroundMusicAudioSource.volume;
            originalSoundEffectsVolume = sfxAudioSource.volume;

            // Mute audio
            backgroundMusicAudioSource.volume = 0;
            sfxAudioSource.volume = 0;
            backgroundMusicSlider.value = 0;
            soundEffectsSlider.value = 0;
        }
        else
        {
            // Restore previous volume levels
            SetBackgroundMusicVolume(originalBackgroundMusicVolume);
            SetSoundEffectsVolume(originalSoundEffectsVolume);
            backgroundMusicSlider.value = originalBackgroundMusicVolume;
            soundEffectsSlider.value = originalSoundEffectsVolume;
        }

        // Update mute button sprite
        UpdateMuteButtonSprite();
    }

    // Method to update mute button sprite
    private void UpdateMuteButtonSprite()
    {
        if (backgroundMusicSlider.value == 0 && soundEffectsSlider.value == 0)
        {
            isMuted = true;
            muteButton.image.sprite = mutedSprite;
        }
        else
        {
            isMuted = false;
            muteButton.image.sprite = normalSprite;
        }
    }

    // Method to show the main menu
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        challengePanel.SetActive(false);
        startPanel.SetActive(false);
        SetMainMenuInteractable(true);
    }

    // Method to start a new game
    public void StartNewGame()
    {
        string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Game savefiles", "cubeData.json");

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Existing save file deleted.");
        }

        StartCoroutine(LoadGameSceneAsync());
    }

    // Method to load an existing game
    public void LoadGame()
    {
        string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Game savefiles", "cubeData.json");

        if (File.Exists(saveFilePath))
        {
            StartCoroutine(LoadGameSceneAsync());
        }
        else
        {
            Debug.Log("No saved game found.");
        }
    }

    // Method to start the game, called by the Start button
    public void StartGame()
    {
        string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Game savefiles", "cubeData.json");
        if (File.Exists(saveFilePath))
        {
            ShowStartPanel();
        }
        else
        {
            StartCoroutine(LoadGameSceneAsync());
        }
    }

    // Method to show the start panel
    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        SetMainMenuInteractable(false);
    }

    // Method to close the start panel
    public void CloseStartPanel()
    {
        startPanel.SetActive(false);
        SetMainMenuInteractable(true);
    }

    // Method to exit the game
    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Method to open settings
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        SetMainMenuInteractable(false);
    }

    // Method to close settings
    public void CloseSettings()
    {
        PlayerPrefs.SetFloat("BackgroundMusicVolume", backgroundMusicSlider.value);
        PlayerPrefs.SetFloat("SoundEffectsVolume", soundEffectsSlider.value);
        PlayerPrefs.Save();

        settingsPanel.SetActive(false);
        SetMainMenuInteractable(true);
    }

    // Method to show challenge panel
    public void ShowChallengePanel()
    {
        challengePanel.SetActive(true);
        SetMainMenuInteractable(false);
    }

    // Method to close challenge panel
    public void CloseChallengePanel()
    {
        challengePanel.SetActive(false);
        SetMainMenuInteractable(true);
    }

    // Method to set main menu buttons interactable state
    private void SetMainMenuInteractable(bool isInteractable)
    {
        foreach (Button button in mainMenuPanel.GetComponentsInChildren<Button>())
        {
            button.interactable = isInteractable;
        }
    }

    // Coroutine to load the game scene asynchronously
    private IEnumerator LoadGameSceneAsync()
    {
        loadingScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        AsyncOperation operation = SceneManager.LoadSceneAsync("Story");
        operation.allowSceneActivation = false;

        // Set loading bar to initial value
        loadingBar.value = 0f;
        loadingScreen.SetLoadingText("Loading...");

        // Load the scene in steps
        for (float progress = 0; progress <= 1; progress += 0.1f)
        {
            // Simulate loading progress
            loadingBar.value = progress;
            yield return new WaitForSeconds(0.5f); // Simulate time taken for loading
        }

        // Switch to "Almost There..." message for the final loading phase
        loadingScreen.SetLoadingText("Almost There...");

        // Wait for a moment before allowing scene activation
        yield return new WaitForSeconds(1f);

        // Allow scene activation
        operation.allowSceneActivation = true;

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            yield return null; // Wait for the next frame
        }

        loadingScreenPanel.SetActive(false);
    }







    // Method to play button click sound
    public void PlayButtonClickSound()
    {
        if (sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
        }
    }

    // Method to set background music volume
    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicAudioSource.volume = volume;
        UpdateMuteButtonSprite();
    }

    // Method to set sound effects volume
    public void SetSoundEffectsVolume(float volume)
    {
        sfxAudioSource.volume = volume;
        UpdateMuteButtonSprite();
    }
}
