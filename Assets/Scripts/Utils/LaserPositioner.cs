using UnityEngine;

public class ResolutionAdaptivePositioner : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
    [SerializeField] private Vector2 referenceScreenPosition; // Screen position at 1920x1080
    [SerializeField] private bool respectSafeArea = true;
    [SerializeField] private bool calculateOnStart = true;
    [SerializeField] private bool recalculateOnScreenChange = true;
    [SerializeField] private float zPosition; // Store the z position separately

    private bool initialized = false;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // If reference position isn't set, calculate it from current transform
        if (referenceScreenPosition == Vector2.zero)
        {
            // Convert current world position to reference screen position
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
            referenceScreenPosition = new Vector2(
                viewportPosition.x * referenceResolution.x,
                viewportPosition.y * referenceResolution.y
            );
        }

        // Store the z position
        zPosition = transform.position.z;
    }

    private void Start()
    {
        if (calculateOnStart)
        {
            PositionForCurrentResolution();
        }
    }

    private void Update()
    {
        // Check if initialized or if screen size has changed and recalculation is enabled
        if (!initialized || (recalculateOnScreenChange && 
            (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)))
        {
            PositionForCurrentResolution();
        }
    }

    public void PositionForCurrentResolution()
    {
        // 1. Convert reference screen position to normalized viewport position (0-1)
        Vector2 viewportPosition = new Vector2(
            referenceScreenPosition.x / referenceResolution.x,
            referenceScreenPosition.y / referenceResolution.y
        );

        // 2. Adjust for safe area if needed
        if (respectSafeArea)
        {
            viewportPosition = AdjustForSafeArea(viewportPosition);
        }

        // 3. Convert viewport position to world position
        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(
            new Vector3(viewportPosition.x, viewportPosition.y, mainCamera.nearClipPlane)
        );

        // 4. Set the position, using the saved z position
        transform.position = new Vector3(worldPosition.x, worldPosition.y, zPosition);

        // Track screen size
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        initialized = true;
    }
    
    private Vector2 AdjustForSafeArea(Vector2 viewportPosition)
    {
        // Get the safe area in screen coordinates
        Rect safeArea = Screen.safeArea;
        
        // Convert safe area to normalized viewport coordinates (0-1)
        Vector2 safeMin = new Vector2(
            safeArea.x / Screen.width,
            safeArea.y / Screen.height
        );
        Vector2 safeMax = new Vector2(
            (safeArea.x + safeArea.width) / Screen.width,
            (safeArea.y + safeArea.height) / Screen.height
        );
        
        // Define edge threshold as percentage of screen
        float edgeThreshold = 0.15f;
        
        // Adjust for edges
        if (viewportPosition.x < edgeThreshold)
        {
            viewportPosition.x = Mathf.Max(viewportPosition.x, safeMin.x);
        }
        else if (viewportPosition.x > (1 - edgeThreshold))
        {
            viewportPosition.x = Mathf.Min(viewportPosition.x, safeMax.x);
        }
        
        if (viewportPosition.y < edgeThreshold)
        {
            viewportPosition.y = Mathf.Max(viewportPosition.y, safeMin.y);
        }
        else if (viewportPosition.y > (1 - edgeThreshold))
        {
            viewportPosition.y = Mathf.Min(viewportPosition.y, safeMax.y);
        }
        
        return viewportPosition;
    }
    
    // Helper method to set reference position using a specific screen coordinate (e.g., 960, 540)
    public void SetReferenceScreenPosition(Vector2 screenPos)
    {
        referenceScreenPosition = screenPos;
        PositionForCurrentResolution();
    }
    
    // For orientation changes
    private void OnRectTransformDimensionsChange()
    {
        if (recalculateOnScreenChange)
        {
            PositionForCurrentResolution();
        }
    }

#if UNITY_EDITOR
    // Visualization in editor to help with positioning
    private void OnDrawGizmosSelected()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;
        
        // Draw a marker at the current reference position
        Vector2 viewportPos = new Vector2(
            referenceScreenPosition.x / referenceResolution.x,
            referenceScreenPosition.y / referenceResolution.y
        );
        
        Vector3 worldPos = mainCamera.ViewportToWorldPoint(
            new Vector3(viewportPos.x, viewportPos.y, mainCamera.nearClipPlane)
        );
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(worldPos, 0.2f);
    }
#endif
}