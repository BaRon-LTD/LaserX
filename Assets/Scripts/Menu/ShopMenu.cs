using UnityEngine;
using TMPro;

public class ShopMenu : Panel
{
    [SerializeField] private TextMeshProUGUI coinsCounter;
    [SerializeField] private AudioClip menuOpenSound; // Sound to play when opening
    private AudioSource audioSource;

    private void Start()
    {
        InitializeAudio(); // Set up the AudioSource
    }

    private void InitializeAudio()
    {
        // Get or add an AudioSource component
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update coins counter and play sound when opening the panel
    public override void Open()
    {
        if (coinsCounter != null)
        {
            coinsCounter.text = "     X " + GameManager.Instance.GetTotalCoinsCollected();
        }

        PlaySound(); // Play sound when opening the menu
        base.Open();
    }

    private void PlaySound()
    {
        if (menuOpenSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(menuOpenSound);
        }
    }
}
