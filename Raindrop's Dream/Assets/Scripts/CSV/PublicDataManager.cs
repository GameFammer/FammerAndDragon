/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-5-29
  *Description: 存储CSV表数据，方便使用
  *Changes:
**********************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PublicDataManager : MonoBehaviour
{
   //private static  Dictionary<int, AIName> items;
    public static PublicDataManager instance=null;

    private Dictionary<int, LevelTable> levelTable;
    private Dictionary<int, TilePrefabTable> tilePrefabTable;
    void Awake()
    {
        //单例，关卡切换不销毁
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        //初始化Ini
        InitIni();
        //初始化CSV
        InitCsv();
    }
    private void InitIni()
    {
        
    }
    private void InitCsv()
    {
        //在这初始化每个CSV表
        //   InitFromCsv<AIName>(ref items, "AIName.csv");
        InitFromCsv<LevelTable>(ref levelTable, "LevelTable.csv");
        InitFromCsv<TilePrefabTable>(ref tilePrefabTable, "TilePrefabTable.csv");
    }
    //初始化CSV表
    private void InitFromCsv<T>(ref Dictionary<int, T> _dataTable,string _fileName)
    {
        _dataTable=LoadCsvData<T>(_fileName);
    }
    //从CSV表初始化Dictionary
    private static Dictionary<int, T> LoadCsvData<T>(string _fileName)
    {
        Dictionary<int, T> dic = new Dictionary<int, T>();

        /* 从CSV文件读取数据 */
        Dictionary<string, Dictionary<string, string>> result = CSVFileStream.ReadCsvFile(_fileName);

        /* 遍历每一行数据 */
        foreach (string ID in result.Keys)
        {
            /* CSV的一行数据 */
            Dictionary<string, string> datas = result[ID];

            /* 读取Csv数据对象的属性 */
            PropertyInfo[] props = typeof(T).GetProperties();

            /* 使用反射，将CSV文件的数据赋值给CSV数据对象的相应字段，要求CSV文件的字段名和CSV数据对象的字段名完全相同 */
            T obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo p in props)                                                                                                                                                                                                                        
            {
                ReflectUtil.PiSetValue<T>(datas[p.Name], p, obj);
            }

            /* 按ID-数据的形式存储 */
            dic[Convert.ToInt32(ID)] = obj;
        }

        return dic;
    }

    /*Level*/
    public LevelTable GetLevelTable(int _ID)
    {
        return levelTable[_ID];
    }


    /*TilePrefab*/
    public Dictionary<int, TilePrefabTable>.KeyCollection GetTilePrefabTableKeys()
    {
        return tilePrefabTable.Keys;
    }
    public TilePrefabTable GetTilePrefabTable(int _ID)
    {
        return tilePrefabTable[_ID];
    }
}
