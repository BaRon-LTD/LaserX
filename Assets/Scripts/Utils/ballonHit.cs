// using UnityEngine;

// public class ballonHit : LaserInteractable
// {
//     [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
//     [SerializeField] private Animator bubbleAnimator; // Reference to the Animator component
//     [SerializeField] private GameObject coin; // Reference to the coin inside the bubble
//     [SerializeField] private float destroyDelay = 1f; // Delay before destroying the bubble (adjust as needed)
//     [SerializeField] private float coinDestroyDelay = 3f; // Fixed delay of 3 seconds for coin destruction

//     private bool isPopped = false; // Ensure the bubble reacts only once


//     public override void OnLaserHit(ref bool stopRay)
//     {
//         // Ensure the bubble only reacts once to the laser
//         if (isPopped) return;
//         isPopped = true;


//         // Log the message
//         Debug.Log(uniqueBehaviorMessage);

//         // Trigger the burst animation using SetBool
//         if (bubbleAnimator != null)
//         {
//             bubbleAnimator.SetTrigger("Explode"); 
//         }

//         // Update the score through the GameManager
//         GameManager.Instance.AddScore(1);

//         // Schedule the destruction of the bubble and handle the coin after animation finishes
//         StartCoroutine(DestroyBubbleAfterAnimation());

//         // Allow the laser to continue its path
//         stopRay = false;
//     }

//     private System.Collections.IEnumerator DestroyBubbleAfterAnimation()
//     {
//         // Wait for the animation to finish
//         if (bubbleAnimator != null)
//         {
//             // Get the duration of the "Burst" animation clip
//             float animationLength = bubbleAnimator.GetCurrentAnimatorStateInfo(0).length;

//             // Wait for the animation to finish
//             yield return new WaitForSeconds(animationLength);
//         }

//         // After the animation has finished, enable the coin's Rigidbody2D and make it fall
//         if (coin != null)
//         {

//             if (coin.TryGetComponent<Rigidbody2D>(out var coinRb))
//             {
//                 coinRb.bodyType = RigidbodyType2D.Dynamic; // Set bodyType to Dynamic to enable physics
//             }
//         }

//         // Destroy the bubble
//         Destroy(gameObject);
//     }
// }

//-------------------------------------------------------------------------------------


// using UnityEngine;

// public class ballonHit : LaserInteractable
// {
//     [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
//     [SerializeField] private Animator bubbleAnimator; // Reference to the Animator component
//     [SerializeField] private GameObject coin; // Reference to the coin inside the bubble
//     [SerializeField] private float destroyDelay = 1f; // Delay before destroying the bubble (adjust as needed)
//     [SerializeField] private RectTransform uiCoinCounter; // Reference to the UI coin counter RectTransform
//     [SerializeField] private Canvas canvas; // Reference to the Canvas containing the UI

//     private bool isPopped = false; // Ensure the bubble reacts only once

//     public override void OnLaserHit(ref bool stopRay)
//     {
//         // Ensure the bubble only reacts once to the laser
//         if (isPopped) return;
//         isPopped = true;

//         // Log the message
//         Debug.Log(uniqueBehaviorMessage);

//         // Trigger the burst animation using SetBool
//         if (bubbleAnimator != null)
//         {
//             bubbleAnimator.SetTrigger("Explode"); 
//         }

//         // Update the score through the GameManager
//         GameManager.Instance.AddScore(1);

//         // Schedule the destruction of the bubble and handle the coin after animation finishes
//         StartCoroutine(DestroyBubbleAfterAnimation());

//         // Allow the laser to continue its path
//         stopRay = false;
//     }

//     private System.Collections.IEnumerator DestroyBubbleAfterAnimation()
//     {
//         // Wait for the animation to finish
//         if (bubbleAnimator != null)
//         {
//             float animationLength = bubbleAnimator.GetCurrentAnimatorStateInfo(0).length;
//             yield return new WaitForSeconds(animationLength);
//         }

//         // After the animation, make the coin fall
//         if (coin != null)
//         {
//             if (coin.TryGetComponent<Rigidbody2D>(out var coinRb))
//             {
//                 coinRb.bodyType = RigidbodyType2D.Dynamic; // Enable physics for falling
//             }

//             // Wait for 1 second before moving the coin to UI
//             yield return new WaitForSeconds(1f);

//             // Disable physics and start moving the coin to the UI
//             if (coin.TryGetComponent<Rigidbody2D>(out coinRb))
//             {
//                 coinRb.bodyType = RigidbodyType2D.Kinematic;
//                 coinRb.linearVelocity = Vector2.zero; // Stop any residual movement
//             }

//             StartCoroutine(MoveCoinToUI());
//         }

//         // Destroy the bubble
//         Destroy(gameObject);
//     }

//     private System.Collections.IEnumerator MoveCoinToUI()
//     {
//         if (coin == null || uiCoinCounter == null || canvas == null)
//             yield break;

