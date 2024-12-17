using UnityEngine;

public class Laser : MonoBehaviour 
{
    [SerializeField] private float maxLength = 8f;
    [SerializeField] private int maxReflections = 8;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask mirrorsLayerMask;

    private Ray ray;
    private RaycastHit2D hit;

    private void Update() 
    {
        // Initialize ray and line renderer
        ray = new Ray(transform.position, transform.up);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        float remainingLength = maxLength;

        for (int i = 0; i < maxReflections; i++) 
        {
            // Perform raycast
            hit = Physics2D.Raycast(ray.origin, ray.direction, remainingLength, mirrorsLayerMask);
            bool stopRay = false; // Add stopRay flag to control reflection
            // Add a new position to the line renderer
            lineRenderer.positionCount += 1;

            if (hit) 
            {
                // Draw the line to the hit point
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // Notify the hit object (if it has a LaserInteractable component)
                LaserInteractable interactable = hit.collider.GetComponent<LaserInteractable>();
                if (interactable != null) 
                {
                    interactable.OnLaserHit(ref stopRay);

                    // Stop reflection if the object requires it
                    if (stopRay) 
                    {
                        break; // Stop further reflections
                    }
                }

                // Reflect the ray and reduce remaining length
                remainingLength -= Vector2.Distance(ray.origin, hit.point);
                ray = new Ray(hit.point + hit.normal * 0.01f, Vector2.Reflect(ray.direction, hit.normal));
            } 
            else 
            {
                // No hit, extend the laser to the remaining length
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
                break; // Stop the loop since no further reflections are possible
            }
        }
    }
}
