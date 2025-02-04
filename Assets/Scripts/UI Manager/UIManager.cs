using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI ballons;
    [SerializeField] private TextMeshProUGUI levelName;
    // [SerializeField] private TextMeshProUGUI moves;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateScoreUI();
        UpdateLevelNumber();
        // UpdateMovesUI(0); // Initialize the moves display

    }

    public void UpdateLevelNumber()
    {
        if (levelName != null)
        {
            levelName.text = GameManager.Instance.GetSceneName();
        }
    }

    public void UpdateScoreUI()
    {
        if (ballons != null)
        {
            ballons.text = "     X " + GameManager.Instance.GetScore();
        }

    }

    // public void UpdateMovesUI(int moveCount)
    // {
    //     if (moves != null)
    //     {
    //         moves.text = "Moves: " + moveCount;
    //     }

    // }
}
