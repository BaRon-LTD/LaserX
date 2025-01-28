using UnityEngine;

public class SettingsMenu : Panel
{
    public override void Open()
    {
        if (PanelManager.IsOpen("loading"))
        {
            PanelManager.Close("loading");

            base.Open();
            PanelManager.Open("loading");
        }
        PanelManager.Close("loading");
        PanelManager.Close("auth");
        PanelManager.Close("error");
        PanelManager.Close("main");

        base.Open();
    }
    
}
