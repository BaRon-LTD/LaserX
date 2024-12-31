using UnityEngine;

public class bulbHit : LaserInteractable
{
    [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
    [SerializeField] private Sprite hitSprite;
    private SpriteRenderer spriteRenderer;
    [SerializeField] [Tooltip("Name of scene to move to when triggering the given tag")] string sceneName;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnLaserHit(ref bool stopRay)
    {
        base.OnLaserHit(ref stopRay);

        // Log and perform specific behavior
        Debug.Log(uniqueBehaviorMessage);

        if (hitSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = hitSprite;
        }

        stopRay = true;
        Invoke(nameof(PerformCustomBehavior), 1f);
    }

    public override void PerformCustomBehavior()
    {
        // Use GameManager to handle scene loading
        GameManager.Instance.LoadScene(sceneName);
    }
}
