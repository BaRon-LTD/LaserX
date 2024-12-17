using UnityEngine;
using System.Collections;

public class Dragable : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    Vector3 mousePositionOffset;
    private Vector3 getMouseWorldPosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        mousePositionOffset = gameObject.transform.position - getMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        transform.position = getMouseWorldPosition() + mousePositionOffset;
    }
}
