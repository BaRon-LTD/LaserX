using UnityEngine;
using System.Collections;

public class Dragable : MonoBehaviour
{
    public delegate void DragEndedDelegate(Dragable draggableObject);

    public DragEndedDelegate drangEndedCallback;

    private bool isDragged = false;

    private Vector3 mouseDragStartPosition;

    private Vector3 spriteDragStartPosition;

    [SerializeField] private Camera mainCamera;

    private void OnMouseDown()
    {
        isDragged = true;
        mouseDragStartPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        spriteDragStartPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        if (isDragged)
        {
            // Calculate the new position
            Vector3 newPosition = spriteDragStartPosition + (mainCamera.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);

            // Clamp the position within the screen bounds
            transform.position = ClampToScreenBounds(newPosition);
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        drangEndedCallback?.Invoke(this);
        GameManager.Instance?.IncrementMoveCount(); // Increment move count on drag end
    }

    private Vector3 ClampToScreenBounds(Vector3 position)
    {
        // Get screen bounds in world coordinates
        Vector3 minBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, position.z));
        Vector3 maxBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, position.z));

        // Clamp the position within bounds
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);

        return position;
    }
}
