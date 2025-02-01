using System;
using System.Collections.Generic;

[Serializable]
public class GameSceneCoinData
{
    public int CoinsCollected;
    public List<string> CollectedCoinIDs = new List<string>();
}

[Serializable]
public class GameSerializableCoinData
{
    public int CoinsCollected;
    public List<string> CollectedCoinIDs;
}

[Serializable]
public class GameSceneDataEntry
{
    public string SceneName;
    public GameSerializableCoinData Data;
}

[Serializable]
public class GameSaveDataWrapper
{
    public List<GameSceneDataEntry> Entries = new List<GameSceneDataEntry>();
}