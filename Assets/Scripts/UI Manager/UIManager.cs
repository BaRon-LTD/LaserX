using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ballons;
    private int previousScore = -1; // Initialize with a value that doesn't match the initial score

    private void Start()
    {
        UpdateScoreUI();
    }

    private void Update()
    {
        // Check if the score has changed
        int currentScore = CollectibleItem.GetScore();
        if (currentScore != previousScore)
        {
            UpdateScoreUI();
            previousScore = currentScore; // Update the stored score
        }
    }

    public void UpdateScoreUI()
    {
        ballons.text = "      X " + CollectibleItem.GetScore();
    }
}
