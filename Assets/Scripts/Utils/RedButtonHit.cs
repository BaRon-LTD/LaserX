using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RedButtonHit : LaserInteractable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite hitSprite;
    [SerializeField] private Sprite unHitSprite;
    [SerializeField] private GameObject bulbProtector;

    [SerializeField] private bool isCurrentlyHit = false;
    [SerializeField] private string buttonId;

    // Reference to the LaserController that might be hitting this button
    [SerializeField] private List<LaserController> laserControllers = new List<LaserController>();

    private void Awake()
    {

        // Generate a unique ID if not set in inspector
        if (string.IsNullOrEmpty(buttonId))
        {
            buttonId = System.Guid.NewGuid().ToString();
        }

        // Ensure references are set
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Load saved state when the script starts
        LoadButtonState();
    }

    private void Update()
    {
        // Check if the button is not being hit this frame
        if (isCurrentlyHit && !IsBeingHitByLaser())
        {
            UpdateButtonState(false);
        }
    }

    private bool IsBeingHitByLaser()
    {
        // Check if this object is the current hit object for any laser
        foreach (var laserController in laserControllers)
        {
            if (laserController != null && laserController.GetCurrentHitObject() == this)
            {
                return true;
            }
        }
        return false;
    }

    private void SaveButtonState()
    {
        // Save the current state using PlayerPrefs
        PlayerPrefs.SetInt(buttonId + "_IsHit", isCurrentlyHit ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadButtonState()
    {
        // Load the saved state
        if (PlayerPrefs.HasKey(buttonId + "_IsHit"))
        {
            bool savedState = PlayerPrefs.GetInt(buttonId + "_IsHit") == 1;
            UpdateButtonState(savedState);
        }
        else
        {
            // Initial state if no saved state exists
            UpdateButtonState(false);
        }
    }

    public override void OnLaserHit(ref bool stopRay)
    {
        Debug.Log("Red button hit!");
        UpdateButtonState(true);
        stopRay = true;
    }

    private void UpdateButtonState(bool isHit)
    {
        isCurrentlyHit = isHit;

        // Update sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isHit ? hitSprite : unHitSprite;
        }

        // Toggle bulb protector
        if (bulbProtector != null)
        {
            bulbProtector.SetActive(!isHit);
        }

        // Save the new state
        SaveButtonState();
    }

    // Optional: Method to reset the state completely
    public void ResetButtonState()
    {
        UpdateButtonState(false);
        PlayerPrefs.DeleteKey(buttonId + "_IsHit");
        PlayerPrefs.Save();
    }
}