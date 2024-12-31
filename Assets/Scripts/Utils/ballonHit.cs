using UnityEngine;

public class ballonHit : LaserInteractable
{
    [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";

    public override void OnLaserHit(ref bool stopRay)
    {
        base.OnLaserHit(ref stopRay);

        // Update the score through the GameManager
        GameManager.Instance.AddScore(1);

        // Log and perform specific behavior
        Debug.Log(uniqueBehaviorMessage);

        stopRay = false;
        PerformCustomBehavior();
    }

    public override void PerformCustomBehavior()
    {
        Destroy(gameObject);
    }
}
