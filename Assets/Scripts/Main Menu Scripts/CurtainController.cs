using UnityEngine;

public class CurtainController : MonoBehaviour
{
    // Reference to the loading screen canvas
    public GameObject loadingScreenCanvas;

    // Reference to the curtain canvas
    public GameObject curtainCanvas;

    // This method will be called at the end of the curtain animation
    public void OnCurtainClosed()
    {
        Debug.Log("Curtain animation ended.");

        // Activate the loading screen and deactivate the curtain canvas
        if (loadingScreenCanvas != null)
        {
            loadingScreenCanvas.SetActive(true);
        }

        if (curtainCanvas != null)
        {
            curtainCanvas.SetActive(false);
        }
    }

    private void Start()
    {
        // Ensure loading screen canvas is inactive at the start
        if (loadingScreenCanvas != null)
        {
            loadingScreenCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("LoadingScreenCanvas is not assigned.");
        }

        // Ensure curtain canvas is active at the start
        if (curtainCanvas != null)
        {
            curtainCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CurtainCanvas is not assigned.");
        }
    }
}
