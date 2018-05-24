using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room{
    public int id;//房间ID
    public Vector2 position;//房间左上角坐标
    public float width;     //房间宽度
    public float height;    //房间高度
    public bool isMainRoom; //是否为主房间
}
