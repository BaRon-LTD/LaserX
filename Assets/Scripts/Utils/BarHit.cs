using UnityEngine;

public class BarHit : LaserInteractable
{
    public override void OnLaserHit(ref bool stopRay)
    {
        // return;
        stopRay = true;
    }

}
