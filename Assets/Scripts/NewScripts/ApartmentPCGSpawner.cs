using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentPCGSpawner : MonoBehaviour
{
    // Array for different types of tables, chairs, pinboards, trashcans, and shelves
    public List<GameObject> tables;
    public List<GameObject> chairs;
    public List<GameObject> pinboards;
    public List<GameObject> trashcans;
    public List<GameObject> shelves;

    // Start is called before the first frame update
    void Start()
    {
        // Enable one random prefab from each list
        EnableRandomPrefab(tables, "Table");
        EnableRandomPrefab(chairs, "Chair");
        EnableRandomPrefab(pinboards, "Pinboard");
        EnableRandomPrefab(trashcans, "Trashcan");
        EnableRandomPrefab(shelves, "Shelf");
    }

    // Function to enable a random prefab from the list and rename it to "ApartmentPCG"
    void EnableRandomPrefab(List<GameObject> prefabList, string category)
    {
        if (prefabList.Count > 0)
        {
            // Choose a random index from the list
            int randomIndex = Random.Range(0, prefabList.Count);

            // Get the selected prefab
            GameObject selectedPrefab = prefabList[randomIndex];

            // Enable the GameObject
            selectedPrefab.SetActive(true);

            // Rename it to "ApartmentPCG"
            selectedPrefab.name = "ApartmentPCG_" + category;

            // Enable the MeshRenderer
            MeshRenderer meshRenderer = selectedPrefab.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }
            else
            {
                Debug.LogWarning("No MeshRenderer found on " + selectedPrefab.name);
            }
        }
        else
        {
            Debug.LogWarning("No prefabs assigned in the inspector for " + category);
        }
    }
}
