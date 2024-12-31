using UnityEngine;

public class ballonHit : LaserInteractable
{
    [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";

    // Override the OnLaserHit to add specific behavior and stop laser reflection
    public override void OnLaserHit(ref bool stopRay)
    {
        base.OnLaserHit(ref stopRay); // Perform base behavior (sprite change, etc.)
        CollectibleItem.AddBalloon(1);
        // Log and perform specific behavior
        Debug.Log(uniqueBehaviorMessage);
        // If a new sprite is assigned, change the object's sprite
        // Stop the laser from deflecting further
        stopRay = false;

        PerformCustomBehavior();
    }

    public override void PerformCustomBehavior()
    {
        Destroy(gameObject);
    }
}
