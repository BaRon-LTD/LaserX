using System.Collections;
using UnityEngine;

public class ballonHit : LaserInteractable
{
    [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
    [SerializeField] private Animator bubbleAnimator; // Reference to the Animator component
    [SerializeField] private GameObject coin; // Reference to the coin inside the bubble
    [SerializeField] private float destroyDelay = 1f; // Delay before destroying the bubble (adjust as needed)
    [SerializeField] private RectTransform uiCounterPosition; // Reference to the UI counter position (for top-left corner)
    [SerializeField] private float moveSpeed = 5f; // Speed at which the coin moves towards the UI counter

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

        // Schedule the destruction of the bubble and handle the coin after animation finishes
        StartCoroutine(DestroyBubbleAfterAnimation());

        // Allow the laser to continue its path
        stopRay = false;
    }

    private IEnumerator DestroyBubbleAfterAnimation()
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

            gameObject.GetComponent<Collider2D>().enabled = false; // Disable the collider to prevent further interactions
            
            // Wait for 1 second before starting to move the coin
            yield return new WaitForSeconds(1f);

            // Disable physics and detach the coin from the bubble
            if (coin.TryGetComponent<Rigidbody2D>(out var _))
            {
                coinRb.bodyType = RigidbodyType2D.Kinematic;
                coinRb.linearVelocity = Vector2.zero; // Stop any residual movement
            }

            // Detach the coin from the bubble to prevent it from being destroyed with the bubble
            coin.transform.SetParent(null); // Remove the coin from the bubble's hierarchy

            // Move the coin towards the top-left UI position
            yield return StartCoroutine(MoveCoinToUI());
        }

        // Destroy the bubble after the coin has finished moving
        Destroy(gameObject);

    }

    private IEnumerator MoveCoinToUI()
    {
        // Convert the UI position to world space
        Vector3 targetWorldPosition = uiCounterPosition.position;

        // Convert the target world position from screen space to world space
        Vector3 worldTargetPosition = Camera.main.ScreenToWorldPoint(targetWorldPosition);
        worldTargetPosition.z = 0; // Ensure the coin stays on the 2D plane (in case there’s a z offset)

        // While the coin hasn't reached the target, move it
        while (Vector3.Distance(coin.transform.position, worldTargetPosition) > 0.1f)
        {
            coin.transform.position = Vector3.MoveTowards(coin.transform.position, worldTargetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Once the coin reaches the target position, you can stop or add other actions (e.g., update the UI)
        coin.transform.position = worldTargetPosition; // Ensure the coin exactly reaches the target

        // Call the CollectCoin method from the Coin script
        if (coin.TryGetComponent<Coin>(out var coinScript))
        {
            coinScript.CollectCoin(); // Notify the GameManager and destroy the coin
        }
        else
        {
            Debug.LogWarning("Coin script not found on the coin object!");
            Destroy(coin); // Fallback: destroy the coin
        }
    }

}
