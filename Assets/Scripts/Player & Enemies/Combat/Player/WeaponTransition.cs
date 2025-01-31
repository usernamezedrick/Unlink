using System.Collections;
using UnityEngine;

public class WeaponTransition : MonoBehaviour
{
    // Objects for Enemy
    public GameObject objectToDeactivateEnemy;
    public GameObject objectToActivateEnemy;

    // Objects for Enemy1
    public GameObject objectToDeactivateEnemy1;
    public GameObject objectToActivateEnemy1;

    // Objects for Enemy2
    public GameObject objectToDeactivateEnemy2;
    public GameObject objectToActivateEnemy2;

    // Objects for Enemy3
    public GameObject objectToDeactivateEnemy3;
    public GameObject objectToActivateEnemy3;

    // Objects for Enemy4
    public GameObject objectToDeactivateEnemy4;
    public GameObject objectToActivateEnemy4;

    // Objects for Enemy5
    public GameObject objectToDeactivateEnemy5;
    public GameObject objectToActivateEnemy5;

    private void OnTriggerEnter(Collider other)
    {
        // Check the tag of the collider and handle accordingly
        switch (other.tag)
        {
            case "Enemy":
                StartCoroutine(TransitionObjects(objectToDeactivateEnemy, objectToActivateEnemy));
                break;

            case "Enemy1":
                StartCoroutine(TransitionObjects(objectToDeactivateEnemy1, objectToActivateEnemy1));
                break;

            case "Enemy2":
                StartCoroutine(TransitionObjects(objectToDeactivateEnemy2, objectToActivateEnemy2));
                break;

            case "Enemy3":
                StartCoroutine(TransitionObjects(objectToDeactivateEnemy3, objectToActivateEnemy3));
                break;

            case "Enemy4":
                StartCoroutine(TransitionObjects(objectToDeactivateEnemy4, objectToActivateEnemy4));
                break;

            case "Enemy5":
                StartCoroutine(TransitionObjects(objectToDeactivateEnemy5, objectToActivateEnemy5));
                break;

            default:
                Debug.LogWarning($"Unhandled tag: {other.tag}");
                break;
        }
    }

    private IEnumerator TransitionObjects(GameObject objectToDeactivate, GameObject objectToActivate)
    {
        // Optional delay
        yield return new WaitForSeconds(0.5f);

        // Deactivate the specified object
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
        }

        // Activate the specified object
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
