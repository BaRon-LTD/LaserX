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
    GameManager gameManager;
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
        gameManager = GameManager.Instance;
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

            // Always show auth panel for first-time players
            PanelManager.CloseAll();
            PanelManager.Open("auth");
        }
        catch (Exception)
        {
            ShowError(ErrorMenu.Action.StartService, "Failed to connect to the network.", "Retry");
        }
    }

    public async Task SignInAnonymouslyAsync()
    {
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        try
        {

            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Already signed in, skipping sign-in process.");
                OnAuthenticated(); // Proceed with the authentication flow
                return;
            }

            AuthenticationService.Instance.ClearSessionToken();
            GameManager.Instance.ClearLocalGameState();
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await AuthenticationService.Instance.UpdatePlayerNameAsync("Guest");

            OnAuthenticated(); // Call reinitialization after successful sign-in
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"Authentication Exception: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to sign in.", "OK");
            PanelManager.CloseAll();
            PanelManager.Open("loading");
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"Request Failed Exception: {e.Message}");
            ShowError(ErrorMenu.Action.SignIn, "Failed to connect to the network.", "Retry");
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
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            // Update Player Name
            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
            OnAuthenticated(); // Call reinitialization after successful sign-in
        }
        catch (AuthenticationException)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Username or password is wrong.", "OK");
        }
        catch (RequestFailedException)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
        }
    }
    
    public async void SignUpWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.CloseAll();
        PanelManager.Open("loading");
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
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
        }
    }
    
    public void SignOut()
    {
        // Clear local game state before signing out
        GameManager.Instance.ClearLocalGameState();
        AuthenticationService.Instance.SignOut();
        PanelManager.CloseAll();
        PanelManager.Open("auth");
    }
    
    private void SetupEvents()
    {
        eventsInitialized = true;
        // Clear any existing state before processing new sign in
        GameManager.Instance.ClearLocalGameState();

        AuthenticationService.Instance.SignedIn += async () =>
        {
            await SignInConfirmAsync();
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            // Clear local state on sign out
            GameManager.Instance.ClearLocalGameState();
            PanelManager.CloseAll();
            PanelManager.Open("auth");
        };
        
        AuthenticationService.Instance.Expired += async() =>
        {
            // Clear local state before re-authenticating
            GameManager.Instance.ClearLocalGameState();
            await SignInAnonymouslyAsync();
        };
    }

    private async void OnAuthenticated()
    {
        try
        {
            // Clear any existing state before initializing
            GameManager.Instance.ClearLocalGameState();

            if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("Guest");
            }

            // Call GameManager's initialization after sign-in
            await GameManager.Instance.InitializeAfterAuthentication();

            PanelManager.CloseAll();

            int totalCoins = await GameManager.Instance.GetTotalCoinsCollectedAsync();
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
        ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
        panel.Open(action, error, button);
    }
    
    private async Task SignInConfirmAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("Player");
            }

            // Wait for GameManager's initialization to complete
            await GameManager.Instance.InitializeAfterAuthentication();

            // Add loading panel while we wait for data
            PanelManager.CloseAll();
            PanelManager.Open("loading");

            int totalCoins = await GameManager.Instance.GetTotalCoinsCollectedAsync();

            Debug.Log($"Total coins loaded: {totalCoins}"); // Debug log to verify

            if(totalCoins > 0)
            {
                PanelManager.Open("main_register");
            }
            else
            {
                PanelManager.Open("main");
            }
        }
        catch
        {
            
        }
    }
    
    
}