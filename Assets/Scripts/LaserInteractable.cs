using UnityEngine;

public class LaserInteractable : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;

    private void Start() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void OnLaserHit(ref bool stopRay) 
    {
        // Log hit for debugging
        Debug.Log($"{gameObject.name} was hit by the laser!");

        // Schedule the additional behavior to run after 2 seconds
        Invoke(nameof(PerformCustomBehavior), 2f);
    }

    public virtual void PerformCustomBehavior() 
    {
        // Example: Destroy the object after a delay
        // Destroy(gameObject, 2f);

        // You can add any behavior here specific to this object
        // For example: trigger animations, change score, etc.
    }
}
