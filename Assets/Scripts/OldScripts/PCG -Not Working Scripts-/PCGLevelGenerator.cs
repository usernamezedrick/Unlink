using UnityEngine;
using System.Collections.Generic;

public class PCGLevelGenerator : MonoBehaviour
{
    public GameObject[] floorPrefabs;
    public GameObject[] wallPrefabs;
    public GameObject[] ceilingPrefabs;
    public GameObject[] objectPrefabs;

    public int roomWidth = 5;
    public int roomLength = 5;
    public int roomHeight = 3;

    private List<GameObject> generatedObjects = new List<GameObject>();

    void Start()
    {
        GenerateRoom();
    }

    void ClearRoom()
    {
        foreach (var obj in generatedObjects)
        {
            Destroy(obj);
        }
        generatedObjects.Clear();
    }

    void GenerateRoom()
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int z = 0; z < roomLength; z++)
            {
                PlaceGroupedTiles(x, z);
            }
        }

        PlaceRoomObjects();
    }

    void PlaceGroupedTiles(int x, int z)
    {
        // Instantiate floor
        GameObject floorTile = Instantiate(GetRandomPrefab(floorPrefabs), new Vector3(x, 0, z), Quaternion.identity);
        generatedObjects.Add(floorTile);

        // Instantiate walls (on the edges of the room)
        if (x == 0 || x == roomWidth - 1 || z == 0 || z == roomLength - 1)
        {
            for (int y = 1; y < roomHeight; y++)
            {
                // Place walls only along the edges of the room
                if (x == 0 || x == roomWidth - 1)
                {
                    GameObject wallTileZ = Instantiate(GetRandomPrefab(wallPrefabs), new Vector3(x, y, z), Quaternion.identity);
                    generatedObjects.Add(wallTileZ);
                }

                if (z == 0 || z == roomLength - 1)
                {
                    GameObject wallTileX = Instantiate(GetRandomPrefab(wallPrefabs), new Vector3(x, y, z), Quaternion.identity);
                    generatedObjects.Add(wallTileX);
                }
            }
        }

        // Instantiate ceiling
        GameObject ceilingTile = Instantiate(GetRandomPrefab(ceilingPrefabs), new Vector3(x, roomHeight, z), Quaternion.identity);
        generatedObjects.Add(ceilingTile);
    }

    void PlaceRoomObjects()
    {
        List<Vector3> occupiedPositions = new List<Vector3>();

        int numberOfObjects = Random.Range(3, 10); // Number of objects to place
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 randomPosition = GetRandomPosition(occupiedPositions);

            // Only instantiate if a valid position was found
            if (randomPosition != Vector3.zero)
            {
                GameObject roomObject = Instantiate(GetRandomPrefab(objectPrefabs), randomPosition, Quaternion.identity);
                generatedObjects.Add(roomObject);
                occupiedPositions.Add(randomPosition);
            }
        }
    }

    Vector3 GetRandomPosition(List<Vector3> occupiedPositions)
    {
        const float minDistance = 2.0f; // Minimum distance between objects
        int maxAttempts = 100;
        Vector3 randomPosition;

        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            randomPosition = new Vector3(
                Random.Range(1, roomWidth - 1),
                1, // Assuming objects are placed on the floor at Y=1
                Random.Range(1, roomLength - 1)
            );

            bool isValidPosition = true;

            foreach (var occupiedPosition in occupiedPositions)
            {
                if (Vector3.Distance(occupiedPosition, randomPosition) < minDistance)
                {
                    isValidPosition = false;
                    break;
                }
            }

            if (isValidPosition)
            {
                return randomPosition;
            }
        }

        // If no valid position found after max attempts, skip placement
        Debug.LogWarning("Could not find a valid position after max attempts.");
        return Vector3.zero;
    }

    GameObject GetRandomPrefab(GameObject[] prefabArray)
    {
        if (prefabArray == null || prefabArray.Length == 0)
        {
            Debug.LogWarning("Prefab array is empty or null!");
            return null;
        }

        int randomIndex = Random.Range(0, prefabArray.Length);
        return prefabArray[randomIndex];
    }
}
