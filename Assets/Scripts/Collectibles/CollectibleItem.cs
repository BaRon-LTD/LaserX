using UnityEngine;

public static class CollectibleItem
{
    private static int score = 0;

    public static void AddBalloon(int amount)
    {
        score += amount;
    }

    public static int GetScore()
    {
        return score;
    }
}
