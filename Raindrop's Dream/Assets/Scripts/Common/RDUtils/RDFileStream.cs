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

public class RDFileStream
{
    //读取CSV文件
    public static Dictionary<string, Dictionary<string, string>> ReadCsvFile(string _fileName)
    {
        Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
        string url = RDPlatform.SplitPath(new string[] { RDPlatform.DATA_PATH, "CSV", _fileName });
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
            string url = RDPlatform.SplitPath(new string[] { RDPlatform.DATA_PATH, "CSV", _fileName });
            FileStream fs = new FileStream(url, FileMode.Append);
            StreamWriter writer = new StreamWriter(fs);
            foreach (CSVModel tableRow in _rowObject)
            {
                PropertyInfo[] props = tableRow.GetType().GetProperties();
                //ID在props最后一个，先处理
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
    //从文件读取LevelTable
    public static void ReadLevelTable(ref Dictionary<int, LevelModel> _levelModel, string _path)
    {
        string allPath = RDPlatform.SplitPath(new string[] { RDPlatform.DATA_PATH, "Level", _path });
        if (!Directory.Exists(allPath))
        {
            //文件读取失败
        }
        else
        {
            try
            {
                DirectoryInfo producerPath = new DirectoryInfo(allPath);
                foreach (DirectoryInfo producer in producerPath.GetDirectories())
                {

                    foreach (FileInfo level in producer.GetFiles())
                    {
                        if (level.Extension == ".level")
                        {
                            LevelModel t = new LevelModel();
                            t.id = int.Parse(level.Name.Split('.')[0].Split('#')[1]);
                            //maker、level名在最后
                            t.name = level.Name.Split('.')[0];
                            t.producer = producer.Name;
                            //全路径：../Level/User/作者名/关卡名#关卡ID.level
                            t.filePath = RDPlatform.SplitPath(new string[] { level.DirectoryName, level.Name });
                            string imgName = level.Name.Split('.')[0] + ".png";
                            t.imagePath = RDPlatform.SplitPath(new string[] { level.DirectoryName, imgName });
                            _levelModel.Add(t.id, t);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                // RDLog.Log(e);
            }
        }
    }
    //读取Level文件
    public static LevelInfo ReadLevelFile(int _levelId)
    {
        LevelInfo level = new LevelInfo();
        try
        {
            string url = RDPlatform.SplitPath(new string[] { PublicDataManager.instance.GetLevelFilePath(_levelId) });
            FileStream fs = new FileStream(url, FileMode.Open);
            StreamReader reader = new StreamReader(fs);
            level.id = int.Parse(reader.ReadLine());
            level.name = reader.ReadLine();
            level.producer = reader.ReadLine();
            string tileInfoLine;//读取的一行
            string[] tileInfos;//以#分二段
            string[] posInfo;//position以,分三段
            while ((tileInfoLine = reader.ReadLine()) != null)
            {
                tileInfos = tileInfoLine.Split('#');

                TileInfo tile = new TileInfo();
                tile.id = int.Parse(tileInfos[0]);
                posInfo = tileInfos[1].Split(',');
                tile.pos = new Vector3Int(int.Parse(posInfo[0]), int.Parse(posInfo[1]), int.Parse(posInfo[2]));

                level.tiles.Add(tile);
            }
            reader.Close();
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return level;
    }

    //写入Level文件,文件夹路径：/Level/User/作者名/关卡名#关卡ID.level
    public static void WriteLevelFile(LevelInfo _level)
    {
        try
        {
            string url = RDPlatform.SplitPath(new string[] { RDPlatform.DATA_PATH, "Level", "User", _level.producer });
            if (!Directory.Exists(url))
            {
                Directory.CreateDirectory(url);
            }
            string fileName = _level.name + "#" + _level.id.ToString() + ".level";
            FileStream fs = new FileStream(RDPlatform.SplitPath(new string[] { url, fileName }), FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine(_level.id);
            writer.WriteLine(_level.name);
            writer.WriteLine(_level.producer);
            foreach (TileInfo tile in _level.tiles)
            {
                writer.WriteLine(tile.id + "#" + tile.pos.x + "," + tile.pos.y + "," + tile.pos.z);
            }
            writer.Close();
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
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
    public static Dictionary<string, AssetBundle> ReadAllAssestBudle()
    {
        string path = RDPlatform.SplitPath(new string[] { RDPlatform.DATA_PATH, "AssestBundles" });
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
