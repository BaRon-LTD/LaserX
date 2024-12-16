using UnityEngine;
using UnityEngine.SceneManagement;

public class bulbHit : LaserInteractable {
   [SerializeField] private string uniqueBehaviorMessage = "Specific behavior triggered!";
   [SerializeField] private Sprite hitSprite; // Optional: New sprite to display when hit
   [SerializeField] private SpriteRenderer spriteRenderer;

   [SerializeField] [Tooltip("Name of scene to move to when triggering the given tag")] string sceneName;

    
   private void Awake() {
      spriteRenderer = GetComponent<SpriteRenderer>();
   }

   // Override the OnLaserHit to add specific behavior and stop laser reflection
   public override void OnLaserHit(ref bool stopRay) {
      base.OnLaserHit(ref stopRay); // Perform base behavior (sprite change, etc.)

      // Log and perform specific behavior
      Debug.Log(uniqueBehaviorMessage);
          // If a new sprite is assigned, change the object's sprite
      // If a new sprite is assigned, change the object's sprite
      if (hitSprite != null && spriteRenderer != null) {
         spriteRenderer.sprite = hitSprite;
      }
      // Stop the laser from deflecting further
      stopRay = true;
   }

   public override void PerformCustomBehavior()
   {
      SceneManager.LoadScene(sceneName);    // Input can either be a serial number or a name; here we use name.
   }
}
