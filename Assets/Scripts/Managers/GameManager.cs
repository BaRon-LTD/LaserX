using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private CollectibleItem collectibleItem;

    private void Awake()
    {
        // Singleton pattern: Ensure only one instance of GameManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        // Initialize the CollectibleItem instance
        collectibleItem = new CollectibleItem();
    }

    public void AddScore(int amount)
    {
        collectibleItem.AddBalloon(amount); // Use CollectibleItem to manage the score
        Debug.Log($"Score updated: {collectibleItem.GetScore()}");
        UIManager.Instance?.UpdateScoreUI(); // Update the UI if UIManager exists
    }

    public int GetScore()
    {
        return collectibleItem.GetScore(); // Retrieve score from CollectibleItem
    }

    public void LoadScene(string sceneName)
    {
        collectibleItem.ResetScore(); // Reset score when loading a new scene
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public void RestartCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }
}
