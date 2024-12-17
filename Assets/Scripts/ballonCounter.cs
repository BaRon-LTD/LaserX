using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CandyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ballons;
    [SerializeField] private int score = 0;

    // private void Start() {
    //     candies = GameObject.Find("candyCounter").GetComponent<Text>();
    // }

    public void AddBallon(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        ballons.text = "      X " + score;
    }
}
