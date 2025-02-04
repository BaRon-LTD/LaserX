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
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private string newLayerName = "UIIgnoreLaser";
    [SerializeField] private string defaultLayerName = "Default";

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
            Vector3 newPosition = spriteDragStartPosition + (mainCamera.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);
            transform.position = ClampToScreenBounds(newPosition);

            if (targetRectTransform.gameObject.activeInHierarchy && IsInsideRectBounds(transform.position))
            {
                gameObject.layer = LayerMask.NameToLayer(newLayerName);
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer(defaultLayerName);
            }
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        drangEndedCallback?.Invoke(this);
        GameManager.Instance?.IncrementMoveCount();
    }

    private Vector3 ClampToScreenBounds(Vector3 position)
    {
        Vector3 minBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, position.z));
        Vector3 maxBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, position.z));
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        return position;
    }

    private bool IsInsideRectBounds(Vector3 position)
    {
        Vector3 localPosition = targetRectTransform.InverseTransformPoint(position);
        return targetRectTransform.rect.Contains(new Vector2(localPosition.x, localPosition.y));
    }
}
