/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-5-29
  *Description: 各种文件读写
**********************************************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FDFileStream
{
    //读取CSV文件
    public static Dictionary<string, Dictionary<string, string>> ReadCsvFile(string _fileName)
    {
        Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
        string url = FDPlatform.SplitPath(new string[] { FDPlatform.DATA_PATH, "CSV", _fileName });
        string[] fileData = File.ReadAllLines(url);
        if (fileData.Length < 3)
        {
            return result;
        }

        /* CSV文件的第一行为Key字段，第二行为说明（不需要读取），第三行开始是数据。第一个字段一定是ID。 */
        string[] keys = fileData[0].Split(',');
        for (int i = 2; i < fileData.Length; i++)
        {
            string[] line = fileData[i].Split(',');

            /* 以ID为key值，创建一个新的集合，用于保存当前行的数据 */
            string ID = line[0];
            result[ID] = new Dictionary<string, string>();
            for (int j = 0; j < line.Length; j++)
            {
                /* 每一行的数据存储规则：Key字段-Value值 */
                result[ID][keys[j]] = line[j];
            }
        }
        return result;
    }
    //写入CSV文件
    public static void WriteCsvFile(string _fileName, CSVModel[] _rowObject)
    {
        try
        {
            string url = FDPlatform.SplitPath(new string[] { FDPlatform.DATA_PATH, "CSV", _fileName });
            FileStream fs = new FileStream(url, FileMode.Append);
            StreamWriter writer = new StreamWriter(fs);
            foreach (CSVModel tableRow in _rowObject)
            {
                PropertyInfo[] props = tableRow.GetType().GetProperties();
                //Name在props最后一个，先处理
                string row = props[props.Length - 1].GetValue(tableRow, null) == null ? "" : props[props.Length - 1].GetValue(tableRow, null).ToString() + ",";
                for (int i = 0; i < props.Length - 1; i++)
                {
                    if (i != props.Length - 2)
                    {
                        string info = props[i].GetValue(tableRow, null) == null ? "" : props[i].GetValue(tableRow, null).ToString();
                        row = row + info + ",";
                    }
                    else
                    {
                        string info = props[i].GetValue(tableRow, null) == null ? "" : props[i].GetValue(tableRow, null).ToString();
                        row = row + info;
                    }
                }
                if (row != null)
                {
                    writer.WriteLine(row);
                }
            }

            writer.Close();
            fs.Close();
        }
        catch (Exception e)
        {

        }
    }

    public static GameObject[] GetPrefabsByNames(string[] _names)
    {
        //TODO
        return new GameObject[0];
    }

    public static GameObject[] GetPrefabsByIds(string[] _ids)
    {
        //TODO
        return new GameObject[0];
    }
    //加载所有AssestBudle
    public static Dictionary<string, AssetBundle> ReadAllAssestBudle()
    {
        string path = FDPlatform.SplitPath(new string[] { FDPlatform.DATA_PATH, "AssestBundles" });
        Dictionary<string, AssetBundle> assets = new Dictionary<string, AssetBundle>();
        if (!Directory.Exists(path))
        {
            //Assest读取失败
            return assets;
        }
        else
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                FileInfo[] file = directory.GetFiles();

                for (int i = 0; i < file.Length; i++)
                {
                    if (file[i].Extension==".package")
                    {
                        AssetBundle assetBundle = AssetBundle.LoadFromFile(file[i].FullName);
                        assets.Add(file[i].Name.Split('.')[0], assetBundle);
                    }                     
                }

            }
            catch (Exception e)
            {

            }
            return assets;
        }
    }
}
