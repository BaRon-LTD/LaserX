using UnityEngine;

public class MirrorMenu_Open : Panel
{
    // Array or list to store the mirrors that are children of the container's child
    [SerializeField] private GameObject[] mirrors = null;

    // Reference to the Bar GameObject's RectTransform
    [SerializeField] private RectTransform barRectTransform = null;

    // Reference to the container (which is a RectTransform for UI purposes)
    [SerializeField] private RectTransform container = null;

    public override void PostInitialize()
    {
        // Don't close the menu on initialization
    }

    public override void Close()
    {
        CheckMirrorsPosition();
        base.Close();
    }

    public override void Open()
    {
        CheckMirrorsPosition();
        base.Open();
    }

    private void CheckMirrorsPosition()
    {
        foreach (var mirror in mirrors)
        {
            // Get the original parent of the mirror to restore later if needed
            Transform originalParent = mirror.transform.parent;

            // Check if the mirror's position is inside the Bar
            if (IsMirrorInsideBar(mirror))
            {
                // If mirror is inside the Bar, reparent it to the container
                Debug.Log($"{mirror.name} is inside the Bar. Moving it to the container.");
                mirror.transform.SetParent(container.transform); // Reparent mirror to container
            }
            else
            {
                // If mirror is outside the Bar, reparent it to this GameObject
                Debug.Log($"{mirror.name} is outside the Bar. Keeping it under current GameObject.");
                mirror.transform.SetParent(this.transform); // Keep mirror under current GameObject
            }
        }
    }

    private bool IsMirrorInsideBar(GameObject mirror)
    {
        // Get the mirror's position
        Vector2 mirrorPosition = mirror.transform.position;

        // Check if the mirror's position is inside the Bar's RectTransform bounds
        if (barRectTransform != null)
        {
            // Convert the mirror's position to local space relative to the Bar's RectTransform
            Vector2 localMirrorPosition = barRectTransform.InverseTransformPoint(mirrorPosition);

            // Check if the local position is inside the Bar's RectTransform
            return barRectTransform.rect.Contains(localMirrorPosition);
        }

        return false; // Return false if no RectTransform is provided
    }
}
