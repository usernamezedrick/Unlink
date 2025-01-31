using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The object to spawn
    public List<GameObject> bakedObjects; // List of baked objects to spawn on
    public float minSpawnHeight = 1f; // Minimum height to spawn above the baked object
    public float spawnAreaPadding = 0.5f; // Padding from the edges of the baked object to avoid edge spawning
    public LayerMask collisionLayer; // Layer to check for collision with other objects
    public float checkRadius = 1f; // Radius to check for collisions

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        // Select a random baked object from the list
        GameObject selectedBakedObject = bakedObjects[Random.Range(0, bakedObjects.Count)];

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
            float randomX = Random.Range(bakedPosition.x - (bakedSize.x / 4) + spawnAreaPadding,
                                         bakedPosition.x + (bakedSize.x / 4) - spawnAreaPadding);
            float randomZ = Random.Range(bakedPosition.z - (bakedSize.z / 4) + spawnAreaPadding,
                                         bakedPosition.z + (bakedSize.z / 4) - spawnAreaPadding);

            // Set the spawn position on top of the baked object
            spawnPosition = new Vector3(randomX, bakedCollider.bounds.max.y + minSpawnHeight, randomZ);

            attempts++;
        }
        // Check for collisions at the spawn position
        while (Physics.CheckSphere(spawnPosition, checkRadius, collisionLayer) && attempts < maxAttempts);

        // If no collision detected, spawn the object
        if (attempts < maxAttempts)
        {
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Failed to find a valid spawn position after several attempts.");
        }
    }
}
