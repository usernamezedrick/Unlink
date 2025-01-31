using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component
    public List<Transform> nodes; // List of node positions for the enemy to patrol
    public float stoppingDistance = 0.5f; // Distance to consider as "reached"

    private int currentNodeIndex; // Index of the current target node

    private void Start()
    {
        if (nodes.Count > 0)

        {
            currentNodeIndex = Random.Range(0, nodes.Count); // Start at a random node
            StartCoroutine(MoveToNode(nodes[currentNodeIndex].position));
        }
    }

    private void Update()
    {
        // Check if we have nodes
        if (nodes.Count == 0) return;

        // Check if the enemy has reached the current node
        if (Vector3.Distance(transform.position, nodes[currentNodeIndex].position) <= stoppingDistance)
        {
            // Set the next node randomly
            SetNextNode();
        }
    }

    private IEnumerator MoveToNode(Vector3 targetNode)
    {
        navMeshAgent.SetDestination(targetNode); // Set the destination to the target node

        // Wait until the agent has reached its destination
        while (Vector3.Distance(transform.position, targetNode) > stoppingDistance)
        {
            yield return null; // Wait until the next frame
        }

        // After reaching the node, set the next target
        SetNextNode();
    }

    private void SetNextNode()
    {
        // Choose a new random node index that is different from the current one
        int newNodeIndex;
        do
        {
            newNodeIndex = Random.Range(0, nodes.Count);
        } while (newNodeIndex == currentNodeIndex);

        currentNodeIndex = newNodeIndex; // Update the current node index
        StartCoroutine(MoveToNode(nodes[currentNodeIndex].position)); // Move to the next node
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with obstacles
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Recalculate the path to the current node if there's a collision
            RecalculatePath();
        }
        // Add check to prevent changing nodes on collision with a node
        else if (nodes.Contains(collision.transform))
        {
            // Do nothing or add logic if needed
            return;
        }
    }

    private void RecalculatePath()
    {
        // Set a new destination based on the current node
        if (currentNodeIndex >= 0 && currentNodeIndex < nodes.Count)
        {
            Vector3 destination = nodes[currentNodeIndex].position;
            navMeshAgent.SetDestination(destination); // Update destination to the current node
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the nodes in the editor for visual reference
        Gizmos.color = Color.red;
        foreach (var node in nodes)
        {
            if (node != null)
            {
                Gizmos.DrawSphere(node.position, 0.2f);
            }
        }

        // Visualize the path the agent is taking
        if (navMeshAgent.hasPath)
        {
            Gizmos.color = Color.yellow;
            foreach (Vector3 point in navMeshAgent.path.corners)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
        }
    }
}