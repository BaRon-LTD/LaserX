using UnityEngine;

public class SpriteRenderVisibility : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isVisible = true;

    [SerializeField] private float durationRepeat = 2f;


    void Start()
    {
        // Get the SpriteRenderer component attached to the current GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on this GameObject.");
            enabled = false; // Disable the script if no SpriteRenderer is found
        }
        else
        {
            // Start the toggling coroutine
            InvokeRepeating("ToggleVisibility", 0f, durationRepeat);
        }
    }

    void ToggleVisibility()
    {
        // Toggle the visibility state
        isVisible = !isVisible;

        // Set the SpriteRenderer's enabled property
        spriteRenderer.enabled = isVisible;
    }
}
