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

public class LevelModel : CSVModel
{
    public int type { get; set; }
    public string producer { get; set; }//制作者名
    public string filePath { get; set; }//关卡文件路径
    public string imagePath { get; set; }//关卡封面路径
}
