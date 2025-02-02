using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

public class SaveManager : MonoBehaviour
{
    private Dictionary<string, GameSceneCoinData> coinsCollectedData;
    private GameLaserColorData laserColorData;
    private const string SAVE_KEY = "player_coins_data";
    public bool isInitialized = false;
    public bool isDataLoaded = false;

    private void Awake()
    {
        coinsCollectedData = new Dictionary<string, GameSceneCoinData>();
        laserColorData = new GameLaserColorData();
    }

    public async Task InitializeAsync()
    {
        if (isInitialized) return;

        try
        {
            ClearSavedData();

            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            isInitialized = true;
            await LoadCloudData();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
            isInitialized = false;
            throw;
        }
    }

    public async Task SaveToCloud()
    {
        try
        {
            string serializedData = SerializeCoinsData();
            Debug.Log($"Attempting to save data: {serializedData}");

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
        if (isDataLoaded) return;
        
        try
        {
            ClearSavedData();

            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { SAVE_KEY });

            if (data != null && data.TryGetValue(SAVE_KEY, out var savedItem))
            {
                var savedData = savedItem.Value.GetAsString();
                if (!string.IsNullOrEmpty(savedData))
                {
                    Debug.Log($"Loaded raw data from cloud: {savedData}");
                    DeserializeCoinsData(savedData);
                }
                else
                {
                    Debug.Log("No saved data found in cloud - starting fresh");
                }
            }
            else
            {
                Debug.Log("No saved data found in cloud - starting fresh");
            }

            isDataLoaded = true;
            UIManager.Instance?.UpdateScoreUI();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game data: {e.Message}");
            Debug.Log("Starting with fresh game data due to load error");
            ClearSavedData();
            isDataLoaded = false;
            throw;
        }
    }

    private string SerializeCoinsData()
    {
        try
        {
            var wrapper = new GameSaveDataWrapper();
            wrapper.Entries = coinsCollectedData.Select(kvp => new GameSceneDataEntry
            {
                SceneName = kvp.Key,
                Data = new GameSceneCoinData
                {
                    CoinsCollected = kvp.Value.CoinsCollected,
                    CollectedCoinIDs = kvp.Value.CollectedCoinIDs ?? new List<string>()
                }
            }).ToList();

            // Add laser color data to wrapper
            wrapper.LaserColorData = laserColorData;

            string json = JsonUtility.ToJson(wrapper);
            Debug.Log($"Serialized data: {json}");
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
            Debug.Log($"Attempting to deserialize: {jsonData}");

            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.Log("Empty save data received");
                return;
            }

            var wrapper = JsonUtility.FromJson<GameSaveDataWrapper>(jsonData);
            if (wrapper != null)
            {
                // Deserialize coins data
                if(wrapper.Entries != null)
                {
                    coinsCollectedData.Clear();
                    foreach (var entry in wrapper.Entries)
                    {
                        if (entry.Data != null)
                        {
                            coinsCollectedData[entry.SceneName] = new GameSceneCoinData
                            {
                                CoinsCollected = entry.Data.CoinsCollected,
                                CollectedCoinIDs = entry.Data.CollectedCoinIDs ?? new List<string>()
                            };
                        }
                    }
                    Debug.Log($"Successfully deserialized {wrapper.Entries.Count} scenes");
                }
                // Deserialize laser color data
                if (wrapper.LaserColorData != null)
                {
                    laserColorData = wrapper.LaserColorData;
                }
                else
                {
                    laserColorData = new GameLaserColorData();
                }
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
            laserColorData = new GameLaserColorData();
        }
    }

    public void AddCoinToScene(string sceneName, int amount, string coinID)
    {
        if (!coinsCollectedData.ContainsKey(sceneName))
        {
            coinsCollectedData[sceneName] = new GameSceneCoinData();
        }

        GameSceneCoinData sceneData = coinsCollectedData[sceneName];
        sceneData.CollectedCoinIDs.Add(coinID);
        sceneData.CoinsCollected += amount;

        Debug.Log($"Coins collected in {sceneName}: {sceneData.CoinsCollected}");
        Debug.Log($"Collected Coin IDs in {sceneName}: {string.Join(", ", sceneData.CollectedCoinIDs)}");

        SaveAsync();
    }

    private async void SaveAsync()
    {
        await SaveToCloud();
    }

    public bool IsCoinAlreadyCollected(string coinID)
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (coinsCollectedData.ContainsKey(currentSceneName))
        {
            var sceneData = coinsCollectedData[currentSceneName];
            return sceneData.CollectedCoinIDs.Contains(coinID);
        }

        return false;
    }

    public void ClearSavedData()
    {
        if (coinsCollectedData != null)
        {
            coinsCollectedData.Clear();
        }
        else 
        {
            coinsCollectedData = new Dictionary<string, GameSceneCoinData>();
        }

        laserColorData = new GameLaserColorData();
        isDataLoaded = false;
    }

    public async Task<int> GetTotalCoinsCollectedAsync()
    {
        try 
        {
            if (!isDataLoaded)
            {
                await LoadCloudData();
            }
            return GetTotalCoinsCollected();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error calculating total coins: {e.Message}");
            return 0;
        }
    }

    public int GetTotalCoinsCollected()
    {
        return coinsCollectedData.Values.Sum(sceneData => sceneData.CoinsCollected);
    }

    public int GetCoinsCollectedInScene(string sceneName)
    {
        if (coinsCollectedData.ContainsKey(sceneName))
        {
            return coinsCollectedData[sceneName].CoinsCollected;
        }
        return 0;
    }
    
    public bool IsCoinAlreadyCollectedInScene(string sceneName, string coinID)
    {
        if (coinsCollectedData.ContainsKey(sceneName))
        {
            var sceneData = coinsCollectedData[sceneName];
            return sceneData.CollectedCoinIDs?.Contains(coinID) ?? false;
        }
        return false;
    }

    
    // New methods for laser color management
    public void AddLaserColor(int colorIndex)
    {
        if (!laserColorData.ColorList.Contains(colorIndex))
        {
            laserColorData.ColorList.Add(colorIndex);
            SaveAsync();
        }
    }

    public void SetCurrentLaserColorIndex(int index)
    {
        if (laserColorData.ColorList.Contains(index))
        {
            laserColorData.ColorIndex = index;
            SaveAsync();
        }
        else
        {
            Debug.LogWarning($"Attempted to set laser color index {index} which is not unlocked");
        }
    }

    public int GetCurrentLaserColorIndex()
    {
        return laserColorData.ColorIndex;
    }

    public List<int> GetUnlockedLaserColors()
    {
        return new List<int>(laserColorData.ColorList);
    }

    public bool IsLaserColorUnlocked(int colorIndex)
    {
        return laserColorData.ColorList.Contains(colorIndex);
    }

}