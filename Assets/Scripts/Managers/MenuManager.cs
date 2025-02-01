using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.Scripting;
using System.Threading.Tasks;

public class MenuManager : MonoBehaviour
{
    private GameManager gameManager;
    private SaveManager saveManager;
    private bool initialized = false;
    private bool eventsInitialized = false;
    
    private static MenuManager singleton = null;

    public static MenuManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindFirstObjectByType<MenuManager>();
                singleton.Initialize();
            }
            return singleton; 
        }
    }

    private void Initialize()
    {
        if (initialized) { return; }
        initialized = true;
    }
    
    private void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }

    private async void Awake()
    {
        // Get GameManager instance and its SaveManager component
        gameManager = GameManager.Instance;
        saveManager = gameManager.SaveManager;
        
        Application.runInBackground = true;
        await StartClientService();
    }

    [Preserve]
    public void PlayGame()
    {
        gameManager.LoadScene("tutorial_1");
    }

    public void PlayGame_Advance()
    {
        gameManager.LoadScene("level1");
    }

    public async Task StartClientService()
    {
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        Debug.Log("1");
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                var options = new InitializationOptions();
                options.SetProfile("default_profile");
                await UnityServices.InitializeAsync();
            }
            
            if (!eventsInitialized)
            {
                SetupEvents();
            }

            if (AuthenticationService.Instance.SessionTokenExists)
            {
                Debug.Log("Session token exists: " + AuthenticationService.Instance.SessionTokenExists);
            }
            else
            {
                Debug.Log("Session token doesn't exist: " + AuthenticationService.Instance.SessionTokenExists);
                PanelManager.CloseAll();
                PanelManager.Open("auth");
            }
        }
        catch (Exception)
        {
            Debug.Log("error connect to the network1");
            ShowError(ErrorMenu.Action.StartService, "Failed to connect to the network. - 1", "Retry");
        }
    }

    public async Task SignInAnonymouslyAsync()
    {
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        Debug.Log("2");
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Already signed in, skipping sign-in process.");
                OnAuthenticated();
                return;
            }

            AuthenticationService.Instance.ClearSessionToken();
            gameManager.ClearLocalGameState();
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await AuthenticationService.Instance.UpdatePlayerNameAsync("Guest");

            OnAuthenticated();
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"Authentication Exception: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to sign in.", "OK");
            PanelManager.CloseAll();
            PanelManager.Open("loading");
            Debug.Log("3");
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"Request Failed Exception: {e.Message}");
            ShowError(ErrorMenu.Action.SignIn, "Failed to connect to the network. - 2", "Retry");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected Exception: {e.Message}");
            ShowError(ErrorMenu.Action.SignIn, "An unexpected error occurred.", "Retry");
        }
    }
    
    public async void SignInWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        Debug.Log("4");
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
            OnAuthenticated();
        }
        catch (AuthenticationException)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Username or password is wrong.", "OK");
        }
        catch (RequestFailedException)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network. - 3", "OK");
        }
    }
    
    public async void SignUpWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        Debug.Log("5");
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to sign you up.", "OK");
        }
        catch (RequestFailedException)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network. - 4", "OK");
        }
    }
    
    public void SignOut()
    {
        gameManager.ClearLocalGameState();
        AuthenticationService.Instance.SignOut();
        PanelManager.CloseAll();
        PanelManager.Open("auth");
    }
    
    private void SetupEvents()
    {
        eventsInitialized = true;
        gameManager.ClearLocalGameState();

        AuthenticationService.Instance.SignedIn += async () =>
        {
            await SignInConfirmAsync();
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            gameManager.ClearLocalGameState();
            PanelManager.CloseAll();
            PanelManager.Open("auth");
        };
        
        AuthenticationService.Instance.Expired += async() =>
        {
            gameManager.ClearLocalGameState();
            await SignInAnonymouslyAsync();
        };
    }

    private async void OnAuthenticated()
    {
        try
        {
            gameManager.ClearLocalGameState();

            if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("Guest");
            }

            await gameManager.InitializeAfterAuthentication();

            int totalCoins = await gameManager.GetTotalCoinsCollectedAsync();

            PanelManager.CloseAll();
            if(totalCoins > 0)
            {
                PanelManager.Open("main_register");
            }
            else
            {
                PanelManager.Open("main");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to complete post-authentication steps: {e.Message}");
        }
    }
    
    private void ShowError(ErrorMenu.Action action = ErrorMenu.Action.None, string error = "", string button = "")
    {
        PanelManager.Close("loading");
        Debug.Log("6");
        ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
        panel.Open(action, error, button);
    }
    
    private async Task SignInConfirmAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("Guest");
            }

            await gameManager.InitializeAfterAuthentication();

            int totalCoins = await gameManager.GetTotalCoinsCollectedAsync();

            Debug.Log($"Total coins loaded: {totalCoins}");

            PanelManager.CloseAll();
            if(totalCoins > 0)
            {
                PanelManager.Open("main_register");
            }
            else
            {
                PanelManager.Open("main");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to complete sign-in confirmation: {e.Message}");
        }
    }
}