using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo
{

    public int levelId;
    public string levelName;
    public string makerName;
    public List<TileInfo> tileInfo;

    public LevelInfo()
    {
        tileInfo = new List<TileInfo>();
    }
}
public class TileInfo
{
    public int tileId;
    public Vector3Int pos;
}