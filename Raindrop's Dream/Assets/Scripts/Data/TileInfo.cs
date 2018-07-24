using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo
{
    public string name;
    public bool isEmpty = true;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public int layer;
    public GameObject tileObjcet = null;
    public TileInfo()
    {
        name = string.Empty;
    }
    public TileInfo(string _name)
    {
        name = _name;
    }
    public void Reset()
    {
        name = string.Empty;
        isEmpty = true;
        tileObjcet = null;
    }
}