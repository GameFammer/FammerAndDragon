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
    private Dictionary<int, ItemModel> itemModel;
    private Dictionary<int, LevelModel> levelModel;
    private Dictionary<int, TilePrefabModel> tilePrefabModel;
    void Awake()
    {
        //单例，关卡切换不销毁
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    
    void OnEnable()
    {
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
        InitLevelModel(ref levelModel, "Dev");
        InitLevelModel(ref levelModel, "User");

        /*prefab*/
        InitFromCsv<TilePrefabModel>(ref tilePrefabModel, "TilePrefabModel.csv");

        InitFromCsv<ItemModel>(ref itemModel, "ItemModel.csv");

    }
    //初始化CSV表
    private void InitFromCsv<T>(ref Dictionary<int, T> _dataModel, string _fileName)
    {
        _dataModel = LoadCsvData<T>(_fileName);
     
    }
    //从文件初始化关卡信息(Level)
    private void InitLevelModel(ref Dictionary<int, LevelModel> _levelModel, string _path)
    {
        if (_levelModel == null)
        {
            _levelModel = new Dictionary<int, LevelModel>();
        }
        RDFileStream.ReadLevelTable(ref _levelModel, _path);
    }

    //从CSV表初始化Dictionary
    private static Dictionary<int, T> LoadCsvData<T>(string _fileName)
    {
        Dictionary<int, T> dic = new Dictionary<int, T>();

        /* 从CSV文件读取数据 */
        Dictionary<string, Dictionary<string, string>> result = RDFileStream.ReadCsvFile(_fileName);
        /* 遍历每一行数据 */
        foreach (string id in result.Keys)
        {
            /* CSV的一行数据 */
            Dictionary<string, string> datas = result[id];

            /* 读取Csv数据对象的属性 */
            PropertyInfo[] props = typeof(T).GetProperties();
            /* 使用反射，将CSV文件的数据赋值给CSV数据对象的相应字段，要求CSV文件的字段名和CSV数据对象的字段名完全相同 */
            T obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo p in props)
            {
                ReflectUtil.PiSetValue<T>(datas[p.Name], p, obj);
            }

            /* 按id-数据的形式存储 */
            dic[Convert.ToInt32(id)] = obj;
        }

        return dic;
    }

    /*Level*/
    public LevelModel GetLevelModel(int _id)
    {
        return levelModel[_id];
    }
    public Dictionary<int, LevelModel>.KeyCollection GetLevelModelKeys()
    {
        return levelModel.Keys;
    }
    public int GetLevelModelMaxKey()
    {
        int maxKey = 0;
        foreach (int key in levelModel.Keys)
        {
            if (key > maxKey)
            {
                maxKey = key;
            }
        }
        return maxKey;
    }
    public string GetLevelName(int _id)
    {
        return levelModel[_id].name;
    }
    public string GetLevelFilePath(int _id)
    {
        return levelModel[_id].filePath;
    }

    /*TilePrefab*/
    public Dictionary<int, TilePrefabModel>.KeyCollection GetTilePrefabModelKeys()
    {
        return tilePrefabModel.Keys;
    }
    public TilePrefabModel GetTilePrefabModel(int _id)
    {
        return tilePrefabModel[_id];
    }
    public string GetTilePrefabName(int _id)
    {
        return tilePrefabModel[_id].prefabName;
    }
    public int GetTilePrefabType(int _id)
    {
        return tilePrefabModel[_id].type;
    }
    public string GetTilePrefabPath(int _id)
    {
        return tilePrefabModel[_id].prefabPath;
    }
}
