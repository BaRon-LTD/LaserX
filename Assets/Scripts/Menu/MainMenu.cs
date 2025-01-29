using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine.UI;

public class MainMenu : Panel
{
    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button logoutButton = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        logoutButton.onClick.AddListener(SignOut);
        base.Initialize();
    }
    
    public override void Open()
    {
        UpdatePlayerNameUI();
        base.Open();
    }
    
    private void SignOut()
    {
        MenuManager.Singleton.SignOut();
    }
    
    private void UpdatePlayerNameUI()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            string playerName = AuthenticationService.Instance.PlayerName;
            
            // Remove #1234 part if it exists
            int hashIndex = playerName.IndexOf("#");
            if (hashIndex != -1)
            {
                playerName = playerName.Substring(0, hashIndex);
            }

            nameText.text = string.IsNullOrEmpty(playerName) ? "Guest" : playerName;
        }
        else
        {
            nameText.text = "Guest";
        }
    }



    
}