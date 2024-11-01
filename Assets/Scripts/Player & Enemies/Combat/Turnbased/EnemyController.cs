using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public GameObject player;
    public CinemachineVirtualCamera enemyCamera;
    public CinemachineVirtualCamera enemyAttackCamera;

    public int health = 100;
    public TextMeshProUGUI healthText;

    public float cameraDuration = 1.5f; // Duration for which the attack camera stays active

    private PlayerController playerController;
    [SerializeField] private int index; // Unique index for each enemy

    private EnemyManager EM;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        UpdateHealthText();
        SetActiveCamera(enemyCamera); // Start with the enemy camera active
        LookAtPlayer();
    }

    public void AttackPlayer()
    {
        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        // Deactivate player's cameras before the attack
        playerController.DeactivatePlayerCameras();

        SetActiveCamera(enemyAttackCamera); // Switch to the attack camera
        yield return new WaitForSeconds(cameraDuration); // Wait for the duration of the attack

        playerController.TakeDamage(20); // Inflict damage on the player
        SetActiveCamera(enemyCamera); // Switch back to the enemy camera
        LookAtPlayer(); // Ensure the enemy looks at the player again
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHealthText();
        if (health <= 0)
        {
            EndGame("Player Wins!");
            StartCoroutine(LoadSceneAsync("Story"));
        }
    }

    // Update to accept CinemachineVirtualCamera
    void SetActiveCamera(CinemachineVirtualCamera activeCamera)
    {
        // Ensure only the active camera is enabled
        enemyCamera.gameObject.SetActive(activeCamera == enemyCamera);
        enemyAttackCamera.gameObject.SetActive(activeCamera == enemyAttackCamera);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Optional: You can add a loading UI or track progress here
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Wait until the next frame
            yield return null;
        }
    }

    void LookAtPlayer()
    {
        transform.LookAt(player.transform.position);
    }

    void UpdateHealthText()
    {
        healthText.text = $"Enemy Health: {health}";
    }

    void EndGame(string result)
    {
        healthText.text = result;
        // You may want to disable the enemy's ability to act or show a game over screen here.
    }
}
