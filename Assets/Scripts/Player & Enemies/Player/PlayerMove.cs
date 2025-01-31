using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraMain;
    private Transform child;
    private bool isIdle;

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4;

    private Player playerInput;
    private Animator animator;
    public GameObject[] interactableObjects; // Array of interactable objects within range

    private void Awake()
    {
        playerInput = new Player();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        cameraMain = Camera.main.transform;
        child = transform.GetChild(0).transform;
    }

    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandleRotation();
        UpdateAnimations();
        HandleAttack();
        HandleInteract();
    }

    private void HandleMovement()
    {
        Vector2 movementInput = playerInput.PlayerMain.Move.ReadValue<Vector2>();
        Vector3 move = (cameraMain.forward * movementInput.y + cameraMain.right * movementInput.x);
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);
    }

    private void HandleJump()
    {
        if (playerInput.PlayerMain.Jump.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }

    private void ApplyGravity()
    {
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector2 movementInput = playerInput.PlayerMain.Move.ReadValue<Vector2>();
        if (movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.Euler(new Vector3(child.localEulerAngles.x, cameraMain.localEulerAngles.y, child.localEulerAngles.z));
            child.rotation = Quaternion.Lerp(child.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void UpdateAnimations()
    {
        Vector2 movementInput = playerInput.PlayerMain.Move.ReadValue<Vector2>();
        float speed = movementInput.magnitude;

        animator.SetFloat("Speed", speed);
        isIdle = (speed == 0);
        animator.SetBool("isIdle", isIdle);
    }

    private void HandleAttack()
    {
        if (playerInput.PlayerMain.Attack.triggered)
        {
            animator.SetTrigger("isAttacking");
            Debug.Log("Attack triggered");
            // Implement any additional attack logic here
        }
    }

    private void HandleInteract()
    {
        if (playerInput.PlayerMain.Interact.triggered)
        {
            foreach (GameObject interactable in interactableObjects)
            {
                if (interactable != null)
                {
                    animator.SetTrigger("isInteracting");
                    Debug.Log("Interacted with " + interactable.name);

                    // Check if interactable has a door script and trigger its interaction
                    SojaExiles.opencloseDoor doorScript = interactable.GetComponent<SojaExiles.opencloseDoor>();
                    if (doorScript != null)
                    {
                        StartCoroutine(doorScript.ToggleDoor()); // Call ToggleDoor coroutine
                        break; // Exit after interacting with the first interactable
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            AddInteractableObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            RemoveInteractableObject(other.gameObject);
        }
    }

    private void AddInteractableObject(GameObject interactable)
    {
        GameObject[] newInteractables = new GameObject[interactableObjects.Length + 1];
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            newInteractables[i] = interactableObjects[i];
        }
        newInteractables[newInteractables.Length - 1] = interactable;
        interactableObjects = newInteractables;
    }

    private void RemoveInteractableObject(GameObject interactable)
    {
        GameObject[] newInteractables = new GameObject[interactableObjects.Length - 1];
        int index = 0;
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            if (interactableObjects[i] != interactable)
            {
                newInteractables[index++] = interactableObjects[i];
            }
        }
        interactableObjects = newInteractables;
    }
}
