using UnityEngine;

public class CollectibleItem
{
    private int score = 0;

    public void AddCoin(int amount)
    {
        score += amount;
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
