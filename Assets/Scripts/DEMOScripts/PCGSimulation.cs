using System.Collections.Generic;
using UnityEngine;

public class PCGSimulation : MonoBehaviour
{
    public List<GameObject> prefabsToSpawn; // List of prefabs to spawn
    public List<GameObject> bakedObjects; // List of baked objects to spawn on
    public float minSpawnHeight = 0.2f; // Minimum height to spawn above the baked object
    public float spawnAreaPadding = 0.5f; // Padding from the edges of the baked object to avoid edge spawning
    public LayerMask collisionLayer; // Layer to check for collision with other objects
    public float checkRadius = 1f; // Radius to check for collisions
    public int numberOfPrefabsToSpawn = 5; // Number of prefabs to spawn

    // List to track which baked objects already have a prefab spawned on them
    private List<GameObject> occupiedBakedObjects = new List<GameObject>();

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < numberOfPrefabsToSpawn; i++)
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        // Filter out baked objects that already have a prefab spawned on them
        List<GameObject> availableBakedObjects = new List<GameObject>(bakedObjects);
        availableBakedObjects.RemoveAll(occupiedBakedObjects.Contains);

        // If there are no available baked objects, stop the spawning process
        if (availableBakedObjects.Count == 0)
        {
            Debug.LogWarning("No available baked objects to spawn on.");
            return;
        }

        // Select a random baked object from the list of available baked objects
        GameObject selectedBakedObject = availableBakedObjects[Random.Range(0, availableBakedObjects.Count)];

        // Select a random prefab from the list of prefabs
        GameObject prefabToSpawn = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];

        // Get the bounds of the selected baked object
        Collider bakedCollider = selectedBakedObject.GetComponent<Collider>();
        if (bakedCollider == null) return;

        Vector3 bakedPosition = bakedCollider.bounds.center;
        Vector3 bakedSize = bakedCollider.bounds.size;

        Vector3 spawnPosition;
        int maxAttempts = 10; // Limit the number of spawn attempts
        int attempts = 0;

        do
        {
            // Generate a random position within the bounds of the baked object, with padding
            float randomX = Random.Range(bakedPosition.x - (bakedSize.x / 3) + spawnAreaPadding,
                                         bakedPosition.x + (bakedSize.x / 3) - spawnAreaPadding);
            float randomZ = Random.Range(bakedPosition.z - (bakedSize.z / 3) + spawnAreaPadding,
                                         bakedPosition.z + (bakedSize.z / 3) - spawnAreaPadding);

            // Set the spawn position on top of the baked object
            spawnPosition = new Vector3(randomX, bakedCollider.bounds.max.y + minSpawnHeight, randomZ);

            attempts++;
        }
        // Check for collisions at the spawn position
        while (Physics.CheckSphere(spawnPosition, checkRadius, collisionLayer) && attempts < maxAttempts);

        // If no collision detected, spawn the object
        if (attempts < maxAttempts)
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            // Mark this baked object as occupied
            occupiedBakedObjects.Add(selectedBakedObject);
        }
        else
        {
            Debug.LogWarning("Failed to find a valid spawn position after several attempts.");
        }
    }
}
