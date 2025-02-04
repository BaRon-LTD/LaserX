using System;
using System.Collections.Generic;

[Serializable]
public class GameSceneCoinData
{
    public int CoinsCollected;
    public List<string> CollectedCoinIDs = new List<string>();
}

[Serializable]
public class GameLaserColorData
{
    public List<int> ColorList;
    public int ColorIndex;

    public GameLaserColorData()
    {
        ColorList = new List<int>() { 0 };
        ColorIndex = 0;
    }
}


[Serializable]
public class GameSceneDataEntry
{
    public string SceneName;
    public GameSceneCoinData Data;
}



[Serializable]
public class GameSaveDataWrapper
{
    public List<GameSceneDataEntry> Entries = new List<GameSceneDataEntry>();
    public GameLaserColorData LaserColorData = new GameLaserColorData();
}