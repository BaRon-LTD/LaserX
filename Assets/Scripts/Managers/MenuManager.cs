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
        Debug.Log("Starting Client Service");
        
        try
        {
            // Initialize Unity Services if needed
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.Log("Initializing Unity Services...");
                var options = new InitializationOptions();
                options.SetProfile("default_profile");
                await UnityServices.InitializeAsync();
                Debug.Log("Unity Services Initialized - ClientSerivce");
            }
            
            // Setup events if not already done
            if (!eventsInitialized)
            {
                Debug.Log("Setting up events for the first time");
                SetupEvents();
            }

            // Remove the automatic session restore attempt
            // Show auth menu directly for fresh start
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Not signed in, showing auth menu");
                PanelManager.CloseAll();
                PanelManager.Open("auth");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"StartClientService error: {e.Message}");
            ShowError(ErrorMenu.Action.StartService, "Failed to connect to the network. - 1", "Retry");
        }
    }

    public async Task SignInAnonymouslyAsync()
    {
        Debug.Log("Starting Anonymous Sign In Process");
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Already signed in, skipping anonymous sign-in process");
                OnAuthenticated();
                return;
            }

            Debug.Log("Proceeding with anonymous sign in");
            AuthenticationService.Instance.ClearSessionToken();
            gameManager.ClearLocalGameState();
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await AuthenticationService.Instance.UpdatePlayerNameAsync("Guest");
            Debug.Log("Anonymous sign in completed successfully");

            OnAuthenticated();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in SignInAnonymouslyAsync: {e.Message}");
            ShowError(ErrorMenu.Action.SignIn, "Failed to sign in anonymously.", "Retry");
        }
    }
    
    public async void SignInWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        
        try
        {
            // First try clearing everything
            AuthenticationService.Instance.SignOut();
            AuthenticationService.Instance.ClearSessionToken();
            gameManager.ClearLocalGameState();
            
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn successful");
            
            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
            Debug.Log("Player name updated successfully");
            
            Debug.Log($"Final Auth State - IsSignedIn: {AuthenticationService.Instance.IsSignedIn}");
            Debug.Log($"Final Auth State - PlayerID: {AuthenticationService.Instance.PlayerId}");
            
            OnAuthenticated();
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"Authentication Exception: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Username or password is wrong.", "OK");
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"Request Failed Exception: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network. - 3", "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected error: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "An unexpected error occurred.", "OK");
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
        Debug.Log("Setting up Authentication Events");
        eventsInitialized = true;
        gameManager.ClearLocalGameState();

        AuthenticationService.Instance.SignedIn += async () =>
        {
            Debug.Log("SignedIn Event Triggered");
            Debug.Log($"Player Name after sign in: {AuthenticationService.Instance.PlayerName}");
            
            // Only proceed if this was triggered by an explicit sign-in action
            if (!string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                await SignInConfirmAsync();
            }
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("SignedOut Event Triggered");
            AuthenticationService.Instance.ClearSessionToken();
            gameManager.ClearLocalGameState();
            if (!AuthenticationService.Instance.IsSignedIn && !PanelManager.IsOpen("loading"))
            {
                PanelManager.CloseAll();
                PanelManager.Open("auth");
            }
        };
        
        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Session Expired Event Triggered");
            gameManager.ClearLocalGameState();
            PanelManager.CloseAll();
            PanelManager.Open("auth");
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
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Not signed in during confirmation, showing auth menu");
                PanelManager.CloseAll();
                PanelManager.Open("auth");
                return;
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
            Debug.LogError($"SignInConfirmAsync error: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to complete sign-in.", "OK");
        }
    }
}