using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // The prefab to be spawned
    public int spawnAmount = 5; // Number of prefabs to spawn
    public float minDistance = 4f; // Minimum distance between spawned prefabs
    public List<Transform> nodes; // List of nodes to avoid during spawn
    private List<GameObject> spawnedPrefabs = new List<GameObject>(); // List to keep track of spawned prefabs

    private Dictionary<string, Transform> namedNodes = new Dictionary<string, Transform>(); // Dictionary to hold named nodes

    // Method to spawn prefabs at the start
    public void SpawnInitialPrefabs()
    {
        InitializeNamedNodes(); // Initialize named nodes
        SpawnRandomPrefabs(transform.position); // Spawn prefabs at the initial position
    }

    // Method to initialize named nodes
    private void InitializeNamedNodes()
    {
        // Assuming nodes are named "Node A", "Node B", "Node C", and "Node D"
        if (nodes.Count >= 4) // Ensure there are at least four nodes
        {
            namedNodes["Node A"] = nodes[0];
            namedNodes["Node B"] = nodes[1];
            namedNodes["Node C"] = nodes[2];
            namedNodes["Node D"] = nodes[3];
        }
        else
        {
            Debug.LogWarning("Not enough nodes provided. Ensure at least 4 nodes are assigned.");
        }
    }

    // Method to spawn new prefabs randomly on the NavMesh
    public void SpawnNewPrefabs()
    {
        RemoveAllPrefabs(); // Remove previously spawned prefabs
        SpawnRandomPrefabs(transform.position); // Spawn new prefabs around the current position
    }

    // Method to spawn random prefabs
    private void SpawnRandomPrefabs(Vector3 position)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnPosition;
            NavMeshHit hit; // Declare the NavMeshHit variable here
            bool validPosition;

            // Find a valid position on the NavMesh, ensuring it's not on a node
            do
            {
                // Generate a random point within a specified range
                spawnPosition = position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
                validPosition = NavMesh.SamplePosition(spawnPosition, out hit, 1.0f, NavMesh.AllAreas);

                // Check if the distance to existing prefabs and nodes is valid
                validPosition &= CheckPrefabSpacing(hit.position);
                validPosition &= !IsNearNode(hit.position); // Ensure it's not near a node
            } while (!validPosition);

            GameObject newPrefab = Instantiate(prefabToSpawn, hit.position, Quaternion.identity);
            spawnedPrefabs.Add(newPrefab); // Add the spawned prefab to the list
        }
    }

    // Method to check if the new prefab is at least a certain distance from existing prefabs
    private bool CheckPrefabSpacing(Vector3 newPrefabPosition)
    {
        foreach (GameObject prefab in spawnedPrefabs)
        {
            if (Vector3.Distance(newPrefabPosition, prefab.transform.position) < minDistance)
            {
                return false; // Too close to another prefab
            }
        }
        return true; // Valid spacing
    }

    // Method to check if a position is too close to any named nodes
    private bool IsNearNode(Vector3 newPosition)
    {
        foreach (var node in namedNodes.Values)
        {
            if (Vector3.Distance(newPosition, node.position) < minDistance)
            {
                return true; // Too close to a node
            }
        }
        return false;
    }

    // Method to remove all spawned prefabs
    public void RemoveAllPrefabs()
    {
        foreach (GameObject prefab in spawnedPrefabs)
        {
            Destroy(prefab); // Destroy each prefab
        }
        spawnedPrefabs.Clear(); // Clear the list
    }
}