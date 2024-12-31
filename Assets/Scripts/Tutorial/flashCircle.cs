using System.Collections.Generic;
using UnityEngine;

public class CircleToggle : MonoBehaviour
{
    [Header("Circles Configuration")]
    [SerializeField] private List<GameObject> circles = new List<GameObject>(); // List of circle GameObjects

    [Header("Mouse Configuration")]
    [SerializeField] private List<GameObject> mice = new List<GameObject>(); // List of mouse GameObjects

    private SpriteRenderer activeRenderer; // The currently active circle's SpriteRenderer

    private int lastScore = -1; // Tracks the last processed score
    private float toggleInterval = 2f; // Interval in seconds
    private float timer = 0f;

    private void Update()
    {
        int currentScore = GameManager.Instance.GetScore();

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
        // Ensure the score is within the bounds of the list
        if (score >= 0 && score < circles.Count)
        {
            GameObject circle = circles[score];
            if (circle != null)
            {
                return circle.GetComponent<SpriteRenderer>();
            }
        }
        return null;
    }

    private void UpdateMouseScriptsAndRenderers(int score)
    {
        for (int i = 0; i <= mice.Count; i++)
        {
            // Get the TutorialMouseMove component and SpriteRenderer for each mouse
            TutorialMouseMove mouseScript = mice[i]?.GetComponent<TutorialMouseMove>();
            SpriteRenderer mouseRenderer = mice[i]?.GetComponent<SpriteRenderer>();

            // Enable or disable based on the score
            bool isActive = i == score;
            if (mouseScript != null) mouseScript.enabled = isActive;
            if (mouseRenderer != null) mouseRenderer.enabled = isActive;
        }
    }
}
