using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo
{

    public int levelId;
    public string levelName;
    public string makerName;
    public List<TileInfo> tiles;

    public LevelInfo()
    {
        levelId = -1;
        tiles = new List<TileInfo>();
    }
    public bool IsEmpty()
    {
        return levelId > 0 ? false : true;
    }
}
public class TileInfo
{
    public int tileId;
    public Vector3Int pos;
}