using UnityEngine;

public class LaserInteractable : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float duration = 0f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void OnLaserHit(ref bool stopRay)
    {
        // Log hit for debugging
        Debug.Log($"{gameObject.name} was hit by the laser!");
        // Schedule the additional behavior to run after 1 seconds
        Invoke(nameof(PerformCustomBehavior), 20);
    }

    public virtual void PerformCustomBehavior()
    {
        // Example: Destroy the object after a delay
        // Destroy(gameObject, 2f);
        // You can add any behavior here specific to this object
        // For example: trigger animations, change score, etc.
    }
}
