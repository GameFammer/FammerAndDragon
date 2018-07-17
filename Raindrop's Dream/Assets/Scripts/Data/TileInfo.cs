using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo
{
    public int id;
    public bool isEmpty = true;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public int layer;
    public GameObject tileObjcet = null;
    public TileInfo()
    {
        id = -1;
    }
    public TileInfo(int _id)
    {
        id = _id;
    }
    public void Reset()
    {
        id = -1;
        isEmpty = true;
        tileObjcet = null;
    }
}