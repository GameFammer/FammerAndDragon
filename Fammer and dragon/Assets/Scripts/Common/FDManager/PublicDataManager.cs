using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PublicDataManager : MonoBehaviour
{
    //private static  Dictionary<int, AIName> items;
    public static PublicDataManager instance = null;
    private Dictionary<string, ItemModel> itemModel;
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
    }
    private void InitIni()
    {
        
      //  FDLog.Log("111111111", LogType.Error);
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
    //    FDFileStream.ReadLevelTable(ref _levelModel, _path);
    //}

    //从CSV表初始化Dictionary
    private static Dictionary<string, T> LoadCsvData<T>(string _fileName)
    {
        Dictionary<string, T> dic = new Dictionary<string, T>();

        /* 从CSV文件读取数据 */
        Dictionary<string, Dictionary<string, string>> result = FDFileStream.ReadCsvFile(_fileName);
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

}
