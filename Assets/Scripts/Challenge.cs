using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace ChallengeNamespace
{
    public static class ListExtensions
    {
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    [System.Serializable]
    public class QuestionData
    {
        public string question;
        public string[] answers;
        public int correctAnswerIndex;
        [HideInInspector] public int shuffledCorrectAnswerIndex;
    }

    [System.Serializable]
    public class GameData
    {
        public int playerHealth;
        public int enemyHealth;
        public int currentQuestionIndex;
        public bool shieldActive;
        public float lastHealTime;
        public float lastShieldTime;
        public List<QuestionData> questionList;
        public bool isPlayerTurn;
    }

    public class Challenge : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI questionText;
        public Button[] answerButtons;
        public TextMeshProUGUI enemyDialogueText;
        public TextMeshProUGUI playerHealthText;
        public TextMeshProUGUI enemyHealthText;
        public GameObject endGamePanel;  // Reference to the end game panel

        [Header("Game Settings")]
        public int playerHealth = 10;
        public int enemyHealth = 5;
        public int maxPlayerHealth = 10;
        public int maxEnemyHealth = 5;

        [Header("Enemy Settings")]
        public float enemyHealCooldown = 10f;
        public float enemyShieldCooldown = 8f;
        public int healAmount = 2;

        [Header("Questions")]
        public List<QuestionData> questionList = new List<QuestionData>();

        [Header("Enemy Intro Taunts")]
        public List<string> enemyIntroTaunts = new List<string>();

        private int currentQuestionIndex = 0;
        private bool isPlayerTurn = true;
        private bool canAnswer = true;
        private bool shieldActive = false;
        private bool gameLoaded = false;
        private float lastHealTime;
        private float lastShieldTime;

        private bool combatHasEnded = false;

        void Start()
        {
            InitializeQuestions();
            SetupAnswerButtons();
            UpdateHealthUI();
            StartCoroutine(EnemyIntroDialogue());
        }

        void InitializeQuestions()
        {
            questionList.Shuffle();
        }

        void SetupAnswerButtons()
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                int index = i;
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
        }

        void OnAnswerSelected(int index)
        {
            if (canAnswer)
            {
                StartCoroutine(HandlePlayerAnswer(index));
            }
        }

        IEnumerator HandlePlayerAnswer(int index)
        {
            canAnswer = false;
            var correctIndex = questionList[currentQuestionIndex].shuffledCorrectAnswerIndex;
            yield return new WaitForSeconds(1f);

            if (index == correctIndex)
            {
                if (shieldActive)
                {
                    DisplayEnemyAction("Shield mitigated 1 damage!", 3f);
                    shieldActive = false;
                }
                else
                {
                    enemyHealth = Mathf.Max(0, enemyHealth - 1);
                    UpdateHealthUI();

                    if (enemyHealth == 0)
                    {
                        EndCombat(true);  // Player wins
                        yield break;
                    }
                }
            }
            else
            {
                DisplayEnemyAction("Wrong answer! Take damage!", 3f);
                yield return new WaitForSeconds(1f);
                playerHealth = Mathf.Max(0, playerHealth - 1);
                UpdateHealthUI();

                if (playerHealth == 0)
                {
                    EndCombat(false);  // Player loses
                    yield break;
                }
            }

            currentQuestionIndex = (currentQuestionIndex + 1) % questionList.Count;
            isPlayerTurn = false;
            yield return new WaitForSeconds(1f);

            StartEnemyTurn();
            DisplayNextQuestion();
            canAnswer = true;
        }

        void DisplayNextQuestion()
        {
            if (questionList.Count == 0) return;

            var questionData = questionList[currentQuestionIndex];
            questionText.text = questionData.question;

            var shuffledAnswers = new List<string>(questionData.answers);
            shuffledAnswers.Shuffle();
            questionData.shuffledCorrectAnswerIndex = shuffledAnswers.IndexOf(questionData.answers[questionData.correctAnswerIndex]);

            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = shuffledAnswers[i];
            }
        }

        void UpdateHealthUI()
        {
            playerHealthText.text = "Player Health: " + playerHealth;
            enemyHealthText.text = "Enemy Health: " + enemyHealth;
        }

        void StartEnemyTurn()
        {
            int action = Random.Range(0, 3);
            switch (action)
            {
                case 0:
                    StartCoroutine(EnemyAttack());
                    break;
                case 1:
                    if (Time.time >= lastHealTime + enemyHealCooldown)
                    {
                        EnemyHeal();
                        lastHealTime = Time.time;
                    }
                    else
                    {
                        StartCoroutine(EnemyAttack());
                    }
                    break;
                case 2:
                    if (Time.time >= lastShieldTime + enemyShieldCooldown)
                    {
                        ActivateShield();
                        lastShieldTime = Time.time;
                    }
                    else
                    {
                        StartCoroutine(EnemyAttack());
                    }
                    break;
            }
            isPlayerTurn = true;
        }

        IEnumerator EnemyAttack()
        {
            DisplayEnemyAction("Enemy attacks!", 3f);
            yield return new WaitForSeconds(1f);
            playerHealth = Mathf.Max(0, playerHealth - 1);
            UpdateHealthUI();

            if (playerHealth == 0)
            {
                EndCombat(false);  // Player loses
            }
        }

        void EnemyHeal()
        {
            enemyHealth = Mathf.Min(maxEnemyHealth, enemyHealth + healAmount);
            DisplayEnemyAction("Enemy healed!", 3f);
            UpdateHealthUI();
        }

        void ActivateShield()
        {
            shieldActive = true;
            DisplayEnemyAction("Enemy shields!", 3f);
        }

        void DisplayEnemyAction(string message, float displayDuration = 3f)
        {
            enemyDialogueText.text = message;
            StartCoroutine(ClearEnemyDialogueAfterDelay(displayDuration));
        }

        IEnumerator ClearEnemyDialogueAfterDelay(float duration)
        {
            yield return new WaitForSeconds(duration);
            enemyDialogueText.text = "";
        }

        IEnumerator EnemyIntroDialogue()
        {
            if (gameLoaded)
            {
                enemyDialogueText.text = "";
                DisplayNextQuestion();
            }
            else
            {
                enemyDialogueText.text = GetRandomIntroTaunt();
                yield return new WaitForSeconds(1f);
                enemyDialogueText.text = "";
                DisplayNextQuestion();
            }

            gameLoaded = false;
        }

        string GetRandomIntroTaunt()
        {
            if (enemyIntroTaunts.Count > 0)
            {
                return enemyIntroTaunts[Random.Range(0, enemyIntroTaunts.Count)];
            }
            return "An enemy appears!";
        }

        void EndCombat(bool playerWon)
        {
            if (playerWon)
            {
                Debug.Log("Player has won the combat!");

                // Update the player's EXP here (example increment)
                ExpManager.Instance.AddExp(50);  // Increase EXP by a set amount, e.g., 50

                // Display the end game panel for 2 seconds before transitioning to the Main Menu
                StartCoroutine(ShowEndGamePanelAndTransition());
            }
            else
            {
                Debug.Log("Player has lost the combat.");
                // Handle the player losing the combat, if necessary
                StartCoroutine(ShowEndGamePanelAndTransition());  // Show the panel for 2 seconds before returning to the main menu
            }
        }

        // Coroutine to show the end game panel and transition after 2 seconds
        IEnumerator ShowEndGamePanelAndTransition()
        {
            // Display the end game panel
            endGamePanel.SetActive(true);

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            // Hide the end game panel and transition to the Main Menu scene
            endGamePanel.SetActive(false);
            SceneManager.LoadScene("Main_Menu");
        }

        public void OnLoadButtonPressed()
        {
            LoadGame();
        }

        public void OnSaveButtonPressed()
        {
            SaveGame();
        }

        void LoadGame()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "gameData.json");

            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);

                playerHealth = loadedData.playerHealth;
                enemyHealth = loadedData.enemyHealth;
                currentQuestionIndex = loadedData.currentQuestionIndex;
                isPlayerTurn = loadedData.isPlayerTurn;
                questionList = loadedData.questionList;
                shieldActive = loadedData.shieldActive;
                lastHealTime = loadedData.lastHealTime;
                lastShieldTime = loadedData.lastShieldTime;

                gameLoaded = true;
                UpdateHealthUI();
                StartCoroutine(EnemyIntroDialogue());
            }
            else
            {
                Debug.Log("No save file found.");
            }
        }

        void SaveGame()
        {
            GameData saveData = new GameData
            {
                playerHealth = playerHealth,
                enemyHealth = enemyHealth,
                currentQuestionIndex = currentQuestionIndex,
                isPlayerTurn = isPlayerTurn,
                questionList = questionList,
                shieldActive = shieldActive,
                lastHealTime = lastHealTime,
                lastShieldTime = lastShieldTime
            };

            string jsonData = JsonUtility.ToJson(saveData);
            string filePath = Path.Combine(Application.persistentDataPath, "gameData.json");

            File.WriteAllText(filePath, jsonData);
        }
    }
}
