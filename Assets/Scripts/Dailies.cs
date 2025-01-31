using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace DailiesNamespace
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
    public class EnemyData
    {
        public string enemyName;
        public int maxHealth;
        public int currentHealth;
        public GameObject enemyPrefab;
    }

    public class Dailies : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI questionText;
        public TextMeshProUGUI enemyDialogueText;
        public TextMeshProUGUI playerHealthText;
        public TextMeshProUGUI enemyHealthText;
        public Button[] answerButtons;

        [Header("Player Settings")]
        public int playerHealth = 10;
        public int maxPlayerHealth = 10;

        [Header("Enemy Settings")]
        public List<EnemyData> enemyDataList;
        public GameObject enemyPlaceholder;
        public float enemyHealCooldown = 10f;
        public float enemyShieldCooldown = 8f;
        public int healAmount = 2;

        [Header("Questions")]
        public List<QuestionData> questionList = new List<QuestionData>();

        private int currentQuestionIndex = 0;
        private int currentEnemyIndex = 0;
        private bool isPlayerTurn = true;
        private bool canAnswer = true;
        private bool shieldActive = false;
        private float lastHealTime;
        private float lastShieldTime;
        private bool combatHasEnded = false;

        void Start()
        {
            InitializeGame();
        }

        void InitializeGame()
        {
            questionList.Shuffle();
            enemyDataList.Shuffle();
            DisplayNextEnemy();
            SetupAnswerButtons();
            UpdateHealthUI();
            DisplayNextQuestion();
        }

        void SetupAnswerButtons()
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                int index = i;
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
                answerButtons[i].interactable = true; // Ensure buttons are enabled
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
                    enemyDataList[currentEnemyIndex].currentHealth = Mathf.Max(0, enemyDataList[currentEnemyIndex].currentHealth - 1);
                    UpdateHealthUI();

                    if (enemyDataList[currentEnemyIndex].currentHealth == 0)
                    {
                        HandleEnemyDefeat();
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
                    EndCombat(false);
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

            // Reset the question UI
            questionText.text = "";

            var questionData = questionList[currentQuestionIndex];
            questionText.text = questionData.question;

            // Shuffle the answers and assign correct index
            var shuffledAnswers = new List<string>(questionData.answers);
            shuffledAnswers.Shuffle();
            questionData.shuffledCorrectAnswerIndex = shuffledAnswers.IndexOf(questionData.answers[questionData.correctAnswerIndex]);

            // Assign the shuffled answers to buttons
            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = shuffledAnswers[i];
            }

            // Re-enable buttons in case they were previously disabled
            foreach (var button in answerButtons)
            {
                button.interactable = true;
            }
        }

        void DisplayNextEnemy()
        {
            if (currentEnemyIndex >= enemyDataList.Count) return;

            var enemyData = enemyDataList[currentEnemyIndex];
            enemyDialogueText.text = "Enemy: " + enemyData.enemyName;
            enemyHealthText.text = "Enemy Health: " + enemyData.currentHealth;

            if (enemyData.enemyPrefab != null && enemyPlaceholder != null)
            {
                Instantiate(enemyData.enemyPrefab, enemyPlaceholder.transform.position, Quaternion.identity);
                Destroy(enemyPlaceholder);
            }

            UpdateHealthUI();
        }

        void UpdateHealthUI()
        {
            playerHealthText.text = "Player Health: " + playerHealth;

            if (currentEnemyIndex < enemyDataList.Count)
            {
                enemyHealthText.text = "Enemy Health: " + enemyDataList[currentEnemyIndex].currentHealth;
            }
        }

        void StartEnemyTurn()
        {
            if (combatHasEnded) return;

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
            var enemyData = enemyDataList[currentEnemyIndex];
            enemyData.currentHealth = Mathf.Min(enemyData.maxHealth, enemyData.currentHealth + healAmount);
            DisplayEnemyAction("Enemy healed!", 3f);
            UpdateHealthUI();
        }

        void ActivateShield()
        {
            shieldActive = true;
            DisplayEnemyAction("Enemy shields!", 3f);
        }

        void DisplayEnemyAction(string message, float duration)
        {
            enemyDialogueText.text = message;
            StartCoroutine(ClearEnemyDialogueAfterDelay(duration));
        }

        IEnumerator ClearEnemyDialogueAfterDelay(float duration)
        {
            yield return new WaitForSeconds(duration);
            enemyDialogueText.text = "";
        }

        void HandleEnemyDefeat()
        {
            currentEnemyIndex++;
            currentQuestionIndex = 0;  // Reset to first question
            if (currentEnemyIndex < enemyDataList.Count)
            {
                DisplayNextEnemy();
                DisplayNextQuestion();
            }
            else
            {
                EndCombat(true);
            }
        }

        void EndCombat(bool playerWon)
        {
            combatHasEnded = true;
            if (playerWon)
            {
                ExpManager.Instance.AddExp(enemyDataList.Count * 20);
            }
            // Further code for handling end of combat
        }
    }
}
