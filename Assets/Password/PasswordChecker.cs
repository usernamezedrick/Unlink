using UnityEngine;
using TMPro;

public class PasswordCheckerTMP : MonoBehaviour
{
    // References to the TMP_InputField, Button, and Panels
    public TMP_InputField passwordInputField; // TextMeshPro InputField
    public GameObject submitButton;
    public GameObject panelToActivate;
    public GameObject panelToDeactivate;

    // Password to be set in the Inspector
    [SerializeField]
    private string correctPassword;

    void Start()
    {
        // Ensure the button has a listener
        submitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CheckPassword);
    }

    void CheckPassword()
    {
        // Get the entered password from the TMP_InputField
        string enteredPassword = passwordInputField.text;

        // Check if it matches the correct password
        if (enteredPassword == correctPassword)
        {
            Debug.Log("Password correct! Updating panels.");

            // Activate the specified panel
            if (panelToActivate != null)
            {
                panelToActivate.SetActive(true);
            }

            // Deactivate the specified panel
            if (panelToDeactivate != null)
            {
                panelToDeactivate.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Incorrect password. Try again.");
            // Optional: Provide feedback to the user
        }
    }
}
