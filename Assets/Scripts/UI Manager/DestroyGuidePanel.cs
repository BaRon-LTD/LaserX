using UnityEngine;

public class DestroyGuidePanel : MonoBehaviour
{
    [SerializeField] private float destroyTime = 5f; // Time before destruction

    private void Start()
    {
        // Destroy the GameObject after 'destroyTime' seconds
        Destroy(gameObject, destroyTime);
    }
}
