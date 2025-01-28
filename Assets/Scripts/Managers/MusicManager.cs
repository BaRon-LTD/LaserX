using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    [SerializeField] private AudioClip backgroundMusic; // Assign your music clip in the Inspector
    private AudioSource audioSource;

    private void Awake()
    {
        // Ensure only one instance of MusicManager exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate MusicManager
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Persist this GameObject across scenes

        // Configure the AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f; // Adjust volume as needed

        audioSource.Play(); // Start playing the background music
    }

    // Optional: Method to stop or change music
    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void ChangeMusic(AudioClip newClip)
    {
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();
    }
}
