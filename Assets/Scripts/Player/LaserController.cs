using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private float maxLength = 8f;
    [SerializeField] private int maxReflections = 8;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask mirrorsLayerMask;

    private LaserInteractable objectHit;
    private LaserInteractable lastHitObject;
    private float hitTimer = 0f;

    private Ray ray;
    private RaycastHit2D hit;

    private void Update()
    {
        // Initialize ray and line renderer
        ray = new Ray(transform.position, transform.up);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        float remainingLength = maxLength;

        // Reset hit object tracking at start of frame
        LaserInteractable currentHitObject = null;

        for (int i = 0; i < maxReflections; i++)
        {
            // Perform raycast
            hit = Physics2D.Raycast(ray.origin, ray.direction, remainingLength, mirrorsLayerMask);
            bool stopRay = false;

            lineRenderer.positionCount += 1;

            if (hit)
            {
                // Draw the line to the hit point
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // Get the hit object
                objectHit = hit.collider.GetComponent<LaserInteractable>();
                if (objectHit != null)
                {
                    currentHitObject = objectHit;

                    // Handle continuous hit timing
                    if (objectHit == lastHitObject)
                    {
                        hitTimer += Time.deltaTime;
                        if (hitTimer >= objectHit.duration)
                        {
                            objectHit.OnLaserHit(ref stopRay);
                            hitTimer = 0f; // Reset timer after triggering
                        }

                        // Stop the laser by setting remainingLength to 0
                        remainingLength = 0f;
                        // Set the laser's end position and exit the current reflection processing
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                        break; // Stop further line calculations while keeping the timer running
                    }
                    else
                    {
                        hitTimer = 0f; // Reset timer when hitting a new object
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
                break;
            }
        }

        // Update last hit object
        lastHitObject = currentHitObject;

        // Reset timer if we didn't hit anything this frame
        if (currentHitObject == null)
        {
            hitTimer = 0f;
            lastHitObject = null;
        }
    }
}