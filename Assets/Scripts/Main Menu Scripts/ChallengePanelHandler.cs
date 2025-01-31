using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ChallengePanelHandler : MonoBehaviour
{
    // References to buttons and their panels
    public Button challengesButton;
    public Button customButton;
    public GameObject challengesPanel;
    public GameObject customPanel;

    // Lesson buttons and loading screen
    public Button lesson1Button;
    public Button lesson2Button;
    public Button lesson3Button;
    public Button lesson4Button;
    public Button dailyChallengeButton; // Reference for the Daily Challenge button
    public GameObject loadingScreen;

    // TextMeshPro component for loading text
    public TextMeshProUGUI loadingText; // Assign this in the Inspector
    public float radius = 100f;          // Radius of circular path
    public float speed = 1f;             // Speed of circular motion

    private void Start()
    {
        SetChallengesActive();
    }

    public void SetChallengesActive()
    {
        challengesPanel.SetActive(true);
        customPanel.SetActive(false);
        challengesButton.Select();
    }

    public void SetCustomActive()
    {
        challengesPanel.SetActive(false);
        customPanel.SetActive(true);
        customButton.Select();
    }

    // Methods for loading lessons and Daily Challenge
    public void LoadLesson1() => StartCoroutine(LoadLessonScene("Challenger1"));
    public void LoadLesson2() => StartCoroutine(LoadLessonScene("Lesson2Scene"));
    public void LoadLesson3() => StartCoroutine(LoadLessonScene("Lesson3Scene"));
    public void LoadLesson4() => StartCoroutine(LoadLessonScene("Lesson4Scene"));
    public void LoadDailyChallenge() => StartCoroutine(LoadLessonScene("DailyChallenge")); // Load Daily Challenge

    // Coroutine to load scene with loading animation for 10 seconds
    private IEnumerator LoadLessonScene(string sceneName)
    {
        loadingScreen.SetActive(true); // Show loading screen
        StartCoroutine(AnimateLoadingText()); // Start the snake-like animation

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float timer = 0f;

        // Loop to check loading progress
        while (timer < 10f)
        {
            timer += Time.deltaTime;

            // Log loading progress for debugging
            Debug.Log("Loading progress: " + asyncLoad.progress);

            // Ensure the scene is loaded and ready to activate after 10 seconds
            if (asyncLoad.progress >= 0.9f && timer >= 10f)
            {
                Debug.Log("Next scene is ready to load."); // Debug log message
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        loadingScreen.SetActive(false);
    }

    // Coroutine to animate each letter of "LOADING" in a snake-like pattern
    private IEnumerator AnimateLoadingText()
    {
        string loadingString = "LOADING";
        GameObject[] letterObjects = new GameObject[loadingString.Length];

        // Create individual letter GameObjects
        for (int i = 0; i < loadingString.Length; i++)
        {
            letterObjects[i] = new GameObject($"Letter_{loadingString[i]}");
            TextMeshProUGUI letterText = letterObjects[i].AddComponent<TextMeshProUGUI>();
            letterText.text = loadingString[i].ToString();
            letterText.fontSize = 36; // Set font size
            letterText.alignment = TextAlignmentOptions.Center;
            letterText.transform.SetParent(loadingScreen.transform); // Set parent to loading screen
        }

        float time = 0f;

        while (loadingScreen.activeSelf)
        {
            // Update the position of each letter to create a snake-like motion
            for (int i = 0; i < loadingString.Length; i++)
            {
                float angle = time + (i * (Mathf.PI * 2 / loadingString.Length)); // Calculate angle
                Vector3 newPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                letterObjects[i].transform.localPosition = newPosition; // Set new position
            }

            // Move the snake-like animation forward
            time += Time.deltaTime * speed;
            yield return null;
        }
    }
}