//         // Convert the coin's world position to screen position
//         Vector2 startScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, coin.transform.position);

//         // Get the UI coin counter position
//         Vector2 endScreenPosition = uiCoinCounter.position;

//         // Debug Log Positions
//         Debug.Log($"Start Screen Position: {startScreenPosition}");
//         Debug.Log($"End Screen Position: {endScreenPosition}");

//         // Animate the coin moving from start to end
//         float duration = 0.5f; // Time for the animation
//         float elapsedTime = 0f;

//         while (elapsedTime < duration)
//         {
//             elapsedTime += Time.deltaTime;

//             // Lerp between the positions
//             Vector2 currentScreenPosition = Vector2.Lerp(startScreenPosition, endScreenPosition, elapsedTime / duration);

//             // Convert the screen position back to world position
//             RectTransformUtility.ScreenPointToWorldPointInRectangle(
//                 canvas.GetComponent<RectTransform>(),
//                 currentScreenPosition,
//                 Camera.main,
//                 out Vector3 worldPosition
//             );

//             // Update the coin's position
//             coin.transform.position = worldPosition;

//             yield return null;
//         }

//         // Snap the coin to the final position and destroy it
//         coin.transform.position = uiCoinCounter.position;
//         Destroy(coin);
//     }
// }


//-------------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;

public class ballonHit : LaserInteractable
{
    [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
    [SerializeField] private Animator bubbleAnimator; // Reference to the Animator component
    [SerializeField] private GameObject coin; // Reference to the coin inside the bubble
    [SerializeField] private float destroyDelay = 1f; // Delay before destroying the bubble (adjust as needed)
    [SerializeField] private Transform uiCoinCounter; // Reference to the UI coin counter Transform

    private bool isPopped = false; // Ensure the bubble reacts only once

    public override void OnLaserHit(ref bool stopRay)
    {
        // Ensure the bubble only reacts once to the laser
        if (isPopped) return;
        isPopped = true;

        // Log the message
        Debug.Log(uniqueBehaviorMessage);

        // Trigger the burst animation
        if (bubbleAnimator != null)
        {
            bubbleAnimator.SetTrigger("Explode"); 
        }

        // Update the score through the GameManager
        GameManager.Instance.AddScore(1);

        // Schedule the destruction of the bubble and handle the coin after animation finishes
        StartCoroutine(DestroyBubbleAfterAnimation());

        // Allow the laser to continue its path
        stopRay = false;
    }

    private System.Collections.IEnumerator DestroyBubbleAfterAnimation()
    {
        // Wait for the animation to finish
        if (bubbleAnimator != null)
        {
            float animationLength = bubbleAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);
        }

        // After the animation, make the coin fall
        if (coin != null)
        {
            if (coin.TryGetComponent<Rigidbody2D>(out var coinRb))
            {
                coinRb.bodyType = RigidbodyType2D.Dynamic; // Enable physics for falling
            }

            // Wait for 1 second before moving the coin to the UI
            yield return new WaitForSeconds(1f);

            // Disable physics and start moving the coin to the UI
            if (coin.TryGetComponent<Rigidbody2D>(out coinRb))
            {
                coinRb.bodyType = RigidbodyType2D.Kinematic;
                coinRb.linearVelocity = Vector2.zero; // Stop any residual movement

            }

            StartCoroutine(MoveCoinToUI());
        }

        // Destroy the bubble
        Destroy(gameObject);
    }

    private IEnumerator MoveCoinToUI()
{
    if (coin == null || uiCoinCounter == null)
    {
        Debug.LogError("Coin or UI Coin Counter reference is missing!");
        yield break;
    }

    // Get the start position in world space
    Vector3 startPosition = coin.transform.position;

    // Convert UI position to world space
    RectTransform rectTransform = uiCoinCounter as RectTransform;
    Vector3 endPosition;
    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, rectTransform.position, Camera.main, out endPosition))
    {
        endPosition.z = 0; // Ensure the z-coordinate matches your 2D game plane.
    }
    else
    {
        Debug.LogError("Failed to convert UI Coin Counter position to world space!");
        yield break;
    }

    // Log debug information
    Debug.Log($"Moving coin from {startPosition} to {endPosition}");

    // Disable the coin's Rigidbody to avoid physics interference
    if (coin.TryGetComponent<Rigidbody2D>(out var coinRb))
    {
        coinRb.bodyType = RigidbodyType2D.Kinematic;
        coinRb.linearVelocity = Vector2.zero; // Stop any residual movement
    }

    // Smoothly animate the coin moving to the target position
    float duration = 0.5f; // Duration of the animation
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;

        // Interpolate the coin's position
        float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
        coin.transform.position = Vector3.Lerp(startPosition, endPosition, t);

        yield return null;
    }

    // Snap the coin to the final position and destroy it
    coin.transform.position = endPosition;
    Destroy(coin);

    Debug.Log("Coin successfully moved and destroyed.");
}
}
