using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private CollectibleItem collectibleItem;
    private int moveCount = 0;
    private bool isInitialized = false;

    private bool isLoadingScene = false;

    public SaveManager SaveManager { get; private set; }

    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private bool isFirstLoad = true;

    private void Start()
    {
        if (isFirstLoad)
        {
            isFirstLoad = false;
            // Load MainMenu through LoadingManager if we're starting from UIScene
            if (SceneManager.GetActiveScene().name == "UIScene")
            {
                LoadScene(mainMenuSceneName);
            }
        }
    }

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        collectibleItem = new CollectibleItem() ?? new CollectibleItem(); // Ensure it's initialized
        SaveManager = gameObject.AddComponent<SaveManager>();

        await SaveManager.InitializeAsync();
        isInitialized = true;
    }

    public async Task InitializeAfterAuthentication()
    {
        Debug.Log("Reinitializing GameManager after authentication...");
        // Only initialize if not already initialized
        if (!SaveManager.isInitialized)
        {
            await SaveManager.InitializeAsync();
        }
    }

    public void AddScore(int amount, string coinID)
    {
        if (coinID == "tutorial")
        {
            collectibleItem.AddCoin(amount);
            UIManager.Instance?.UpdateScoreUI();
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if coin was already collected
        if (SaveManager.IsCoinAlreadyCollected(coinID))
        {
            Debug.Log($"Coin with ID {coinID} has already been collected in scene {currentSceneName}.");
            return;
        }

        // Update local score
        collectibleItem.AddCoin(amount);

        // Update saved data
        SaveManager.AddCoinToScene(currentSceneName, amount, coinID);

        Debug.Log($"Score updated: {collectibleItem.GetScore()}");
        UIManager.Instance?.UpdateScoreUI();
    }

    public int GetScore()
    {
        return collectibleItem.GetScore();
    }

    // public void IncrementMoveCount()
    // {
    //     moveCount++;
    //     Debug.Log($"Moves made: {moveCount}");
    //     UIManager.Instance?.UpdateMovesUI(moveCount);
    // }

    public void ResetMoveCount()
    {
        moveCount = 0;
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void LoadScene(string sceneName)
    {
        try
        {
            if (collectibleItem == null)
            {
                Debug.LogWarning("collectibleItem is null! Reinitializing...");
                collectibleItem = new CollectibleItem();
            }

            collectibleItem.ResetScore();
            ResetMoveCount();
            Debug.Log($"Loading scene: {sceneName}");
            // Check if LoadingManager exists
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("LoadingManager instance not found!");
                // Fallback to direct scene loading
                SceneManager.LoadScene(sceneName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading scene {sceneName}: {e.Message}");
        }
    }

    public async void LoadSceneMap(string sceneName)
    {
        if (isLoadingScene)
        {
            Debug.Log("Scene load already in progress, ignoring request");
            return;
        }

        isLoadingScene = true;

        try
        {
            if (!await IsReadyToLoad())
            {
                Debug.LogError("GameManager not ready to load scenes");
                return;
            }

            // Safely check coins - default to 0 if SaveManager is somehow null
            int coins = SaveManager?.GetCoinsCollectedInScene(sceneName) ?? 0;
            if (coins == 0)
            {
                Debug.Log($"No coins collected in scene {sceneName}, skipping load");
                return;
            }

            LoadScene(sceneName);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading scene map: {e.Message}");
        }
        finally
        {
            isLoadingScene = false;
        }
    }

    public async Task<bool> IsReadyToLoad()
    {
        if (!isInitialized || SaveManager == null)
        {
            Debug.LogWarning("GameManager not fully initialized. Attempting initialization...");
            try
            {
                if (SaveManager == null)
                {
                    SaveManager = gameObject.AddComponent<SaveManager>();
                }
                await SaveManager.InitializeAsync();
                isInitialized = true;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize GameManager: {e.Message}");
                return false;
            }
        }
        return true;
    }


    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }


    public void RestartCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }

    public void ClearLocalGameState()
    {
        moveCount = 0;

        if (collectibleItem != null)
        {
            collectibleItem.ResetScore();
        }
        else
        {
            collectibleItem = new CollectibleItem();
        }

        SaveManager.ClearSavedData();

        UIManager.Instance?.UpdateScoreUI();
        // UIManager.Instance?.UpdateMovesUI(moveCount);

        Debug.Log("Local game state cleared");
    }

    public async Task<int> GetTotalCoinsCollectedAsync()
    {
        return await SaveManager.GetTotalCoinsCollectedAsync();
    }

    public int GetTotalCoinsCollected()
    {
        return SaveManager.GetTotalCoinsCollected();
    }

   public int GetCoinsCollectedInScene(string sceneName)
    {
        if (SaveManager == null)
        {
            Debug.LogWarning("SaveManager is null when getting coins");
            return 0;
        }
        return SaveManager.GetCoinsCollectedInScene(sceneName);
    }

    public void ReduceTotalCoinsCollected(int amount)
    {
        SaveManager.ReduceTotalCoins(amount);
    }

    public int GetUniqueCoinsInScene(string sceneName)
    {
        return SaveManager.GetUniqueCoinsInScene(sceneName);
    }

    public async Task<int> GetTotalUniqueCoinsAsync()
    {
        return await SaveManager.GetTotalUniqueCoinsAsync();
    }


    public void AddLaserColor(int colorIndex)
    {
        SaveManager.AddLaserColor(colorIndex);
    }

    public void SetCurrentLaserColorIndex(int index)
    {
        SaveManager.SetCurrentLaserColorIndex(index);
    }

    public int GetCurrentLaserColorIndex()
    {
        return SaveManager.GetCurrentLaserColorIndex();
    }

    public List<int> GetUnlockedLaserColors()
    {
        return SaveManager.GetUnlockedLaserColors();
    }

    public bool IsLaserColorUnlocked(int colorIndex)
    {
        return SaveManager.IsLaserColorUnlocked(colorIndex);
    }

}