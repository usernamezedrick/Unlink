using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapActivation : MonoBehaviour
{
    public GameObject Nextfloor;
    public GameObject Lastfloor;
    public bool previousFloor;

    private void OnTriggerEnter(Collider other)
    {
        if (previousFloor == true)
        {
            if (other.CompareTag("Player"))
            {
                Lastfloor.SetActive(false);
            }
        }
        else 
        {
            if (other.CompareTag("Player"))
            {
                Nextfloor.SetActive(true);
            }
        }
    }
}
