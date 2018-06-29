using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemTable:CSVTable
{
    public string name { get; set; }//名字
    public string type { get; set; }//类型
    public string producer { get; set; }//制作者名
    public string description { get; set; }//描述
    public string prefabName { get; set; }//tile预制体名
    public string prefabPath { get; set; }//tile预制体路径
    public int value { get; set; }//属性
}