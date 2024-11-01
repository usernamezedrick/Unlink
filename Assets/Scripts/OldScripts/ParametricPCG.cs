using UnityEngine;

public class ParametricPCG : MonoBehaviour
{
    public GameObject platformPrefab; // Prefab for platforms
    public int numberOfPlatforms = 10; // Number of platforms
    public float platformSpacing = 10f; // Spacing between platforms
    public Vector2 platformScaleRange = new Vector2(1f, 3f); // Range for platform scale

    void Start()
    {
        GeneratePlatforms();
    }

    void GeneratePlatforms()
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            // Define position for each platform
            Vector3 platformPosition = new Vector3(0, 0, i * platformSpacing);

            // Random scale for the platform
            float scaleX = Random.Range(platformScaleRange.x, platformScaleRange.y);
            float scaleZ = Random.Range(platformScaleRange.x, platformScaleRange.y);

            // Instantiate and adjust platform scale
            GameObject platform = Instantiate(platformPrefab, platformPosition, Quaternion.identity);
            platform.transform.localScale = new Vector3(scaleX, 1, scaleZ);
        }
    }
}
