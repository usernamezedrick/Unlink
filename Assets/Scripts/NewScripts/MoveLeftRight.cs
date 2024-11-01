using UnityEngine;

public class MoveLeftRight : MonoBehaviour
{
    public float speed = 2f; // Speed of movement
    public float leftBoundary = -5f; // Left boundary
    public float rightBoundary = 5f; // Right boundary
    private bool movingRight = true; // Boolean to check the direction

    void Update()
    {
        // If moving right and the object hasn't hit the right boundary
        if (movingRight)
        {
            // Move the object to the right
            transform.Translate(Vector3.right * speed * Time.deltaTime);

            // If the object goes beyond the right boundary, change direction
            if (transform.position.x >= rightBoundary)
            {
                movingRight = false;
            }
        }
        else
        {
            // Move the object to the left
            transform.Translate(Vector3.left * speed * Time.deltaTime);

            // If the object goes beyond the left boundary, change direction
            if (transform.position.x <= leftBoundary)
            {
                movingRight = true;
            }
        }
    }
}
