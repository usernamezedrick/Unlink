using System.Collections;
using UnityEngine;

public class EWeaponTransition : MonoBehaviour
{
    private GameObject player; // This will hold the reference to the player object
    public GameObject objectToActivate;   // Object to activate
    public GameObject objectToDeactivate; // Object to deactivate

    private void Start()
    {
        // Find the player GameObject by its tag (assuming it's tagged as "Player")
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found! Please ensure the player GameObject has the 'Player' tag.");
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start the transition of objects
            StartCoroutine(TransitionObjects());
        }
    }

   private IEnumerator TransitionObjects()
{
    yield return new WaitForSeconds(0.5f); // Optional delay
    if (objectToActivate != null)
    {
        objectToActivate.SetActive(true);
    }
    yield return null; // Ensure the activation happens first
    if (objectToDeactivate != null)
    {
        objectToDeactivate.SetActive(false);
    }
}

}
