using UnityEngine;

public class PanelActivator : MonoBehaviour
{
    public GameObject panelToActivate;  // Reference to the panel you want to activate

    // This method will be called when the button is clicked
    public void ShowPanel()
    {
        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);  // Enable the panel
        }
    }
}
