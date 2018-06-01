using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    /*UI*/
    public GameObject tileButton;//tile按钮预制体

    public GameObject backgroundPage;//背景tile分页
    public GameObject mapHasColliderPage;//可碰撞地形组成分页
    public GameObject mapNoColliderPage;//不可碰撞地形组成分页
    public GameObject itemsPage;//道具分页
    public GameObject npcPage;//NPC分页
    public GameObject enemyPage;//enemy分页

    public GameObject nowTileImage;//当前tile图标
    public GameObject saveButton;//保存按钮
    /*Data*/
    private MapData mapData;//地图信息数据
    private Dictionary<int,GameObject> tilePrefabs;//Tile预制体

    /*TileType*/
    private const int TILE_BACKGROUND = 0;
    private const int TILE_MAPHASCOLLIDER = 1;
    private const int TILE_MAPNOCOLLIDER = 2;
    private const int TILE_ITEM = 3;
    private const int TILE_NPC = 4;
    private const int TILE_ENEMY = 5;
    void Awake()
    {
        //单例，关卡切换不销毁
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //初始化工具栏
        InitToolbars();
        //初始化Tile预制体
        InitTilePrefabs();
        //初始化Tile选择按钮
        InitTileButtons();
        //初始化已有Level列表
        InitLevelList();
        mapData = new MapData();
        mapData.mapId = 1;
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
    //初始化工具栏
    void InitToolbars()
    {

    }
    //加载TilePrefabs
    void InitTilePrefabs()
    {
        
    }
    //初始化TileButton
    void InitTileButtons()
    {
        foreach (int key in PublicDataManager.instance.GetTilePrefabTableKeys())
        {
            TilePrefabTable tile = PublicDataManager.instance.GetTilePrefabTable(key);
            //创建按钮绑定点击函数
            GameObject btn = Instantiate(tileButton, Vector3.zero, Quaternion.identity);
            btn.GetComponent<Button>().name = tile.TileName;
            btn.GetComponent<Button>().onClick.AddListener(() => { OnTileButtonClick(key); });

            //根据Tpye加到不同的分页下
            switch (tile.TileType)
            {
                case TILE_BACKGROUND: btn.transform.SetParent(backgroundPage.transform); break;
                case TILE_MAPHASCOLLIDER: btn.transform.SetParent(mapHasColliderPage.transform); break;
                case TILE_MAPNOCOLLIDER: btn.transform.SetParent(mapNoColliderPage.transform); break;
                case TILE_ITEM: btn.transform.SetParent(itemsPage.transform); break;
                case TILE_NPC: btn.transform.SetParent(npcPage.transform); break;
                case TILE_ENEMY: btn.transform.SetParent(enemyPage.transform); break;
                default: break;
            }
        }     
    }
    //初始化已有Level列表
    void InitLevelList()
    {

    }
    void OnTileButtonClick(int _ID)
    {
        //设置当前选中Tile图标
        nowTileImage.GetComponent<Image>().sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        //创建tile
        GameObject tile = Instantiate(tilePrefabs[_ID], Input.mousePosition, Quaternion.identity);
    }

    /*各种Set*/
    //记录Tile数据并加入到tileList
    void SetTileDataAndAddToMapData(int _ID,Vector3Int _pos,int _layer)
    {
        TileData tileInfo = new TileData();
        tileInfo.tileId = _ID;
        tileInfo.position = _pos;
        tileInfo.layer = _layer;
        mapData.tileList.Add(tileInfo);
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
                    writer.WriteLine(tile.tileId + "#" + tile.position.x + "," + tile.position.y + "," + tile.position.z + "#" + tile.layer);
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
                FileStream fs = new FileStream(Application.streamingAssetsPath +"\\Level\\"+levelInfo.LevelFilePath, FileMode.Open);
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
    }
}
