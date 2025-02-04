using UnityEngine;

public class SettingsMenu : Panel
{
    [SerializeField] private AudioClip menuOpenSound; // Sound to play
    private AudioSource audioSource;

    private void Start()
    {
        InitializeAudio(); // Call our custom initialization method
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

    public override void Open()
    {
        base.Open();
        PlaySound(); // Play sound when opening the menu
    }

    public override void Close()
    {
        base.Close();
    }

    private void PlaySound()
    {
        if (menuOpenSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(menuOpenSound);
        }
    }
}
