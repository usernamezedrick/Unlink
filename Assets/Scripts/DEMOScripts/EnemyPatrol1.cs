using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.IO; // Required for file handling

public class EnemyPatrol1 : MonoBehaviour
{
    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component
    public List<Transform> nodes; // List of node positions for the enemy to patrol
    public float stoppingDistance = 0.5f; // Distance to consider as "reached"
    public PrefabSpawner prefabSpawner; // Reference to the PrefabSpawner

    private int currentNodeIndex; // Index of the current target node
    private int nodeReachedCount = 0; // Count of nodes reached
    private float nodeTime = 0f; // Time taken to reach the current node
    private float totalDistanceTraveled = 0f; // Total distance traveled from node to node
    private int nodesExpandedCount = 0; // Count of nodes expanded to reach the target node
    private string logFilePath; // Path to the log file
    private float programStartTime; // Start time of the program
    private bool isRunning = true; // Flag to check if the program is running

    private void Start()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        logFilePath = Application.dataPath + $"/NodeLog_{timestamp}.txt"; // Define the path for the log file

        LogToFile("Program started.\n");
        programStartTime = Time.time; // Record the program start time

        if (nodes.Count > 0)
        {
            currentNodeIndex = Random.Range(0, nodes.Count); // Start at a random node
            prefabSpawner.SpawnInitialPrefabs(); // Spawn prefabs at the start
            StartCoroutine(MoveToNode(nodes[currentNodeIndex].position));
        }
    }

    private void Update()
    {
        if (nodes.Count == 0 || !isRunning) return; // If no nodes or not running, exit early

        // Check if the enemy has reached the current node
        if (Vector3.Distance(transform.position, nodes[currentNodeIndex].position) <= stoppingDistance)
        {
            nodeReachedCount++;
            LogToFile($"Node {nodeReachedCount} reached. Time taken for this node: {nodeTime:F2} seconds. Distance traveled: {totalDistanceTraveled:F2} meters.\n");

            // Calculate and log costs
            float g_n = totalDistanceTraveled; // Actual cost
            float h_n = (transform.position - nodes[currentNodeIndex].position).sqrMagnitude; // Heuristic cost (squared distance)
            float f_n = g_n + h_n; // Total cost f(n)

            LogToFile($"Actual cost g(n): {g_n:F2}, Heuristic cost h(n): {h_n:F2}, Total cost f(n): {f_n:F2}\n");
            LogToFile($"Nodes expanded to reach node {nodeReachedCount}: {nodesExpandedCount}\n");

            nodesExpandedCount = 0; // Reset nodes expanded count for the next node

            if (nodeReachedCount >= 101)
            {
                LogToFile("Reached 101 nodes. Program ending.\n");
                EndProgram(); // End the program
            }

            SetNextNode(); // Move to the next node
        }
    }

    private IEnumerator MoveToNode(Vector3 targetNode)
    {
        navMeshAgent.SetDestination(targetNode);
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(targetNode, path); // Recalculate path to get accurate data

        // Calculate nodes expanded and path distance
        nodesExpandedCount = path.corners.Length - 1; // Exclude starting position
        float g_n = CalculatePathDistance(path);
        LogToFile($"Starting move to node. g(n) (path distance): {g_n:F2}, Nodes expanded: {nodesExpandedCount}");

        float maxTime = 10f; // Timeout after 10 seconds
        float elapsedTime = 0f;
        nodeTime = 0f; // Reset node time at the start of moving to a new node
        totalDistanceTraveled = 0f; // Reset the total distance traveled for the new node
        Vector3 lastPosition = transform.position; // Store the starting position

        while (Vector3.Distance(transform.position, targetNode) > stoppingDistance)
        {
            elapsedTime += Time.deltaTime;
            nodeTime += Time.deltaTime;

            float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
            totalDistanceTraveled += distanceThisFrame;
            lastPosition = transform.position;

            if (elapsedTime > maxTime)
            {
                LogToFile($"Timeout reached after {elapsedTime:F2} seconds. Program ran for {Time.time - programStartTime:F2} seconds before issue.\n");
                EndProgram();
                yield break;
            }

            yield return null;
        }

        prefabSpawner.SpawnNewPrefabs(); // Spawn prefabs at the reached node
    }

    private void SetNextNode()
    {
        int newNodeIndex;
        do
        {
            newNodeIndex = Random.Range(0, nodes.Count);
        } while (newNodeIndex == currentNodeIndex);

        currentNodeIndex = newNodeIndex;
        StartCoroutine(MoveToNode(nodes[currentNodeIndex].position));
    }

    private float CalculatePathDistance(NavMeshPath path)
    {
        float distance = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return distance;
    }

    private void LogToFile(string message)
    {
        File.AppendAllText(logFilePath, message);
        Debug.Log(message);
    }

    private void EndProgram()
    {
        LogTotalRuntime();
        isRunning = false;
        Application.Quit();
    }

    private void LogTotalRuntime()
    {
        float totalRuntime = Time.time - programStartTime;
        LogToFile($"Total runtime: {totalRuntime:F2} seconds.\n");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var node in nodes)
        {
            if (node != null)
            {
                Gizmos.DrawSphere(node.position, 0.2f);
            }
        }

        if (navMeshAgent && navMeshAgent.hasPath)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < navMeshAgent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (isRunning)
        {
            LogTotalRuntime();
        }
    }
}
