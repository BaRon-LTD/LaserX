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
        if (!PanelManager.IsOpen("loading"))
        {
            PanelManager.CloseAll();
            PanelManager.Open("loading");
        }
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
        if (!PanelManager.IsOpen("loading"))
        {
            PanelManager.CloseAll();
            PanelManager.Open("loading");
        }

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

    public async Task SignInWithUsernameAndPasswordAsync(string username, string password)
    {
        if (!PanelManager.IsOpen("loading"))
        {
            PanelManager.CloseAll();
            PanelManager.Open("loading");
        }

        try
        {
            // Comprehensive authentication state reset
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }

            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);

            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);

            Debug.Log("SignIn successful");
            Debug.Log($"Final Auth State - IsSignedIn: {AuthenticationService.Instance.IsSignedIn}");
            Debug.Log($"Final Auth State - PlayerID: {AuthenticationService.Instance.PlayerId}");

            OnAuthenticated();
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"Authentication Exception: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Username or password is wrong.", "OK");
            throw; // Re-throw to allow caller to handle
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"Request Failed Exception: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
            throw; // Re-throw to allow caller to handle
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected error: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "An unexpected error occurred.", "OK");
            throw; // Re-throw to allow caller to handle
        }
    }

    public async void SignUpWithUsernameAndPasswordAsync(string username, string password)
    {
        if (!PanelManager.IsOpen("loading"))
        {
            PanelManager.CloseAll();
            PanelManager.Open("loading");
        }


        try
        {
            // Comprehensive authentication state reset
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }

            // Validate input 
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                ShowError(ErrorMenu.Action.OpenAuthMenu, "Username must be at least 3 characters long.", "OK");
                return;
            }

            if (password.Length < 6)
            {
                ShowError(ErrorMenu.Action.OpenAuthMenu, "Password must be at least 6 characters long.", "OK");
                return;
            }

            // Attempt signup
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

            // Immediately attempt to sign in
            await SignInWithUsernameAndPasswordAsync(username, password);
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"Signup Authentication Exception: {e.Message}");

            if (e.Message.Contains("username is already in use"))
            {
                ShowError(ErrorMenu.Action.OpenAuthMenu, "Username is already taken. Please choose another username.", "OK");
            }
            else
            {
                ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to sign you up. " + e.Message, "OK");
            }
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"Signup Request Failed Exception: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network. " + e.Message, "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected signup error: {e.Message}");
            ShowError(ErrorMenu.Action.OpenAuthMenu, "An unexpected error occurred during signup. " + e.Message, "OK");
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
            if (totalCoins > 0)
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
            if (totalCoins > 0)
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