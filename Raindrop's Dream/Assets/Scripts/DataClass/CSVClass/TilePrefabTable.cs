/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-5-30
  *Description:  TilePrefabTable表对应类
  *Changes:
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePrefabTable : CSVTable
{
    public string name { get; set; }//Tile名
    public int type { get; set; }//Tile类别
    public string producer { get; set; }//制作者名
    public string description { get; set; }
    public string prefabName { get; set; }//tile预制体名
    public string prefabPath { get; set; }//tile预制体路径
}
