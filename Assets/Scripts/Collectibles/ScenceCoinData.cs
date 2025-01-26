using System.Collections.Generic;

[System.Serializable]
public class SceneCoinData
{
    public int CoinsCollected; // Total coins collected in the scene
    public List<string> CollectedCoinIDs; // List of collected coin IDs

    public SceneCoinData()
    {
        CoinsCollected = 0;
        CollectedCoinIDs = new List<string>();
    }
}
