using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo
{
    public int id;
    public GameObject tileObjcet;
    public TileInfo()
    {
        id = -1;
        tileObjcet = new GameObject();
    }
    public TileInfo(int _id, GameObject _tile)
    {
        id = _id;
        tileObjcet = _tile;
    }
    public bool isEmpty()
    {
        return id < 0 ? true : false;
    }
    public void Reset()
    {
        id = -1;
        tileObjcet = new GameObject();
    }
}