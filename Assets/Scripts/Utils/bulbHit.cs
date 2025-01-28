// using Unity.VisualScripting;
// using UnityEngine;

// public class bulbHit : LaserInteractable
// {
//     [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
//     [SerializeField] private Sprite hitSprite;
//     [SerializeField] private Animator bulbAnimator; // Reference to the Animator component

//     private SpriteRenderer spriteRenderer;

//     [SerializeField] private float durationHold = 1f;

//     [SerializeField] [Tooltip("Name of scene to move to when triggering the given tag")] string sceneName;

//     private bool isGlow = false; // Ensure the bulb reacts only once


//     private void Awake()
//     {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         duration = durationHold; // Set instance-specific duration
//     }

//     public override void OnLaserHit(ref bool stopRay)
//     {
//         if (isGlow) return;
//         isGlow = true;

//         if (bulbAnimator != null)
//         {
//             bulbAnimator.SetTrigger("Glow");
//         }

//         // base.OnLaserHit(ref stopRay);
//         stopRay = true;
//         Invoke("PerformCustomBehavior", 2f);
//     }

//     public override void PerformCustomBehavior()
//     {
//         if (hitSprite != null && spriteRenderer != null)
//         {
//             spriteRenderer.sprite = hitSprite;
//         }

//         GameManager.Instance.LoadScene(sceneName);
//     }
// }


using Unity.VisualScripting;
using UnityEngine;

public class bulbHit : LaserInteractable
{
    [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
    [SerializeField] private Sprite hitSprite;
    [SerializeField] private Animator bulbAnimator; // Reference to the Animator component
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip hitSound; // The sound to play on hit
    [SerializeField] private float delaySound = 0.5f; // Delay before the sound plays (adjustable in the Inspector)

    private SpriteRenderer spriteRenderer;

    [SerializeField] private float durationHold = 1f;

    [SerializeField] [Tooltip("Name of scene to move to when triggering the given tag")] string sceneName;

    private bool isGlow = false; // Ensure the bulb reacts only once

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        duration = durationHold; // Set instance-specific duration
    }

    public override void OnLaserHit(ref bool stopRay)
    {
        if (isGlow) return;
        isGlow = true;

        // Trigger the animation
        if (bulbAnimator != null)
        {
            bulbAnimator.SetTrigger("Glow");
        }

        // Play the sound after a delay
        if (audioSource != null && hitSound != null)
        {
            Invoke("PlaySound", delaySound);
        }

        // Stop the ray
        stopRay = true;
    }

    private void PlaySound()
    {
        audioSource.clip = hitSound;
        audioSource.Play();

        // Delay scene transition until the sound finishes playing
        float soundDuration = hitSound.length;
        Invoke("PerformCustomBehavior", delaySound + soundDuration);
    }

    public override void PerformCustomBehavior()
    {
        if (hitSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = hitSprite;
        }

        GameManager.Instance.LoadScene(sceneName);
    }
}