using UnityEngine;

public class CustomButton : MonoBehaviour
{
    [SerializeField] private Panel panelToOpen;  // Panel to open when clicked
    [SerializeField] private Panel panelToClose; // Panel to close when clicked

    // Called when the button is clicked
    private void OnMouseDown()
    {
        // Open the panel to open and close the panel to close
        if (panelToOpen != null)
        {
            panelToOpen.Open();
        }

        if (panelToClose != null)
        {
            panelToClose.Close();
        }
    }
}