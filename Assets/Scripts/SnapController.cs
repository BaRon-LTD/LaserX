using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapController : MonoBehaviour{

    [SerializeField]public List<Transform> snapPoints;
    [SerializeField]public List<Dragable> draggableObjects;

    [SerializeField]public float snapRange = 0.5f;

    private void Start() {

        foreach (Dragable dragable in draggableObjects)
        {
            dragable.drangEndedCallback = OnDragEnded;
        }
    }
    private void OnDragEnded(Dragable draggable){
        float closesDistance = -1;
        Transform closestSnapPoint = null;

        foreach (Transform snapPoint in snapPoints)
        {
            float currentDistance = Vector2.Distance(draggable.transform.position , snapPoint.position);
            if(closestSnapPoint == null || currentDistance < closesDistance){
                closestSnapPoint = snapPoint;
                closesDistance = currentDistance;
            }
        }

        if(closestSnapPoint != null && closesDistance <= snapRange)
        {
            draggable.transform.position = closestSnapPoint.position;
        }
    }
}