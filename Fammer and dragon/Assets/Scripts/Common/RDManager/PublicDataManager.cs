/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-5-29
  *Description: 存储CSV表数据，方便使用
**********************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PublicDataManager : MonoBehaviour
{
    //private static  Dictionary<int, AIName> items;
    public static PublicDataManager instance = null;
    private Dictionary<string, ItemModel> itemModel;
    private Dictionary<string, LevelModel> levelModel;
    private Dictionary<string, SceneTileModel> sceneTileModel;
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
        
      //  RDLog.Log("111111111", LogType.Error);
    }
    private void InitCsv()
    {
        //在这初始化每个Dictionary
        /*level*/
        InitFromCsv<LevelModel>(ref levelModel, "Level.csv");

        /*prefab*/
        InitFromCsv<SceneTileModel>(ref sceneTileModel, "SceneTile.csv");

        InitFromCsv<ItemModel>(ref itemModel, "Item.csv");

    }
    //初始化CSV表
    private void InitFromCsv<T>(ref Dictionary<string, T> _dataModel, string _fileName)
    {
        _dataModel = LoadCsvData<T>(_fileName);
     
    }
    ////从文件初始化关卡信息(Level)
    //private void InitLevelModel(ref Dictionary<string, LevelModel> _levelModel, string _path)
    //{
    //    if (_levelModel == null)
    //    {
    //        _levelModel = new Dictionary<int, LevelModel>();
    //    }
    //    RDFileStream.ReadLevelTable(ref _levelModel, _path);
    //}

    //从CSV表初始化Dictionary
    private static Dictionary<string, T> LoadCsvData<T>(string _fileName)
    {
        Dictionary<string, T> dic = new Dictionary<string, T>();

        /* 从CSV文件读取数据 */
        Dictionary<string, Dictionary<string, string>> result = RDFileStream.ReadCsvFile(_fileName);
        /* 遍历每一行数据 */
        foreach (string name in result.Keys)
        {
            /* CSV的一行数据 */
            Dictionary<string, string> datas = result[name];

            /* 读取Csv数据对象的属性 */
            PropertyInfo[] props = typeof(T).GetProperties();
            /* 使用反射，将CSV文件的数据赋值给CSV数据对象的相应字段，要求CSV文件的字段名和CSV数据对象的字段名完全相同 */
            T obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo p in props)
            {
                ReflectUtil.PiSetValue<T>(datas[p.Name], p, obj);
            }

            /* 按name-数据的形式存储 */
            dic[name] = obj;
        }

        return dic;
    }

    /*Level*/
    public LevelModel GetLevelModel(string _name)
    {
        return levelModel[_name];
    }
    public Dictionary<string, LevelModel>.KeyCollection GetLevelModelKeys()
    {
        return levelModel.Keys;
    }
    public string GetLevelName(string _name)
    {
        return levelModel[_name].name;
    }
    public string GetLevelFilePath(string _name)
    {
        return levelModel[_name].filePath;
    }

    /*SceneTile*/
    public Dictionary<string, SceneTileModel>.KeyCollection GetSceneTileModelKeys()
    {
        return sceneTileModel.Keys;
    }
    public SceneTileModel SceneTileModel(string _name)
    {
        return sceneTileModel[_name];
    }
    public string GetSceneTileName(string _name)
    {
        return sceneTileModel[_name].name;
    }
    public int GetSceneTileType(string _name)
    {
        return sceneTileModel[_name].type;
    }
    public string GetSceneTileLevelType(string _name)
    {
        return sceneTileModel[_name].levelType;
    }

}
