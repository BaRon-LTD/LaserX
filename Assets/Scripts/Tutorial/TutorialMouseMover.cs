using UnityEngine;

public class TutorialMouseMover : MonoBehaviour
{
    [SerializeField] private GameObject mirror; // Reference to the mirror object
    [SerializeField] private Transform endPoint; // Target end point for the hand movement
    [SerializeField] private float speed = 2f; // Speed of the hand movement
    [SerializeField] private float distanceReturn = 0.01f; // Distance threshold for returning
    [SerializeField] private float radius = 1f; // Radius within which the hand will not be visible

    private Vector3 startPoint; // Starting point of the tutorial hand
    private Vector3 parentChildOffset; // Offset between parent and child
    private bool isMovingForward = true; // Direction of movement
    private bool isDragging = false; // Dragging state
    private Transform childTransform;
    private SpriteRenderer handRenderer; // Renderer to control visibility

    private void Start()
    {
        childTransform = transform.GetChild(0); // Get the child transform of the tutorial hand
        startPoint = mirror.transform.position; // Initialize the starting point as the mirror's position
        parentChildOffset = transform.position - childTransform.position;

        // Get the SpriteRenderer of the hand
        handRenderer = GetComponent<SpriteRenderer>();
        if (handRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found on the tutorial hand!");
        }
    }

    private void Update()
    {
        // Check if the mirror is within the radius of the endPoint
        bool isWithinRadius = Vector3.Distance(mirror.transform.position, endPoint.position) <= radius;

        if (isWithinRadius)
        {
            UpdateHandVisibility(false); // Hide the tutorial hand when within the radius
        }
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            UpdateHandVisibility(false); // Hide the tutorial hand when dragging
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            UpdateHandVisibility(true); // Show the tutorial hand when not dragging

            // Restart the route from the mirror's new position
            ResetRouteFromMirror();
        }

        if (isDragging)
        {
            // Do not move the tutorial hand while dragging
            return;
        }

        MoveHand();
    }

    private void ResetRouteFromMirror()
    {
        // Set the start point to the current mirror position
        startPoint = mirror.transform.position;

        // Reset the tutorial hand's position to the start point
        transform.position = startPoint;

        // Ensure the hand starts moving forward from the new position
        isMovingForward = true;
    }

    private void MoveHand()
    {
        // Determine the target based on the movement direction
        Vector3 target = isMovingForward ? endPoint.position : startPoint;

        // Move the hand towards the target
        transform.position = Vector3.MoveTowards(transform.position, target + parentChildOffset, speed * Time.deltaTime);

        // Switch direction if the hand reaches the target
        if (Vector3.Distance(transform.position - parentChildOffset, target) < distanceReturn)
        {
            isMovingForward = !isMovingForward;
        }
    }

    private void UpdateHandVisibility(bool isVisible)
    {
        if (handRenderer != null)
        {
            handRenderer.enabled = isVisible; // Toggle the visibility of the hand
        }
    }
}
