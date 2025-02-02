using UnityEngine;
using UnityEngine.UI;

public class GuidePanel : MonoBehaviour
{
    [SerializeField] private GameObject guidePanel; // Panel to be destroyed
    [SerializeField] private GameObject newPanel;   // Panel to be shown after destruction

    private void Start()
    {
        // Get the Button component and add a listener to call DestroyPanel when clicked
        GetComponent<Button>().onClick.AddListener(SwapPanels);
    }

    private void SwapPanels()
    {
        if (guidePanel != null)
        {
            Destroy(guidePanel); // Destroy the old panel
        }

        if (newPanel != null)
        {
            newPanel.SetActive(true); // Show the new panel
        }
    }
}
