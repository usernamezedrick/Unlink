using UnityEngine;
using System.Collections.Generic;

public class DistributionPCG : MonoBehaviour
{
    public GameObject[] objectsToPlace; // Array of object prefabs to place
    public int numberOfObjects = 100;   // Number of objects to place
    public Vector2 placementRange = new Vector2(50, 50); // Terrain range for object placement
    public float objectSpacing = 1f;    // Minimum distance between objects

    private List<Vector3> objectPositions = new List<Vector3>(); // List to store positions of placed objects

    void Start()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            TryPlaceObject();
        }

        // After all objects are placed, calculate and log distribution metrics
        CalculateDistributionMetrics();
    }

    void TryPlaceObject()
    {
        Vector3 randomPosition = GetRandomPosition();

        // Define a small radius for overlap check based on the object's size or spacing
        float radius = objectSpacing / 2f;

        // Check if there's already an object in the desired position
        Collider[] colliders = Physics.OverlapSphere(randomPosition, radius);

        if (colliders.Length == 0)
        {
            // No objects detected in the area, proceed to instantiate
            int randomObjectIndex = Random.Range(0, objectsToPlace.Length);
            Instantiate(objectsToPlace[randomObjectIndex], randomPosition, Quaternion.identity);

            // Save the position for testing purposes
            objectPositions.Add(randomPosition);
        }
        else
        {
            // Try again if position is already occupied
            TryPlaceObject();
        }
    }

    Vector3 GetRandomPosition()
    {
        // Generate a random position within the placement range
        return new Vector3(
            Random.Range(-placementRange.x / 2, placementRange.x / 2),
            0,
            Random.Range(-placementRange.y / 2, placementRange.y / 2)
        );
    }

    void CalculateDistributionMetrics()
    {
        if (objectPositions.Count == 0)
        {
            Debug.Log("No objects placed, cannot calculate distribution metrics.");
            return;
        }

        // Calculate the average position of all placed objects
        Vector3 averagePosition = Vector3.zero;
        foreach (var position in objectPositions)
        {
            averagePosition += position;
        }
        averagePosition /= objectPositions.Count;

        // Calculate the standard deviation of positions
        float standardDeviation = CalculateStandardDeviation(averagePosition);

        // Log results
        Debug.Log("=== Distribution Metrics ===");
        Debug.Log($"Total Objects Placed: {objectPositions.Count}");
        Debug.Log($"Average Position: {averagePosition}");
        Debug.Log($"Standard Deviation of Positions: {standardDeviation}");
    }

    float CalculateStandardDeviation(Vector3 avgPosition)
    {
        // Calculate variance for object positions (distance from average)
        float variance = 0;
        foreach (var pos in objectPositions)
        {
            variance += Vector3.SqrMagnitude(pos - avgPosition); // Sum of squared distances
        }
        variance /= objectPositions.Count;

        // Return the square root of the variance (standard deviation)
        return Mathf.Sqrt(variance);
    }
}
