using UnityEngine;
using System.Collections;

public class Dragable : MonoBehaviour
{
    public delegate void DragEndedDelegate(Dragable draggableObject);

    public DragEndedDelegate drangEndedCallback;

    private bool isDragged = false;

    private Vector3 mouseDragStartPosition;

    private Vector3 spriteDragStartPosition;

    [SerializeField]private Camera mainCamera;

    private void OnMouseDown()
    {
        isDragged = true;
        mouseDragStartPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        spriteDragStartPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        if(isDragged){
            transform.position = spriteDragStartPosition + (mainCamera.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);
        }
    }
    private void OnMouseUp() {
        isDragged = false;
        drangEndedCallback(this);
    }
}
