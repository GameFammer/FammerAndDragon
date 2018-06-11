/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-5-29
  *Description: 存储CSV表数据，方便使用
  *Changes:
**********************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;

public class PublicDataManager : MonoBehaviour
{
    //private static  Dictionary<int, AIName> items;
    public static PublicDataManager instance = null;
    public static string DATA_PATH
    {
        get
        {
#if UNITY_IOS || UNITY_ANDROID
            return Application.persistentDataPath;

#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return Application.streamingAssetsPath;
#endif
        }
    }
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
        /*level*/
        InitFromCsv<LevelTable>(ref levelTable, "LevelTable.csv");
        InitLevelFromFile(ref levelTable);

        InitFromCsv<TilePrefabTable>(ref tilePrefabTable, "TilePrefabTable.csv");

    }
    //初始化CSV表
    private void InitFromCsv<T>(ref Dictionary<int, T> _dataTable, string _fileName)
    {
        _dataTable = LoadCsvData<T>(_fileName);
    }
    //初始化玩家自定义关卡(Level)
    private void InitLevelFromFile(ref Dictionary<int, LevelTable> _dataTable)
    {
        string url = PublicDataManager.DATA_PATH + "\\Level\\User";
        //ID为当前最后一个+1
        int id = levelTable.Count + 1;

        if (!Directory.Exists(url))
        {
            //文件读取失败
        }
        else
        {
            try
            {
                DirectoryInfo makerPath = new DirectoryInfo(url);
                foreach(DirectoryInfo maker in makerPath.GetDirectories())
                {
                    
                    foreach(FileInfo level in maker.GetFiles())
                    {
                        if(level.Extension==".level")
                        {
                            LevelTable t = new LevelTable();
                            t.ID = id;
                            //maker、level名在最后
                            t.LevelName = level.Name.Split('.')[0];
                            t.MakerName = maker.Name;
                            t.LevelFilePath = "User\\" + t.MakerName + '\\' + t.LevelName;
                            levelTable.Add(t.ID, t);
                            id++;
                        }
                       
                    }
                }
                /*   foreach (string pathOnMaker in Directory.GetDirectories(url))
                   {
                       string[] makerNameSplit = pathOnMaker.Split('\\');
                       foreach (string pathOnLevel in Directory.GetFiles(pathOnMaker))
                       {
                           string[] levelNamerSplit = pathOnLevel.Split('\\');
                           LevelTable t = new LevelTable();
                           t.ID = id;
                           //maker、level名在最后
                           t.LevelName = levelNamerSplit[levelNamerSplit.Length-1];   
                           t.MakerName = makerNameSplit[makerNameSplit.Length - 1];
                           t.LevelFilePath = "User\\"+t.MakerName + '\\' + t.LevelName;
                           levelTable.Add(t.ID, t);
                           id++;
                       }


                   }*/

            }
            catch (Exception e)
            {

            }

        }
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
    public Dictionary<int, LevelTable>.KeyCollection GetLevelTableKeys()
    {
        return levelTable.Keys;
    }
    public int GetLevelTableCount()
    {
        return levelTable.Count;
    }
    public string GetLevelName(int _ID)
    {
        return levelTable[_ID].LevelName;
    }
    public string GetLevelFilePath(int _ID)
    {
        return levelTable[_ID].LevelFilePath;
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
    public string GetTilePrefabName(int _ID)
    {
        return tilePrefabTable[_ID].TilePrefabName;
    }
    public int GetTilePrefabType(int _ID)
    {
        return tilePrefabTable[_ID].TileType;
    }
    public string GetTilePrefabPath(int _ID)
    {
        return tilePrefabTable[_ID].TilePrefabPath;
    }
}
