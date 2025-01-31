using UnityEngine;

public class MirrorMenu_Open : Panel
{
    [SerializeField] private GameObject[] mirrors = null;
    [SerializeField] private RectTransform barRectTransform = null;

    // 🎵 Add AudioSource and AudioClip references
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioClip openSound = null;
    [SerializeField] private AudioClip closeSound = null;

    public override void PostInitialize()
    {
        // Don't close the menu on initialization
    }

    public override void Close()
    {
        PlaySound(closeSound); // 🔊 Play close sound
        CheckMirrorsPosition();
        base.Close();
    }

    public override void Open()
    {
        PlaySound(openSound); // 🔊 Play open sound
        CheckMirrorsPosition();
        base.Open();
    }

    private void CheckMirrorsPosition()
    {
        foreach (var mirror in mirrors)
        {
            Transform originalParent = mirror.transform.parent;
            if (IsMirrorInsideBar(mirror))
            {
                Debug.Log($"{mirror.name} is inside the Bar. Moving it to the container.");
                mirror.transform.SetParent(container.transform);
            }
            else
            {
                Debug.Log($"{mirror.name} is outside the Bar. Keeping it under current GameObject.");
                mirror.transform.SetParent(this.transform);
            }
        }
    }

    private bool IsMirrorInsideBar(GameObject mirror)
    {
        Vector2 mirrorPosition = mirror.transform.position;
        if (barRectTransform != null)
        {
            Vector2 localMirrorPosition = barRectTransform.InverseTransformPoint(mirrorPosition);
            return barRectTransform.rect.Contains(localMirrorPosition);
        }
        return false;
    }

    // 🔊 Play sound helper method
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
