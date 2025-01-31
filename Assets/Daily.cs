using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace DailyNamespace
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
        public int totalExpAccumulated;
        public int totalScore;
    }

    public class Daily : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI questionText;
        public Button[] answerButtons;
        public TextMeshProUGUI enemyDialogueText;
        public TextMeshProUGUI playerHealthText;
        public TextMeshProUGUI enemyHealthText;
        public TextMeshProUGUI timerText;

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

        [Header("Combat Settings")]
        public float combatDuration = 180f;

        private int currentQuestionIndex = 0;
        private bool isPlayerTurn = true;
        private bool canAnswer = true;
        private bool shieldActive = false;
        private bool gameLoaded = false;
        private float lastHealTime;
        private float lastShieldTime;

        private bool combatHasEnded = false;

        private float currentCombatTime;

        private int totalExpAccumulated = 0;
        private int totalScore = 0;

        [Header("UI Panels")]
        public GameObject endGamePanel;

        void Start()
        {
            InitializeQuestions();
            SetupAnswerButtons();
            UpdateHealthUI();
            StartCoroutine(EnemyIntroDialogue());
            currentCombatTime = combatDuration;
            UpdateTimerUI();
        }

        void Update()
        {
            if (!combatHasEnded)
            {
                if (currentCombatTime > 0)
                {
                    currentCombatTime -= Time.deltaTime;
                    if (currentCombatTime < 0)
                        currentCombatTime = 0;
                    UpdateTimerUI();
                    if (currentCombatTime <= 0)
                    {
                        Debug.Log("Combat ended due to timer expiration.");
                        EndCombat(false);
                    }
                }
            }
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
            if (canAnswer && !combatHasEnded)
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
                    totalExpAccumulated += 10;
                    totalScore += 100;
                    UpdateHealthUI();

                    if (enemyHealth == 0)
                    {
                        EndCombat(true);
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
            var questionData = questionList[currentQuestionIndex];
            questionText.text = questionData.question;

            var shuffledAnswers = new List<string>(questionData.answers);
            shuffledAnswers.Shuffle();
            questionData.shuffledCorrectAnswerIndex = shuffledAnswers.IndexOf(questionData.answers[questionData.correctAnswerIndex]);

            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < shuffledAnswers.Count)
                {
                    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = shuffledAnswers[i];
                    answerButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
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
            if (!combatHasEnded)
            {
                combatHasEnded = true;

                if (endGamePanel != null)
                {
                    endGamePanel.SetActive(true);
                }

                if (playerWon)
                {
                    Debug.Log("Player has won the combat!");

                    if (ExpManager.Instance != null)
                    {
                        ExpManager.Instance.AddExp(totalExpAccumulated);
                        PersistentMenuManager.Instance.SetTotalScore(totalScore);
                    }
                }
                else
                {
                    Debug.Log("Player has lost the combat!");
                }

                StartCoroutine(HandleEndGameTransition());
            }
        }

        IEnumerator HandleEndGameTransition()
        {
            yield return new WaitForSeconds(2f);

            if (endGamePanel != null)
            {
                endGamePanel.SetActive(false);
            }

            SceneManager.LoadScene("Main_Menu");
        }

        void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(currentCombatTime / 60);
            int seconds = Mathf.FloorToInt(currentCombatTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
