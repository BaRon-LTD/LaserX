using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private CollectibleItem collectibleItem;
    private int moveCount = 0;

    public SaveManager SaveManager { get; private set; }

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        collectibleItem = new CollectibleItem();
        SaveManager = gameObject.AddComponent<SaveManager>();
        
        await SaveManager.InitializeAsync();
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

    public void IncrementMoveCount()
    {
        moveCount++;
        Debug.Log($"Moves made: {moveCount}");
        UIManager.Instance?.UpdateMovesUI(moveCount);
    }

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
        collectibleItem.ResetScore();
        ResetMoveCount();
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
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
        UIManager.Instance?.UpdateMovesUI(moveCount);
        
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
        return SaveManager.GetCoinsCollectedInScene(sceneName);
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