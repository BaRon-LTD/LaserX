using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        // Try to get the Button component and add the listener
        if (TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(PlaySound);
        }
        else
        {
            Debug.LogWarning("ButtonSound script requires a Button component!", this);
        }
    }

    private void PlaySound()
    {
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
        }
        else
        {
            Debug.LogWarning("No click sound assigned to ButtonSound script!", this);
        }
    }
}
