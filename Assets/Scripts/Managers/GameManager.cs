using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private CollectibleItem collectibleItem;

    private int moveCount = 0; // Counter for the number of moves
    
    private int totalCoins = 0; // per player

    // Dictionary to track coin data per scene
    private Dictionary<string, SceneCoinData> coinsCollectedData;
    private const string SAVE_KEY = "player_coins_data";
    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        collectibleItem = new CollectibleItem();
        coinsCollectedData = new Dictionary<string, SceneCoinData>(); // Initialize the dictionary

        // Initialize data loading
        await InitializeAsync();
    }

    // Call this after authentication is complete
    public async Task InitializeAfterAuthentication()
    {
        Debug.Log("Reinitializing GameManager after authentication...");
        await InitializeAsync(); // Call the async initialization logic
    }

    // Separate async initialization method
    private async Task InitializeAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            // Sign in anonymously if not already authenticated
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Load cloud data after authentication
            await LoadCloudData();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
        }
    }

     // Save data to Cloud Save
    public async Task SaveToCloud()
    {
        try
        {
            string serializedData = SerializeCoinsData();
            Debug.Log($"Attempting to save data: {serializedData}"); // For debugging

            var data = new Dictionary<string, object>
            {
                { SAVE_KEY, serializedData }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log("Game data saved to cloud successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game data: {e.Message}");
        }
    }

    public async Task LoadCloudData()
    {
        try
        {
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { SAVE_KEY });

            if (data != null && data.TryGetValue(SAVE_KEY, out var savedItem))
            {
                var savedData = savedItem.Value.GetAsString();
                if (!string.IsNullOrEmpty(savedData))
                {
                    Debug.Log($"Loaded raw data from cloud: {savedData}"); // For debugging
                    DeserializeCoinsData(savedData);
                }
                else
                {
                    totalCoins = 0;
                    Debug.Log("No saved data found in cloud - starting fresh");
                }
            }
            else
            {
                Debug.Log("No saved data found in cloud - starting fresh");
            }
            
            UIManager.Instance?.UpdateScoreUI();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game data: {e.Message}");
            Debug.Log("Starting with fresh game data due to load error");
            throw; // Rethrow to handle in calling code
        }
    }

    private string SerializeCoinsData()
    {
        try
        {
            var wrapper = new SceneCoinDataWrapper();
            wrapper.Entries = coinsCollectedData.Select(kvp => new SceneDataEntry
            {
                SceneName = kvp.Key,
                Data = new SerializableSceneCoinData
                {
                    CoinsCollected = kvp.Value.CoinsCollected,
                    CollectedCoinIDs = kvp.Value.CollectedCoinIDs ?? new List<string>()
                }
            }).ToList();

            string json = JsonUtility.ToJson(wrapper);
            Debug.Log($"Serialized data: {json}"); // For debugging
            return json;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error serializing save data: {e.Message}");
            return "";
        }
    }

    private void DeserializeCoinsData(string jsonData)
    {
        try
        {
            Debug.Log($"Attempting to deserialize: {jsonData}"); // For debugging

            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.Log("Empty save data received");
                return;
            }

            var wrapper = JsonUtility.FromJson<SceneCoinDataWrapper>(jsonData);
            if (wrapper != null && wrapper.Entries != null)
            {
                coinsCollectedData.Clear();
                foreach (var entry in wrapper.Entries)
                {
                    if (entry.Data != null)
                    {
                        coinsCollectedData[entry.SceneName] = new SceneCoinData
                        {
                            CoinsCollected = entry.Data.CoinsCollected,
                            CollectedCoinIDs = entry.Data.CollectedCoinIDs ?? new List<string>()
                        };
                    }
                }
                Debug.Log($"Successfully deserialized {wrapper.Entries.Count} scenes");
            }
            else
            {
                Debug.Log("Could not deserialize save data - starting fresh");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deserializing save data: {e.Message}");
            coinsCollectedData.Clear();
        }
    }

    public void AddScore(int amount, string coinID)
    {
        if(coinID == "tutorial") //tutorial scence coin
        {
            // Update the total score
            collectibleItem.AddCoin(amount);
            UIManager.Instance?.UpdateScoreUI();
            return;
        }
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Ensure the current scene has an entry in the dictionary
        if (!coinsCollectedData.ContainsKey(currentSceneName))
        {
            coinsCollectedData[currentSceneName] = new SceneCoinData();
        }

        SceneCoinData sceneData = coinsCollectedData[currentSceneName];

        // Check if the coin ID has already been collected
        if (sceneData.CollectedCoinIDs.Contains(coinID))
        {
            Debug.Log($"Coin with ID {coinID} has already been collected in scene {currentSceneName}.");
            return; // Exit if the coin has already been collected
        }


        // Add the coin ID to the list and update the coin count
        sceneData.CollectedCoinIDs.Add(coinID);
        sceneData.CoinsCollected += amount;

        // Update the total score
        collectibleItem.AddCoin(amount);

        Debug.Log($"Score updated: {collectibleItem.GetScore()}");
        Debug.Log($"Coins collected in {currentSceneName}: {sceneData.CoinsCollected}");
        Debug.Log($"Collected Coin IDs in {currentSceneName}: {string.Join(", ", sceneData.CollectedCoinIDs)}");

        UIManager.Instance?.UpdateScoreUI();

        // Save to cloud after collecting coins
        SaveAsync();
    }

    // Helper method to handle async save operation
    private async void SaveAsync()
    {
        await SaveToCloud();
    }

    public int GetScore()
    {
        return collectibleItem.GetScore();
    }

    public int GetCoinsCollectedInScene(string sceneName)
    {
        if (coinsCollectedData.ContainsKey(sceneName))
        {
            return coinsCollectedData[sceneName].CoinsCollected;
        }
        return 0;
    }

    public async Task<int> GetTotalCoinsCollectedAsync()
    {
        totalCoins = 0; // Reset the total coins count
        // List of all level scene names
        List<string> levelScenes = new List<string>
        {
            "level1",
            "level2",
            // Add all your level scene names here
        };

        // Iterate through each scene name and sum up the coins
        foreach (string sceneName in levelScenes)
        {
            // Simulate an async operation if needed (e.g., loading data from cloud)
            totalCoins += await Task.Run(() => GetCoinsCollectedInScene(sceneName));
        }

        Debug.Log($"Total coins collected across all scenes: {totalCoins}");
        return totalCoins;
    }



    public void IncrementMoveCount()
    {
        moveCount++; // Increment the move count
        Debug.Log($"Moves made: {moveCount}");
        UIManager.Instance?.UpdateMovesUI(moveCount); // Update the moves display
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
        ResetMoveCount(); // Reset moves when loading a new scene
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public void RestartCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }

    // Check if the coin with a given ID has already been collected in the current scene
    public bool IsCoinAlreadyCollected(string coinID)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if we have data for this scene
        if (coinsCollectedData.ContainsKey(currentSceneName))
        {
            var sceneData = coinsCollectedData[currentSceneName];
            return sceneData.CollectedCoinIDs.Contains(coinID); // Return true if the coin has been collected
        }

        return false; // Return false if no data exists for this scene
    }

    // Helper classes for serialization
    [Serializable]
    public class SerializableSceneCoinData
    {
        public int CoinsCollected;
        public List<string> CollectedCoinIDs;
    }

    [Serializable]
    public class SceneDataEntry
    {
        public string SceneName;
        public SerializableSceneCoinData Data;
    }

    [Serializable]
    public class SceneCoinDataWrapper
    {
        public List<SceneDataEntry> Entries = new List<SceneDataEntry>();
    }
}
