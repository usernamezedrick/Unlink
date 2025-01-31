using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject loadingScreenPanel;
    public GameObject challengePanel;
    public GameObject startPanel;

    public Slider backgroundMusicSlider;
    public Slider soundEffectsSlider;
    public Slider masterVolumeSlider;
    public AudioSource backgroundMusicAudioSource;
    public AudioSource sfxAudioSource;

    public Slider loadingBar;
    public LoadingScreen loadingScreen;

    public Button muteButton;
    public Sprite normalSprite;
    public Sprite mutedSprite;

    private bool isMuted = false;
    private float originalBackgroundMusicVolume;
    private float originalSoundEffectsVolume;
    private float originalMasterVolume = 1f;

    private void Start()
    {
        originalMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        masterVolumeSlider.value = originalMasterVolume;

        originalBackgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 1f);
        originalSoundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);

        backgroundMusicSlider.value = originalBackgroundMusicVolume;
        soundEffectsSlider.value = originalSoundEffectsVolume;

        ApplyMasterVolume(masterVolumeSlider.value);
        SetBackgroundMusicVolume(backgroundMusicSlider.value);
        SetSoundEffectsVolume(soundEffectsSlider.value);

        masterVolumeSlider.onValueChanged.AddListener(ApplyMasterVolume);
        backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundMusicVolume);
        soundEffectsSlider.onValueChanged.AddListener(SetSoundEffectsVolume);

        UpdateMuteButtonSprite();
        ShowMainMenu();
    }

    private void ApplyMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();

        backgroundMusicAudioSource.volume = backgroundMusicSlider.value * volume;
        sfxAudioSource.volume = soundEffectsSlider.value * volume;

        UpdateMuteButtonSprite();
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicAudioSource.volume = volume * masterVolumeSlider.value;
        UpdateMuteButtonSprite();
    }

    public void SetSoundEffectsVolume(float volume)
    {
        sfxAudioSource.volume = volume * masterVolumeSlider.value;
        UpdateMuteButtonSprite();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            originalMasterVolume = masterVolumeSlider.value;
            originalBackgroundMusicVolume = backgroundMusicSlider.value;
            originalSoundEffectsVolume = soundEffectsSlider.value;

            masterVolumeSlider.value = 0f;
            backgroundMusicSlider.value = 0f;
            soundEffectsSlider.value = 0f;
        }
        else
        {
            masterVolumeSlider.value = originalMasterVolume;
            backgroundMusicSlider.value = originalBackgroundMusicVolume;
            soundEffectsSlider.value = originalSoundEffectsVolume;
        }

        ApplyMasterVolume(masterVolumeSlider.value);
        SetBackgroundMusicVolume(backgroundMusicSlider.value);
        SetSoundEffectsVolume(soundEffectsSlider.value);
    }

    private void UpdateMuteButtonSprite()
    {
        if (masterVolumeSlider.value == 0f)
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

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        challengePanel.SetActive(false);
        startPanel.SetActive(false);
        SetMainMenuInteractable(true);
    }

    public void StartNewGame()
    {
        string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Game savefiles", "cubeData.json");

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        StartCoroutine(LoadGameSceneAsync());
    }

    public void LoadGame()
    {
        string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Game savefiles", "cubeData.json");

        if (File.Exists(saveFilePath))
        {
            StartCoroutine(LoadGameSceneAsync());
        }
    }

    public void StartGame()
    {
        string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Game savefiles", "cubeData.json");
        if (File.Exists(saveFilePath))
        {
            ShowStartPanel();
        }
        else
        {
            loadingScreenPanel.SetActive(true); // Show the loading screen
            StartCoroutine(LoadGameSceneAsync());
        }
    }


    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        SetMainMenuInteractable(false);
    }

    public void CloseStartPanel()
    {
        startPanel.SetActive(false);
        SetMainMenuInteractable(true);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        SetMainMenuInteractable(false);
    }

    public void CloseSettings()
    {
        PlayerPrefs.SetFloat("BackgroundMusicVolume", backgroundMusicSlider.value);
        PlayerPrefs.SetFloat("SoundEffectsVolume", soundEffectsSlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.Save();

        settingsPanel.SetActive(false);
        SetMainMenuInteractable(true);
    }

    public void ShowChallengePanel()
    {
        challengePanel.SetActive(true);
        SetMainMenuInteractable(false);
    }

    public void CloseChallengePanel()
    {
        challengePanel.SetActive(false);
        SetMainMenuInteractable(true);
    }

    private void SetMainMenuInteractable(bool isInteractable)
    {
        foreach (Button button in mainMenuPanel.GetComponentsInChildren<Button>())
        {
            button.interactable = isInteractable;
        }
    }

    private IEnumerator LoadGameSceneAsync()
    {
        loadingScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        AsyncOperation operation = SceneManager.LoadSceneAsync("Story");
        operation.allowSceneActivation = false;

        loadingBar.value = 0f;
        loadingScreen.SetLoadingText("Loading...");

        for (float progress = 0; progress <= 1; progress += 0.1f)
        {
            loadingBar.value = progress;
            yield return new WaitForSeconds(0.5f);
        }

        loadingScreen.SetLoadingText("Almost There...");
        yield return new WaitForSeconds(1f);

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        loadingScreenPanel.SetActive(false);
    }

    public void PlayButtonClickSound()
    {
        if (sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
        }
    }
}
