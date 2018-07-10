/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-5-30
  *Description:  TilePrefabTable表对应类
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTileModel : CSVModel
{
    public string name { get; set; }//Tile名
    public int type { get; set; }//Tile类别(Static:0 Function:1)
    public string producer { get; set; }//制作者名
    public string description { get; set; }//描述
    public string levelType { get; set; }//对应关卡类型
}
