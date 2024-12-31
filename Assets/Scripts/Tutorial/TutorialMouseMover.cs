using UnityEngine;

public class TutorialMouseMover : MonoBehaviour
{
   [SerializeField] private GameObject mirror;
   [SerializeField] private Transform endPoint;
   [SerializeField] private float speed = 2f;
   
   private Vector3 startPoint;
   private Vector3 parentChildOffset;
   private bool isMovingForward = true;
   private bool isDragging = false;
   private Transform childTransform;

   private void Start()
   {
       childTransform = transform.GetChild(0);
       startPoint = mirror.transform.position;
       parentChildOffset = transform.position - childTransform.position;
   }

   private void Update()
   {
       if (Input.GetMouseButtonDown(0))
       {
           isDragging = true;
       }
       
       if (Input.GetMouseButtonUp(0))
       {
           isDragging = false;

            // Update the start point to the mirror's current position
            startPoint = mirror.transform.position;

            // Reset the direction to start from the new start point
            isMovingForward = true;
       }

       if (isDragging)
       {
        //    transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + parentChildOffset;
           return;
       }

       Vector3 target = isMovingForward ? endPoint.position : startPoint;
       transform.position = Vector3.MoveTowards(transform.position, target + parentChildOffset, speed * Time.deltaTime);

       if (Vector3.Distance(transform.position - parentChildOffset, target) < 0.01f)
       {
           isMovingForward = !isMovingForward;
       }
   }
}