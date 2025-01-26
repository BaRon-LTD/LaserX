using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private CollectibleItem collectibleItem;

    private int moveCount = 0; // Counter for the number of moves

    // Dictionary to track coin data per scene
    private Dictionary<string, SceneCoinData> coinsCollectedData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        collectibleItem = new CollectibleItem();
        coinsCollectedData = new Dictionary<string, SceneCoinData>(); // Initialize the dictionary
    }

    public void AddScore(int amount, string coinID)
    {
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
}
