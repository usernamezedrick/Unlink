using UnityEngine;

public class ToggleActiveAndSwitchCamera : MonoBehaviour
{
    // Drag and drop the object to enable in the Inspector
    public GameObject targetObject;

    // Reference to the camera GameObject to deactivate
    public GameObject camera1Object;

    // Reference to the Canvas GameObject to deactivate
    public GameObject canvasObject;

    // Initial setup
    private void Start()
    {
        // Ensure targetObject is inactive at the start
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        // Ensure the camera and canvas are initially active
        if (camera1Object != null)
        {
            camera1Object.SetActive(true);
        }

        if (canvasObject != null)
        {
            canvasObject.SetActive(true);
        }
    }

    // Function to be called by the button
    public void OnButtonClick()
    {
        // Enable the target object
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        // Set the camera GameObject inactive
        if (camera1Object != null)
        {
            camera1Object.SetActive(false);
        }

        // Set the Canvas GameObject inactive
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
    }
}
