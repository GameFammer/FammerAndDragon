using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo
{

    public int id;
    public string name;
    public string producer;
    public List<TileInfo> tiles;

    public LevelInfo()
    {
        id = -1;
        tiles = new List<TileInfo>();
    }
    public bool IsEmpty()
    {
        return tiles.Count> 0 ? false : true;
    }
}