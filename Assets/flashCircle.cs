using UnityEngine;

public class CircleToggle : MonoBehaviour
{
    [SerializeField] private GameObject circle1; // Circle 1 GameObject
    [SerializeField] private GameObject circle2; // Circle 2 GameObject
    [SerializeField] private GameObject circle3; // Circle 3 GameObject
    [SerializeField] private GameObject mouse1;  // Mouse 1 GameObject containing TutorialMouseMove
    [SerializeField] private GameObject mouse2;  // Mouse 2 GameObject containing TutorialMouseMove

    private SpriteRenderer activeRenderer; // The currently active circle's SpriteRenderer
    private int lastScore = -1; // Tracks the last processed score
    private float toggleInterval = 2f; // Interval in seconds
    private float timer = 0f;

    private void Update()
    {
        int currentScore = CollectibleItem.GetScore();

        // Check if the score has changed
        if (currentScore != lastScore)
        {
            // Update the active renderer and enable/disable mouse scripts and renderers
            UpdateActiveRenderer(currentScore);
            UpdateMouseScriptsAndRenderers(currentScore);
            lastScore = currentScore;
        }

        // If there's an active renderer, toggle its visibility
        if (activeRenderer != null)
        {
            timer += Time.deltaTime;
            if (timer >= toggleInterval)
            {
                timer = 0f;
                activeRenderer.enabled = !activeRenderer.enabled; // Toggle visibility
            }
        }
    }

    private void UpdateActiveRenderer(int score)
    {
        // Reset the timer and disable the previous renderer if it exists
        timer = 0f;

        if (activeRenderer != null)
        {
            activeRenderer.enabled = false;
        }

        // Determine the new active renderer based on the score
        activeRenderer = GetRendererByScore(score);
    }

    private SpriteRenderer GetRendererByScore(int score)
    {
        GameObject circle = null;

        switch (score)
        {
            case 0:
                circle = circle1;
                break;
            case 1:
                circle = circle2;
                break;
            case 2:
                circle = circle3;
                break;
            default:
                return null; // No active renderer for invalid scores
        }

        // Check if the circle is still valid before accessing its SpriteRenderer
        if (circle != null)
        {
            return circle.GetComponent<SpriteRenderer>();
        }

        return null;
    }

    private void UpdateMouseScriptsAndRenderers(int score)
    {
        // Get the TutorialMouseMove components from the mouse objects
        TutorialMouseMove mouse1Script = mouse1.GetComponent<TutorialMouseMove>();
        TutorialMouseMove mouse2Script = mouse2.GetComponent<TutorialMouseMove>();

        // Get the SpriteRenderer components from the mouse objects
        SpriteRenderer mouse1Renderer = mouse1.GetComponent<SpriteRenderer>();
        SpriteRenderer mouse2Renderer = mouse2.GetComponent<SpriteRenderer>();

        // Enable/disable scripts and renderers based on the score
        switch (score)
        {
            case 0:
                if (mouse1Script != null) mouse1Script.enabled = true;
                if (mouse2Script != null) mouse2Script.enabled = false;

                if (mouse1Renderer != null) mouse1Renderer.enabled = true;
                if (mouse2Renderer != null) mouse2Renderer.enabled = false;
                break;
            case 1:
                if (mouse1Script != null) mouse1Script.enabled = false;
                if (mouse2Script != null) mouse2Script.enabled = true;

                if (mouse1Renderer != null) mouse1Renderer.enabled = false;
                if (mouse2Renderer != null) mouse2Renderer.enabled = true;
                break;
            case 2:
                if (mouse1Script != null) mouse1Script.enabled = false;
                if (mouse2Script != null) mouse2Script.enabled = false;

                if (mouse1Renderer != null) mouse1Renderer.enabled = false;
                if (mouse2Renderer != null) mouse2Renderer.enabled = false;
                break;
            default:
                if (mouse1Script != null) mouse1Script.enabled = false;
                if (mouse2Script != null) mouse2Script.enabled = false;

                if (mouse1Renderer != null) mouse1Renderer.enabled = false;
                if (mouse2Renderer != null) mouse2Renderer.enabled = false;
                break;
        }
    }
}
