using System.Collections;
using UnityEngine;

namespace SojaExiles
{
    public class opencloseDoor : MonoBehaviour
    {
        public Animator openandclose;  // Shared animator (unique per door)
        [SerializeField] private bool isLocked = true; // Lock state, editable in Inspector
        public bool open = false;      // Unique open state for each door
        public Transform Player;
        public Transform Enemy;

        private float doorCooldown = 1f; // Cooldown duration in seconds
        private float lastActionTime = 0f; // Track when the last action occurred

        void Start()
        {
            open = false; // Ensure the door starts closed
        }

        public IEnumerator ToggleDoor()
        {
            if (Time.time - lastActionTime < doorCooldown) yield break; // Prevent action if cooldown hasn't passed
            if (isLocked) // Prevent door interaction if locked
            {
                Debug.Log("The door is locked and cannot be opened.");
                yield break;
            }

            float dist = Vector3.Distance(Player.position, transform.position);
            if (dist < 15) // Only allow action if the player is within range
            {
                if (!open)
                {
                    yield return StartCoroutine(opening());
                }
                else
                {
                    yield return StartCoroutine(clclosing());
                }
            }
        }

        private void Update()
        {
            // Handle touch input for Android
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began) // When the screen is tapped
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject == gameObject) // Check if it's the door this script is attached to
                        {
                            float dist = Vector3.Distance(Player.position, transform.position);
                            if (dist < 15 && (Time.time - lastActionTime >= doorCooldown))
                            {
                                if (isLocked)
                                {
                                    Debug.Log("The door is locked and cannot be opened.");
                                }
                                else if (!open)
                                {
                                    StartCoroutine(opening());
                                }
                                else
                                {
                                    StartCoroutine(clclosing());
                                }
                            }
                        }
                    }
                }
            }

            // Handle mouse input for PC
            if (Input.GetMouseButtonDown(0)) // Left-click (Mouse)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject) // Check if it's the door this script is attached to
                    {
                        float dist = Vector3.Distance(Player.position, transform.position);
                        if (dist < 15 && (Time.time - lastActionTime >= doorCooldown))
                        {
                            if (isLocked)
                            {
                                Debug.Log("The door is locked and cannot be opened.");
                            }
                            else if (!open)
                            {
                                StartCoroutine(opening());
                            }
                            else
                            {
                                StartCoroutine(clclosing());
                            }
                        }
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy") ||
                collision.gameObject.CompareTag("Enemy1") ||
                collision.gameObject.CompareTag("Enemy2") ||
                collision.gameObject.CompareTag("Enemy3") ||
                collision.gameObject.CompareTag("Enemy4") ||
                collision.gameObject.CompareTag("Enemy5"))
            {
                Debug.Log($"{collision.gameObject.tag} collided with the door.");
                float dist = Vector3.Distance(collision.transform.position, transform.position);

                if (dist < 15 && (Time.time - lastActionTime >= doorCooldown))
                {
                    if (!open)
                    {
                        StartCoroutine(opening());
                    }
                    else
                    {
                        StartCoroutine(Enemyclosing());
                    }
                }
            }
        }

        IEnumerator opening()
        {
            open = true;
            Debug.Log("Opening the door.");
            openandclose.Play("Opening");
            lastActionTime = Time.time; // Update the time of the last action
            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator clclosing()
        {
            Debug.Log("Closing the door.");
            openandclose.Play("Closing");
            open = false;
            lastActionTime = Time.time; // Update the time of the last action
            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator Enemyclosing()
        {
            Debug.Log("Enemy closed the door.");
            openandclose.Play("Closing");
            open = false;
            lastActionTime = Time.time; // Update the time of the last action
            yield return new WaitForSeconds(0.4f);
        }

        public void UnlockDoor()
        {
            isLocked = false;
            Debug.Log("The door has been unlocked. isLocked: " + isLocked);
        }

        public void LockDoor()
        {
            isLocked = true;
            Debug.Log("The door has been locked. isLocked: " + isLocked);
        }
    }
}
