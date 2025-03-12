using UnityEngine;

public class LaserPositioner : MonoBehaviour
{
    public enum ScreenPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    [SerializeField] private ScreenPosition screenPosition = ScreenPosition.MiddleLeft;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector2 margin = Vector2.zero; // X = horizontal margin, Y = vertical margin

    void Start()
    {
        PositionLaser();
    }

    void PositionLaser()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 viewportPos = GetViewportPosition(screenPosition);
        
        // Convert margin from viewport space (0 to 1) to screen space
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector3 marginInViewport = new Vector3(margin.x / screenSize.x, margin.y / screenSize.y, 0);

        // Apply margin
        viewportPos += marginInViewport;

        Vector3 worldPos = mainCamera.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, mainCamera.nearClipPlane + 1));

        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
    }

    Vector3 GetViewportPosition(ScreenPosition position)
    {
        switch (position)
        {
            case ScreenPosition.TopLeft: return new Vector3(0, 1, 0);
            case ScreenPosition.TopCenter: return new Vector3(0.5f, 1, 0);
            case ScreenPosition.TopRight: return new Vector3(1, 1, 0);
            case ScreenPosition.MiddleLeft: return new Vector3(0, 0.5f, 0);
            case ScreenPosition.MiddleCenter: return new Vector3(0.5f, 0.5f, 0);
            case ScreenPosition.MiddleRight: return new Vector3(1, 0.5f, 0);
            case ScreenPosition.BottomLeft: return new Vector3(0, 0, 0);
            case ScreenPosition.BottomCenter: return new Vector3(0.5f, 0, 0);
            case ScreenPosition.BottomRight: return new Vector3(1, 0, 0);
            default: return new Vector3(0.5f, 0.5f, 0); // Fallback to center
        }
    }
}
