/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-5-30
  *Description: LevelTable表对应类
  *Changes:
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTable :CSVTable
{
    public string LevelName { get; set; }//关卡名
    public string MakerName { get; set; }//制作者名
    public string LevelFilePath { get; set; }//关卡文件路径
}
