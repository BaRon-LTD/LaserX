using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    [SerializeField] private List<Transform> snapPoints;
    [SerializeField] private List<Dragable> draggableObjects;

    [SerializeField] private RectTransform targetRectTransform;

    [SerializeField] private float snapRange = 0.5f;

    private void Start()
    {
        foreach (Dragable dragable in draggableObjects)
        {
            dragable.drangEndedCallback = OnDragEnded;
        }
    }

    private void OnDragEnded(Dragable draggable)
    {
        // Check if targetRectTransform is active
        if (targetRectTransform != null && targetRectTransform.gameObject.activeInHierarchy)
        {
            float closestDistance = float.MaxValue;
            Transform closestSnapPoint = null;

            foreach (Transform snapPoint in snapPoints)
            {
                float currentDistance = Vector2.Distance(draggable.transform.position, snapPoint.position);
                if (currentDistance < closestDistance)
                {
                    closestSnapPoint = snapPoint;
                    closestDistance = currentDistance;
                }
            }

            if (closestSnapPoint != null && closestDistance <= snapRange)
            {
                draggable.transform.position = closestSnapPoint.position;
            }
        }
    }
}
