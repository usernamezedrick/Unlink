using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public GameObject enemy; // The enemy GameObject
    public CinemachineVirtualCamera playerCamera; // Camera for the player
    public CinemachineVirtualCamera playerAttackCamera; // Camera for when the player attacks

    public Button[] answerButtons; // Array of buttons for answer options
    public TextMeshProUGUI questionText; // Text for the question
    public TextMeshProUGUI healthText; // Text for displaying player health

    public int health = 100; // Player health
    private bool isPlayerTurn = true; // Track if it's the player's turn

    public float cameraDuration = 1.5f; // Duration for which the attack camera stays active

    private EnemyController enemyController; // Reference to the enemy controller
    private QuestionManager questionManager; // Reference to the question manager
    private Question currentQuestion; // Current question being asked

    void Start()
    {
        enemyController = enemy.GetComponent<EnemyController>();
        questionManager = FindObjectOfType<QuestionManager>(); // Find QuestionManager in the scene

        Debug.Log("EnemyController: " + enemyController);
        Debug.Log("QuestionManager: " + questionManager);

        // Ensure questionManager is not null
        if (questionManager == null)
        {
            Debug.LogError("QuestionManager is not assigned or found!");
            return; // Exit Start if there's no QuestionManager
        }

        SetQuestion();
        UpdateHealthText();
        SetActiveCamera(playerCamera);
        LookAtEnemy();
    }

    void SetQuestion()
    {
        Debug.Log("Fetching current question from QuestionManager...");
        currentQuestion = questionManager.GetCurrentQuestion(); // Get the current question

        // Check if currentQuestion is null
        if (currentQuestion == null)
        {
            Debug.LogError("No current question found!");
            return; // Exit the method if there's no question
        }

        Debug.Log("Current Question: " + currentQuestion.questionText); // Log the question text
        questionText.text = currentQuestion.questionText; // Set question text

        // Ensure answers are not null or empty
        if (currentQuestion.answers == null || currentQuestion.answers.Length == 0)
        {
            Debug.LogError("Current question has no answers!");
            return; // Exit if there are no answers
        }

        // Set up the answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool isActive = i < currentQuestion.answers.Length; // Determine if the button should be active
            answerButtons[i].gameObject.SetActive(isActive); // Display relevant buttons
            if (isActive)
            {
                // Set button text
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];

                // Capture index for the button
                int index = i;
                answerButtons[i].onClick.RemoveAllListeners(); // Clear any previous listeners
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index)); // Add listener for button click
            }
        }
    }

    void CheckAnswer(int index)
    {
        if (isPlayerTurn)
        {
            if (index == currentQuestion.correctAnswerIndex)
            {
                StartCoroutine(AttackEnemy());
            }
            else
            {
                enemyController.AttackPlayer(); // Call EnemyController to attack player
            }

            questionManager.NextQuestion(); // Move to the next question for the next turn
        }
    }

    IEnumerator AttackEnemy()
    {
        isPlayerTurn = false;
        SetActiveCamera(playerAttackCamera); // Switch to attack camera
        yield return new WaitForSeconds(cameraDuration); // Wait for attack animation

        enemyController.TakeDamage(20); // Inflict damage on enemy
        SetActiveCamera(playerCamera); // Switch back to player camera
        isPlayerTurn = true;
        SetQuestion(); // Load new question
        LookAtEnemy(); // Ensure player is looking at the enemy
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHealthText();
        if (health <= 0)
        {
            EndGame("Enemy Wins!");
        }
    }

    void SetActiveCamera(CinemachineVirtualCamera activeCamera)
    {
        playerCamera.gameObject.SetActive(activeCamera == playerCamera);
        playerAttackCamera.gameObject.SetActive(activeCamera == playerAttackCamera);
    }

    public void DeactivatePlayerCameras()
    {
        playerCamera.gameObject.SetActive(false);
        playerAttackCamera.gameObject.SetActive(false);
    }

    void LookAtEnemy()
    {
        transform.LookAt(enemy.transform.position);
    }

    void UpdateHealthText()
    {
        healthText.text = $"Player Health: {health}"; // Update health display
    }

    void EndGame(string result)
    {
        questionText.text = result; // Display the result
        foreach (Button button in answerButtons)
        {
            button.interactable = false; // Disable answer buttons
        }
    }
}
