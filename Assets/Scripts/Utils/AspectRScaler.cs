using UnityEngine;

[ExecuteAlways]
public class ResolutionScaler : MonoBehaviour
{
    [SerializeField] private float referenceAspect = 16f / 9f;
    [SerializeField] private bool adjustScale = true;
    [SerializeField] private bool adjustPosition = true;
    [SerializeField] private bool preserveDepth = true;
    [SerializeField] private bool executeInUpdate = false;

    private Camera cam;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    private Vector2 refWorldSize;         // World size of the camera at reference aspect
    private Vector2 marginFromEdge;       // Distance from object to screen edge in world units

    private void Awake()
    {
        cam = Camera.main;
        if (cam == null || !cam.orthographic)
        {
            Debug.LogError("This script is for 2D (orthographic camera) only.");
            return;
        }

        originalScale = transform.localScale;
        originalPosition = transform.position;

        CacheReferenceMargin();
        ApplyScalingAndPosition();
    }

    private void Start()
    {
        ApplyScalingAndPosition();
    }

    private void Update()
    {
        if (executeInUpdate)
        {
            ApplyScalingAndPosition();
        }
    }

    private void CacheReferenceMargin()
    {
        // Reference camera size
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * referenceAspect;
        refWorldSize = new Vector2(camWidth, camHeight);

        // Distance from object to each screen edge
        Vector3 pos = transform.position;
        Vector3 camCenter = cam.transform.position;

        float left = camCenter.x - camWidth / 2f;
        float bottom = camCenter.y - camHeight / 2f;

        marginFromEdge = new Vector2(
            pos.x - left,
            pos.y - bottom
        );
    }

    private void ApplyScalingAndPosition()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null || !cam.orthographic)
            return;

        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * ((float)Screen.width / Screen.height);

        // Scale
        if (adjustScale)
        {
            float scaleFactor = camWidth / refWorldSize.x;
            transform.localScale = new Vector3(
                originalScale.x * scaleFactor,
                originalScale.y * scaleFactor,
                preserveDepth ? originalScale.z : originalScale.z * scaleFactor
            );
        }

        // Position
        if (adjustPosition)
        {
            Vector3 camCenter = cam.transform.position;

            float left = camCenter.x - camWidth / 2f;
            float bottom = camCenter.y - camHeight / 2f;

            Vector3 newPos = new Vector3(
                left + marginFromEdge.x,
                bottom + marginFromEdge.y,
                preserveDepth ? originalPosition.z : transform.position.z
            );

            transform.position = newPos;
        }
    }

    [ContextMenu("Reset Reference Margin")]
    public void ResetToCurrent()
    {
        if (cam == null)
            cam = Camera.main;

        originalScale = transform.localScale;
        originalPosition = transform.position;

        CacheReferenceMargin();
        ApplyScalingAndPosition();
    }
}
