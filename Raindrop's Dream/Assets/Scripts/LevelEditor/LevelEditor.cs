using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public struct TileData
{
    public int tileId;//CSV表中物品对应的ID
    public Vector3Int position;//Tile位置坐标信息
    public int layer;
}
public class MapData
{
    public int mapId;//csv表中对应id
    public string mapName;//地图名称
    public string makerName;//地图制作者
    public List<TileData> tileList = new List<TileData>();//tile列表
}
public class LevelEditor : MonoBehaviour {
    public static LevelEditor instance = null;
    public Tilemap tileMap;

    private MapData mapData;//地图信息数据
    void Awake()
    {
        //单例，关卡切换不销毁
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        mapData = new MapData();
        mapData.mapId = 10;
        mapData.mapName = "ABC";
        mapData.makerName = "AICHEN";
        for (int i = 0; i < 100; i++)
        {
            TileData a=new TileData();
            a.tileId = i;
            a.position = new Vector3Int(i, i, i);
            a.layer = i;
            mapData.tileList.Add(a);
        }
    }
    // Use this for initialization
    void Start ()
    {
       
	}
	
	// Update is called once per frame
	void Update ()
    {

    }
    //保存地图
    public void SaveLevel()
    {
        //文件夹路径：/Level/作者名/地图名文件夹
        string saveDirPath = Application.streamingAssetsPath + "\\Level\\" + mapData.makerName + "\\" + mapData.mapName;
        if(!Directory.Exists(saveDirPath))
        {
            Directory.CreateDirectory(saveDirPath);
        }
        else
        {
            try
            {
                FileStream fs = new FileStream(saveDirPath + "\\" + mapData.mapName + ".level", FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);
                writer.WriteLine(mapData.mapId);
                writer.WriteLine(mapData.mapName);
                writer.WriteLine(mapData.makerName);
                foreach (TileData tile in mapData.tileList)
                {
                    writer.WriteLine(tile.tileId + '#' + tile.position.x + ',' + tile.position.y + ',' + tile.position.z + '#' + tile.layer);
                }
                writer.Close();
            }
           catch(Exception e)
            {
                //文件写入失败

            }
        }      
    }
    //从level文件读取Level
    public void LoadLevel(int _mapId)
    {
        LevelTable levelInfo = PublicDataManager.instance.GetLevelTable(_mapId);
        if (levelInfo != null)
        {
            try
            {
                FileStream fs = new FileStream(Application.streamingAssetsPath + levelInfo.LevelFilePath, FileMode.Open);
                StreamReader reader = new StreamReader(fs);
                mapData.mapId = int.Parse(reader.ReadLine());
                mapData.mapName = reader.ReadLine();
                mapData.makerName = reader.ReadLine();
                string tileInfoLine;//读取的一行
                string[] tileInfo;//以#分三段
                string[] posInfo;//position以,分三段
                while ((tileInfoLine = reader.ReadLine()) != null)
                {
                    tileInfo = tileInfoLine.Split('#');
                    TileData t = new TileData();
                    t.tileId = int.Parse(tileInfo[0]);

                    posInfo = tileInfo[1].Split(',');
                    t.position = new Vector3Int(int.Parse(posInfo[0]), int.Parse(posInfo[1]), int.Parse(posInfo[2]));

                    t.layer = int.Parse(tileInfo[2]);
                    mapData.tileList.Add(t);
                }

            }
            catch(Exception e)
            {
                //读取level文件失败
            }
        }
        //StreamReader reader=new StreamReader()
    }
}
