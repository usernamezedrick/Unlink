using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component
    public List<Transform> nodes; // List of node positions for the enemy to patrol
    public float stoppingDistance = 0.5f; // Distance to consider as "reached" for patrolling
    public float lookAroundDuration = 5f; // Time to look around at each node
    public float detectionRange = 10f; // Range for detecting the player
    public float attackRange = 2f; // Range for attacking the player
    public float detectionAngle = 45f; // Angle of the detection cone

    public Transform player; // Reference to the player
    public Animator animator; // Reference to the Animator component

    private bool isMoving; // Moving state for patrolling
    private bool isChasing; // Chasing state for animations
    private bool isAttacking; // Attacking state for animations
    private int currentNodeIndex; // Index of the current target node

    [SerializeField] private int index; // Unique index for each enemy

    private bool isEnemyEnabled = true; // Flag to check if the enemy is enabled

    private void Awake()
    {
        if (nodes.Count > 0)
        {
            currentNodeIndex = Random.Range(0, nodes.Count); // Start at a random node
            StartCoroutine(MoveToNode(nodes[currentNodeIndex].position));
        }
    }

    private void OnEnable()
    {
        // Ensure the NavMeshAgent is enabled when the object is turned on
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
            if (navMeshAgent.isOnNavMesh)
            {
                // Set the destination to the current node
                navMeshAgent.SetDestination(nodes[currentNodeIndex].position);
            }
        }

        isEnemyEnabled = true;
    }

    private void OnDisable()
    {
        if (isEnemyEnabled)
        {
            // Only call ResetPath or Stop if the agent is properly placed on the NavMesh
            if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;  // Stop movement
                animator.SetBool("isWalking", false); // Stop walking animation

                // Clear the path if the agent is on a NavMesh
                navMeshAgent.ResetPath(); // Clear the path
            }

            // Mark the enemy as disabled
            isEnemyEnabled = false;
        }
    }

    private void Update()
    {
        if (isAttacking) return; // Do not update if in attacking state

        if (PlayerInDetectionCone())
        {
            // If player is within detection cone, chase
            ChasePlayer();
        }
        else if (isChasing)
        {
            // Stop chasing and return to patrol if player is lost
            StopChasing();
        }

        // Patrol logic only if not chasing or attacking
        if (!isChasing && nodes.Count > 0)
        {
            if (isMoving && Vector3.Distance(transform.position, nodes[currentNodeIndex].position) <= stoppingDistance)
            {
                isMoving = false; // Set isMoving to false when the node is reached
                animator.SetBool("isWalking", false); // Stop walking animation

                navMeshAgent.speed = 0f; // Stop NavMeshAgent movement
                StartCoroutine(LookAroundAndSetNextNode()); // Start looking around coroutine
            }
        }
    }

    private bool PlayerInDetectionCone()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is within detection range, angle, and not obstructed
        return angleToPlayer <= detectionAngle && distanceToPlayer <= detectionRange;
    }

    private void ChasePlayer()
    {
        if (!isChasing)
        {
            isChasing = true;
            animator.SetBool("isChasing", true);
            navMeshAgent.speed = 5f; // Set speed for chasing
        }

        navMeshAgent.SetDestination(player.position); // Set destination to the player

        // Check if within attack range
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            StartAttacking();
        }
    }

    private void StartAttacking()
    {
        isAttacking = true;
        isChasing = false;
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", true);
        navMeshAgent.isStopped = true; // Stop movement while attacking

        StartCoroutine(AttackRoutine()); // Start the attack routine
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1f); // Wait for attack duration

        // After attack, reset attacking state
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        navMeshAgent.isStopped = false;

        // Resume chasing if player is still in range
        if (PlayerInDetectionCone())
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        animator.SetBool("isChasing", false);
        navMeshAgent.speed = 3.5f; // Reset speed to patrol speed
        SetNextNode(); // Resume patrolling
    }

    private IEnumerator MoveToNode(Vector3 targetNode)
    {
        navMeshAgent.SetDestination(targetNode); // Set the destination to the target node
        navMeshAgent.speed = 3.5f; // Patrol movement speed
        isMoving = true; // Set isMoving to true when moving toward a node
        animator.SetBool("isWalking", true); // Trigger walking animation

        // Wait until the agent has reached its destination
        while (Vector3.Distance(transform.position, targetNode) > stoppingDistance)
        {
            yield return null; // Wait until the next frame
        }

        isMoving = false; // Set isMoving to false when the node is reached
        animator.SetBool("isWalking", false); // Stop walking animation
    }

    private IEnumerator LookAroundAndSetNextNode()
    {
        Debug.Log("Look around!");
        // Trigger the look-around animation
        animator.SetBool("isLookingAround", true);

        // Wait for the look-around duration
        yield return new WaitForSeconds(lookAroundDuration);

        // Stop the look-around animation
        animator.SetBool("isLookingAround", false);

        navMeshAgent.speed = 3.5f; // Re-enable NavMeshAgent movement speed
        SetNextNode(); // Move to the next node
    }

    private void SetNextNode()
    {
        // Choose a new random node index that is different from the current one
        int newNodeIndex;
        do
        {
            newNodeIndex = Random.Range(0, nodes.Count);
        } while (newNodeIndex == currentNodeIndex); // Ensure it's a different node

        currentNodeIndex = newNodeIndex; // Update the current node index

        // Move to the next node
        StartCoroutine(MoveToNode(nodes[currentNodeIndex].position));
    }

    private void OnDrawGizmos()
    {
        // Draw the detection cone in the editor for visual reference
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw nodes in the editor
        Gizmos.color = Color.red;
        foreach (var node in nodes)
        {
            if (node != null)
            {
                Gizmos.DrawSphere(node.position, 0.2f);
            }
        }
    }
}
