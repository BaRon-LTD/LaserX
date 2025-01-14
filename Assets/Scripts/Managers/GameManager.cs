using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private CollectibleItem collectibleItem;

    private int moveCount = 0; // Counter for the number of moves

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
    }

    public void AddScore(int amount)
    {
        collectibleItem.AddBalloon(amount);
        Debug.Log($"Score updated: {collectibleItem.GetScore()}");
        UIManager.Instance?.UpdateScoreUI();
    }

    public int GetScore()
    {
        return collectibleItem.GetScore();
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
