using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterPanel : MonoBehaviour
{
    public string itemName; // Name of the item to pick up
    public GameObject poster; // UI panel to show when clue is picked up
    private bool isPlayerNearby = false;
    private Player playerInput;

    private void Awake()
    {
        playerInput = new Player();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player Poster Interact " + itemName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void Update()
    {
        if (isPlayerNearby && playerInput.PlayerMain.Interact.triggered)
        {
            PickUpItem();
        }
    }

    private void PickUpItem()
    {
        Debug.Log("Poster " + itemName);
        poster.SetActive(true);
    }
}
