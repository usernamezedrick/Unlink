using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace TurnBasedGameNamespace
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

    public class TurnBasedGame : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI questionText;
        public Button[] answerButtons;
        public TextMeshProUGUI enemyDialogueText;
        public Slider playerHealthSlider; // Slider for player health
        public Slider enemyHealthSlider; // Slider for enemy health

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

        [Header("Win/Loss Objects")]
        public GameObject objectToActivateOnWin;
        public GameObject objectToDeactivateOnWin;
        public GameObject objectToDestroyOnWin;
        public GameObject cameraToActivateOnWin;
        public GameObject objectToActivateBeforeDeactivating;

        private int currentQuestionIndex = 0;
        private bool isPlayerTurn = true;
        private bool canAnswer = true;
        private bool shieldActive = false;
        private bool gameLoaded = false;
        private float lastHealTime;
        private float lastShieldTime;

        private bool combatHasEnded = false;  // Flag to track if combat has ended

        void Start()
        {
            // Initialize sliders
            playerHealthSlider.maxValue = maxPlayerHealth;
            playerHealthSlider.value = playerHealth;

            enemyHealthSlider.maxValue = maxEnemyHealth;
            enemyHealthSlider.value = enemyHealth;

            InitializeQuestions();
            SetupAnswerButtons();
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

            // Shuffle the answers for each question
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
            if (playerHealthSlider != null)
                playerHealthSlider.value = playerHealth;
            else
                Debug.LogError("PlayerHealthSlider is not assigned in the Inspector!");

            if (enemyHealthSlider != null)
                enemyHealthSlider.value = enemyHealth;
            else
                Debug.LogError("EnemyHealthSlider is not assigned in the Inspector!");

            // Optional: Check for TextMeshProUGUI child elements
            var playerHealthText = playerHealthSlider?.GetComponentInChildren<TextMeshProUGUI>();
            var enemyHealthText = enemyHealthSlider?.GetComponentInChildren<TextMeshProUGUI>();

            if (playerHealthText != null)
                playerHealthText.text = $"Player: {playerHealth}/{maxPlayerHealth}";
            else
                Debug.LogWarning("No TextMeshProUGUI found for player health slider.");

            if (enemyHealthText != null)
                enemyHealthText.text = $"Enemy: {enemyHealth}/{maxEnemyHealth}";
            else
                Debug.LogWarning("No TextMeshProUGUI found for enemy health slider.");
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
                EndCombat(false);
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
                ExpManager.Instance.AddExp(10);
                MissionManager.Instance.IncrementEnemiesDefeated();

                if (MissionDisplay.Instance != null)
                {
                    MissionDisplay.Instance.UpdateMissionText();
                }

                if (objectToActivateOnWin != null)
                {
                    objectToActivateOnWin.SetActive(true);
                    Debug.Log("Activated object: " + objectToActivateOnWin.name);
                }

                if (objectToActivateBeforeDeactivating != null)
                {
                    objectToActivateBeforeDeactivating.SetActive(true);
                    Debug.Log("Activated object before deactivating: " + objectToActivateBeforeDeactivating.name);
                }

                if (objectToDeactivateOnWin != null)
                {
                    objectToDeactivateOnWin.SetActive(false);
                    Debug.Log("Deactivated object: " + objectToDeactivateOnWin.name);
                }

                if (objectToDestroyOnWin != null)
                {
                    Destroy(objectToDestroyOnWin);
                    Debug.Log("Destroyed object: " + objectToDestroyOnWin.name);
                }
                if (gameObject.transform.parent != null)
                {
                    gameObject.transform.parent.gameObject.SetActive(false);
                }

                if (cameraToActivateOnWin != null)
                {
                    cameraToActivateOnWin.SetActive(true);
                    Debug.Log("Activated camera: " + cameraToActivateOnWin.name);
                }
            }
            else
            {
                Debug.Log("Player has lost the combat!");
                enemyDialogueText.text = "Game Over!";
            }

            combatHasEnded = true;
        }
    }
}
