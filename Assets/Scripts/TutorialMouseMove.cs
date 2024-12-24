using UnityEngine;

public class MouseLineMover : MonoBehaviour
{
    [SerializeField] private GameObject mirror;  // The GameObject representing the start point
    [SerializeField] private Transform endPoint; // The target end point
    [SerializeField] private float speed = 2f;   // Movement speed

    private Vector3 startPoint;   // Calculated start point
    private bool isMovingForward = true; // Direction of movement
    private bool isMouseClicked = false;  // Check if the mouse is clicked

    private void Start()
    {
        if (mirror == null || endPoint == null)
        {
            Debug.LogError("Mirror or End Point is not assigned.");
            enabled = false;
            return;
        }

        // Initialize the start point
        startPoint = mirror.transform.position;

        // Set the initial position to the start point
        transform.position = startPoint;
    }

    private void Update()
    {
        // Check if mouse button is pressed or released
        if (Input.GetMouseButtonDown(0))  // Left click
        {
            isMouseClicked = true;
        }

        if (Input.GetMouseButtonUp(0))  // Left click released
        {
            isMouseClicked = false;

            // Update the start point to the mirror's current position
            startPoint = mirror.transform.position;

            // Reset the direction to start from the new start point
            isMovingForward = true;
        }

        // If the mouse is clicked, stop the movement
        if (isMouseClicked) return;

        // Determine the target position based on the movement direction
        Vector3 target = isMovingForward ? endPoint.position : startPoint;

        // Smoothly move towards the target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if the object has reached the target
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            // Switch direction when reaching the target
            isMovingForward = !isMovingForward;
        }
    }
}
