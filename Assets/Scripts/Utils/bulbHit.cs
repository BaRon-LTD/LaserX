using Unity.VisualScripting;
using UnityEngine;

public class bulbHit : LaserInteractable
{
    [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
    [SerializeField] private Sprite hitSprite;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float durationHold = 1f;

    [SerializeField][Tooltip("Name of scene to move to when triggering the given tag")] string sceneName;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        duration = durationHold; // Set instance-specific duration
    }

    public override void OnLaserHit(ref bool stopRay)
    {
        base.OnLaserHit(ref stopRay);
        stopRay = true;
        PerformCustomBehavior();
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