using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private Transform laserStartPoint; // Reference to the child transform
    [SerializeField] private float maxLength = 8f;
    [SerializeField] private int maxReflections = 8;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask mirrorsLayerMask;
    [SerializeField] private float returnMargin = 0.01f;
    private LaserInteractable objectHit;
    private LaserInteractable lastHitObject;
    private float hitTimer = 0f;

    private Ray ray;
    private RaycastHit2D hit;

    private LaserColorType currentLaserColor = LaserColorType.Red;

    private void Update()
    {
        if (laserStartPoint == null)
        {
            Debug.LogError("Laser start point is not assigned. Please assign a child GameObject as the laser start point.");
            return;
        }

        
        // Initialize ray and line renderer
        Vector3 startPoint = laserStartPoint.position; // Use the child transform position
        ray = new Ray(startPoint, laserStartPoint.right); // Use the child transform's direction
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPoint);
        float remainingLength = maxLength;
        
        // Get current color from GameManager and convert to enum
        currentLaserColor = LaserColorUtility.GetLaserColorType(GameManager.Instance.GetCurrentLaserColorIndex());
        lineRenderer.startColor = LaserColorUtility.GetColor(currentLaserColor);
        lineRenderer.endColor = LaserColorUtility.GetColor(currentLaserColor);

        // Set line width based on color
        float lineWidth = LaserColorUtility.GetLineWidth(currentLaserColor);
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        // Reset hit object tracking at start of frame
        LaserInteractable currentHitObject = null;

        // Exclude the UIIgnoreLaser layer from the raycast by adjusting the LayerMask
        LayerMask layerMaskWithoutUI = mirrorsLayerMask & ~(1 << LayerMask.NameToLayer("UIIgnoreLaser"));

        for (int i = 0; i < maxReflections; i++)
        {
            // Perform raycast
            hit = Physics2D.Raycast(ray.origin, ray.direction, remainingLength, layerMaskWithoutUI);
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
                        break; // Stop further line calculations while keeping the timer running
                    }
                    else
                    {
                        hitTimer = 0f; // Reset timer when hitting a new object
                    }
                }

                // Reflect the ray and reduce remaining length
                remainingLength -= Vector2.Distance(ray.origin, hit.point);
                ray = new Ray(hit.point + hit.normal * returnMargin, Vector2.Reflect(ray.direction, hit.normal));
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

// // LaserColorUtility.cs
// using UnityEngine;

public static class LaserColorUtility
{
    public static Color GetColor(LaserColorType colorType)
    {
        return colorType switch
        {
            LaserColorType.Red => Color.red,
            LaserColorType.Blue => Color.blue,
            LaserColorType.Green => Color.green,
            LaserColorType.Yellow => Color.yellow,
            LaserColorType.Purple => new Color(0.5f, 0f, 0.5f, 1f), // Purple
            _ => Color.red // Default to red
        };
    }

    public static LaserColorType GetLaserColorType(int index)
    {
        if (System.Enum.IsDefined(typeof(LaserColorType), index))
        {
            return (LaserColorType)index;
        }
        return LaserColorType.Red; // Default to red if invalid index
    }

    public static float GetLineWidth(LaserColorType colorType)
    {
        return colorType switch
        {
            LaserColorType.Red => 0.05f,
            LaserColorType.Blue => 0.2f,
            LaserColorType.Green => 0.2f,
            LaserColorType.Yellow => 0.2f,
            LaserColorType.Purple => 0.2f,
            _ => 0.1f // Default width
        };
    }
}

// LaserColorType.cs
public enum LaserColorType
{
    Red = 0,
    Blue = 1,
    Green = 2,
    Yellow = 3,
    Purple = 4
}
