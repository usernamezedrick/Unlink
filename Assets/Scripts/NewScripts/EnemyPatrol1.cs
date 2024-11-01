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
    private Vector3 lastPosition; // Store the last position to calculate distance

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
            lastPosition = transform.position; // Initialize the last position
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
            // Mathematical formula f(n) = g(n) + h(n)
            float g_n = totalDistanceTraveled; // Actual cost
            float h_n = (transform.position - nodes[currentNodeIndex].position).sqrMagnitude; // Heuristic cost (squared distance)
            float f_n = g_n + h_n; // Total cost f(n)

            // Log actual cost, heuristic cost, and total cost
            LogToFile($"Actual cost g(n): {g_n:F2}, Heuristic cost h(n): {h_n:F2}, Total cost f(n): {f_n:F2}\n");

            // Log number of nodes expanded to reach this target node
            LogToFile($"Nodes expanded to reach node {nodeReachedCount}: {nodesExpandedCount}\n");

            // Reset nodes expanded count for the next node
            nodesExpandedCount = 0;

            // Check if reached maximum nodes
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
        navMeshAgent.SetDestination(targetNode); // Set the destination to the target node
        float maxTime = 10f; // Timeout after 10 seconds
        float elapsedTime = 0f;
        nodeTime = 0f; // Reset node time at the start of moving to a new node
        totalDistanceTraveled = 0f; // Reset the total distance traveled for the new node

        lastPosition = transform.position; // Store the starting position

        // Wait until the agent has reached its destination or timeout
        while (Vector3.Distance(transform.position, targetNode) > stoppingDistance)
        {
            elapsedTime += Time.deltaTime;
            nodeTime += Time.deltaTime; // Increment node time

            // Calculate the distance traveled since the last frame
            float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
            totalDistanceTraveled += distanceThisFrame; // Add to total distance traveled
            lastPosition = transform.position; // Update last position

            // Increment the count of nodes expanded
            nodesExpandedCount++;

            // Check if timeout occurs
            if (elapsedTime > maxTime)
            {
                LogToFile($"Timeout reached after {elapsedTime:F2} seconds. Program ran for {Time.time - programStartTime:F2} seconds before issue.\n");
                EndProgram(); // End the program if timeout occurs
                yield break; // Exit the coroutine
            }

            yield return null; // Wait until the next frame
        }

        // After reaching the target node, spawn new prefabs
        prefabSpawner.SpawnNewPrefabs(); // Spawn prefabs at the reached node
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

    // Method to log messages to the file
    private void LogToFile(string message)
    {
        File.AppendAllText(logFilePath, message); // Append message to log file
        Debug.Log(message); // Also log it to the Unity Console
    }

    private void EndProgram()
    {
        LogTotalRuntime(); // Log total runtime before quitting
        isRunning = false; // Set running flag to false
        Application.Quit(); // Stop the program
    }

    private void LogTotalRuntime()
    {
        float totalRuntime = Time.time - programStartTime; // Calculate total   
        LogToFile($"Total runtime: {totalRuntime:F2} seconds.\n"); // Log total runtime
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

    private void OnApplicationQuit()
    {
        // Ensure total runtime is logged when the application quits
        if (isRunning)
        {
            LogTotalRuntime();
        }
    }
}
