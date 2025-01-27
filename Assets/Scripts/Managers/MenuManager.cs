using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.Scripting;
using System.Threading.Tasks;  // Add this for Task support

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

    private void Awake()
    {
        Application.runInBackground = true;
        StartClientService();
    }
    private void Start() {
        gameManager = GameManager.Instance;
    }

    [Preserve]
    public void PlayGame()
    {
        gameManager.LoadScene("tutorial_1");
    }

    public async void StartClientService()
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

            if (AuthenticationService.Instance.SessionTokenExists)
            {
                SignInAnonymouslyAsync();
            }
            else
            {
                PanelManager.Open("auth");
            }
        }
        catch (Exception exception)
        {
            ShowError(ErrorMenu.Action.StartService, "Failed to connect to the network.", "Retry");
        }

        // Force reinitialization
        await ReinitializeServices();
    }

    private async Task ReinitializeServices()
    {
        try
        {
            var options = new InitializationOptions();
            options.SetProfile($"profile_{System.Guid.NewGuid()}"); // Create new profile
            await UnityServices.InitializeAsync(options);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to reinitialize services: {e.Message}");
        }
    }

    public async void SignInAnonymouslyAsync()
    {
        PanelManager.Open("loading");
        try
        {
            // Ensure we have a clean state before anonymous sign-in
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                AuthenticationService.Instance.SignOut();
                AuthenticationService.Instance.ClearSessionToken();
                await ReinitializeServices();
            }
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerPrefs.SetInt("IsAnonymousUser", 1);
            PlayerPrefs.Save();
        }
        catch (AuthenticationException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to sign in.", "OK");
        }
        catch (RequestFailedException exception)
        {
            ShowError(ErrorMenu.Action.SignIn, "Failed to connect to the network.", "Retry");
        }
    }
    
    public async void SignInWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.Open("loading");
        try
        {
            // Ensure we have a clean state before username/password sign-in
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                AuthenticationService.Instance.SignOut();
                AuthenticationService.Instance.ClearSessionToken();
                await ReinitializeServices();
            }
            
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            PlayerPrefs.SetInt("IsAnonymousUser", 0);
            PlayerPrefs.Save();
        }
        catch (AuthenticationException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Username or password is wrong.", "OK");
        }
        catch (RequestFailedException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
        }
    }
    
    public async void SignUpWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.Open("loading");
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to sign you up.", "OK");
        }
        catch (RequestFailedException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
        }
    }
    
    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        // Explicitly clear the session token
        AuthenticationService.Instance.ClearSessionToken();
        PlayerPrefs.DeleteKey("IsAnonymousUser");
        PlayerPrefs.Save();


        PanelManager.CloseAll();
        PanelManager.Open("auth");
        
    }
    
    private void SetupEvents()
    {
        eventsInitialized = true;
        AuthenticationService.Instance.SignedIn += () =>
        {
            SignInConfirmAsync();
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            PanelManager.CloseAll();
            PanelManager.Open("auth");
        };
        
        AuthenticationService.Instance.Expired += () =>
        {
            SignInAnonymouslyAsync();
        };
    }
    
    private void ShowError(ErrorMenu.Action action = ErrorMenu.Action.None, string error = "", string button = "")
    {
        PanelManager.Close("loading");
        ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
        panel.Open(action, error, button);
    }
    
    private async void SignInConfirmAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("Player");
            }
            PanelManager.CloseAll();
            PanelManager.Open("main");
        }
        catch
        {
            
        }
    }
    
    
}