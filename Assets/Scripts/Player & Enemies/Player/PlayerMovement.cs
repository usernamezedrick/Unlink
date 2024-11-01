using System.Collections;
using UnityEngine;
using SojaExiles;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isIdle; // Boolean to detect idle state
    private Transform cameraTransform;

    public GameObject[] interactableObjects; // Array of interactable GameObjects within range

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        //interactableObjects = new GameObject[0]; // Initialize the array
    }

    void Update()
    {
        MovePlayer();
        HandleJump();
        HandleInteract();
        HandleAttack();
    }

    void MovePlayer()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get movement inputs
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Determine if player is idle
        isIdle = (moveX == 0 && moveZ == 0);
        animator.SetBool("isIdle", isIdle);

        // Determine speed based on if player is running
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Calculate the camera-based movement direction
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * moveZ + cameraRight * moveX;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Set animation speed based on movement
        float speedPercent = moveDirection.magnitude * (Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f);
        animator.SetFloat("Speed", speedPercent);

        // Rotate the player to face the direction of movement
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("isJumping", true);
        }
        else if (isGrounded)
        {
            animator.SetBool("isJumping", false);
        }
    }

    
    void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (GameObject interactable in interactableObjects)
            {
                if (interactable != null)
                {
                    animator.SetTrigger("isInteracting");
                    Debug.Log("Interacted with " + interactable.name);

                    SojaExiles.opencloseDoor doorScript = interactable.GetComponent<SojaExiles.opencloseDoor>();
                    if (doorScript != null)
                    {
                        StartCoroutine(doorScript.ToggleDoor()); // Call the ToggleDoor coroutine
                        break; // Exit after interacting with the first interactable
                    }
                }
            }
        }
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            animator.SetTrigger("isAttacking");
            Debug.Log("Attack triggered");
            // Implement attack logic here
        }
    }

    // Trigger detection for interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            AddInteractableObject(other.gameObject); // Add the interactable object
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            RemoveInteractableObject(other.gameObject); // Remove the interactable object
        }
    }

    private void AddInteractableObject(GameObject interactable)
    {
        // Add the new interactable object to the array
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
        // Remove the interactable object from the array
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
