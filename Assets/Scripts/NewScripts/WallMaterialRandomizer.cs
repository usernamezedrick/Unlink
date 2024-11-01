using UnityEngine;

public class WallMaterialAndTextureRandomizer : MonoBehaviour
{
    //===================Floor Materials and Textures====================
    //-----------First Floor Materials and Textures---------------
    [SerializeField] private Material[] firstFloorMaterials;
    [SerializeField] private Texture2D[] firstFloorTextures;
    //-------------------------------------------------------------
    //-----------Second Floor Materials and Textures---------------
    [SerializeField] private Material[] secondFloorMaterials;
    [SerializeField] private Texture2D[] secondFloorTextures;
    //-------------------------------------------------------------
    //-----------Third Floor Materials and Textures----------------
    [SerializeField] private Material[] thirdFloorMaterials;
    [SerializeField] private Texture2D[] thirdFloorTextures;
    //-------------------------------------------------------------

    void Start()
    {
        // Run both material and texture randomizers for each floor with their respective tags
        FirstFloorWallMaterial();
        FirstFloorWallTexture();

        SecondFloorWallMaterial();
        SecondFloorWallTexture();

        ThirdFloorWallMaterial();
        ThirdFloorWallTexture();
    }

    // =================== Material Randomization ====================

    public void FirstFloorWallMaterial()
    {
        if (firstFloorMaterials.Length == 0)
        {
            Debug.LogWarning("No materials assigned in the First Floor Materials array.");
            return;
        }

        Material randomMaterial = firstFloorMaterials[Random.Range(0, firstFloorMaterials.Length)];
        Debug.Log("Assigned first floor material: " + randomMaterial.name);

        GameObject[] firstFloorWalls = GameObject.FindGameObjectsWithTag("firstFloorWallMaterial");

        foreach (GameObject wall in firstFloorWalls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = randomMaterial;
            }
            else
            {
                Debug.LogWarning("No Renderer found on wall: " + wall.name);
            }
        }
    }

    public void SecondFloorWallMaterial()
    {
        if (secondFloorMaterials.Length == 0)
        {
            Debug.LogWarning("No materials assigned in the Second Floor Materials array.");
            return;
        }

        Material randomMaterial = secondFloorMaterials[Random.Range(0, secondFloorMaterials.Length)];
        Debug.Log("Assigned second floor material: " + randomMaterial.name);

        GameObject[] secondFloorWalls = GameObject.FindGameObjectsWithTag("secondFloorWallMaterial");

        foreach (GameObject wall in secondFloorWalls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = randomMaterial;
            }
            else
            {
                Debug.LogWarning("No Renderer found on wall: " + wall.name);
            }
        }
    }

    public void ThirdFloorWallMaterial()
    {
        if (thirdFloorMaterials.Length == 0)
        {
            Debug.LogWarning("No materials assigned in the Third Floor Materials array.");
            return;
        }

        Material randomMaterial = thirdFloorMaterials[Random.Range(0, thirdFloorMaterials.Length)];
        Debug.Log("Assigned third floor material: " + randomMaterial.name);

        GameObject[] thirdFloorWalls = GameObject.FindGameObjectsWithTag("thirdFloorWallMaterial");

        foreach (GameObject wall in thirdFloorWalls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = randomMaterial;
            }
            else
            {
                Debug.LogWarning("No Renderer found on wall: " + wall.name);
            }
        }
    }

    // =================== Texture Randomization ====================

    public void FirstFloorWallTexture()
    {
        if (firstFloorTextures.Length == 0)
        {
            Debug.LogWarning("No textures assigned in the First Floor Textures array.");
            return;
        }

        Texture2D randomTexture = firstFloorTextures[Random.Range(0, firstFloorTextures.Length)];
        Debug.Log("Assigned first floor texture: " + randomTexture.name);

        GameObject[] firstFloorWalls = GameObject.FindGameObjectsWithTag("firstFloorWallTexture");

        foreach (GameObject wall in firstFloorWalls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Create a new material based on the existing one and assign the texture
                renderer.material = new Material(renderer.material);
                renderer.material.mainTexture = randomTexture;
            }
            else
            {
                Debug.LogWarning("No Renderer found on wall: " + wall.name);
            }
        }
    }

    public void SecondFloorWallTexture()
    {
        if (secondFloorTextures.Length == 0)
        {
            Debug.LogWarning("No textures assigned in the Second Floor Textures array.");
            return;
        }

        Texture2D randomTexture = secondFloorTextures[Random.Range(0, secondFloorTextures.Length)];
        Debug.Log("Assigned second floor texture: " + randomTexture.name);

        GameObject[] secondFloorWalls = GameObject.FindGameObjectsWithTag("secondFloorWallTexture");

        foreach (GameObject wall in secondFloorWalls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material);
                renderer.material.mainTexture = randomTexture;
            }
            else
            {
                Debug.LogWarning("No Renderer found on wall: " + wall.name);
            }
        }
    }

    public void ThirdFloorWallTexture()
    {
        if (thirdFloorTextures.Length == 0)
        {
            Debug.LogWarning("No textures assigned in the Third Floor Textures array.");
            return;
        }

        Texture2D randomTexture = thirdFloorTextures[Random.Range(0, thirdFloorTextures.Length)];
        Debug.Log("Assigned third floor texture: " + randomTexture.name);

        GameObject[] thirdFloorWalls = GameObject.FindGameObjectsWithTag("thirdFloorWallTexture");

        foreach (GameObject wall in thirdFloorWalls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material);
                renderer.material.mainTexture = randomTexture;
            }
            else
            {
                Debug.LogWarning("No Renderer found on wall: " + wall.name);
            }
        }
    }
}
