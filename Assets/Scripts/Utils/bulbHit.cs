using UnityEngine;

public class bulbHit : LaserInteractable
{
    [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
    [SerializeField] private Sprite hitSprite;
    private SpriteRenderer spriteRenderer;

    [SerializeField] float durationHold = 0.5f;

    [SerializeField] [Tooltip("Name of scene to move to when triggering the given tag")] string sceneName;

    private void Awake()
    {
        LaserInteractable.duration = durationHold;
        spriteRenderer = GetComponent<SpriteRenderer>();
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