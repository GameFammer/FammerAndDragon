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
}
public class MapData
{
    public int mapId;//csv表中对应id
    public string mapName;//地图名称
    public string makerName;//地图制作者
    public List<TileData> tileList = new List<TileData>();//tile列表
}
public class LevelEditor : MonoBehaviour
{
    public static LevelEditor instance = null;
    /*UI*/
    public GameObject tileButton;//tile按钮预制体
    public GameObject levelButton;//已有关卡按钮

    public GameObject LevelPanel;//已有level面板
    public GameObject backgroundPage;//背景tile分页
    public GameObject platformHasColliderPage;//可碰撞地形组成分页
    public GameObject platformNoColliderPage;//不可碰撞地形组成分页
    public GameObject itemsPage;//道具分页
    public GameObject npcPage;//NPC分页
    public GameObject enemyPage;//enemy分页

    public GameObject backgroundPageBtn;//背景tile分页按钮
    public GameObject platformHasColliderPageBtn;//可碰撞地形组成分页按钮
    public GameObject platformNoColliderPageBtn;//不可碰撞地形组成分页按钮
    public GameObject itemsPageBtn;//道具分页按钮
    public GameObject npcPageBtn;//NPC分页按钮
    public GameObject enemyPageBtn;//enemy分页按钮

    public GameObject nowTileImage;//当前tile图标
    public GameObject saveButton;//保存按钮
    /*Data*/
    private MapData mapData;//地图信息数据
    private Dictionary<int, GameObject> tilePrefabs;//Tile预制体
    private int nowTileId;//当前选中TileID

    /*Object*/
    public GameObject tileMap;//tile父对象
    private GameObject nowTileObject;//当前选中tile
    /*Mouse*/
    private Vector3 mousePos;//鼠标指针位置坐标
    /*图层*/
    private int nowLayer;
    /*TileType*/
    private const int TILE_BACKGROUND = 0;
    private const int TILE_PLATFORMHASCOLLIDER = 1;
    private const int TILE_PLATFORMNOCOLLIDER = 2;
    private const int TILE_ITEM = 3;
    private const int TILE_NPC = 4;
    private const int TILE_ENEMY = 5;

    /*地图大小*/
    private const int MAX_WIDTH = 100;
    private const int MAX_HEIGHT = 50;
    private const int MAX_LAYERS = 5;

    /*自动保存*/
    public float saveSpan;//自动保存时间间隔
    private float lastSaveTime;//上次保存时间
    void Awake()
    {
        //单例，关卡切换不销毁
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }
    // Use this for initialization
    void Start()
    {
        nowLayer = 0;
        //初始化工具栏
        InitToolbars();
        //初始化Tile预制体
        InitTilePrefabs();
        //初始化Tile选择按钮
        InitTileButtons();
        //初始化已有Level列表
        InitLevelList();
        StartCoroutine(AutoSave());
    }

    // Update is called once per frame
    void Update()
    {
        //跟随鼠标移动，更新Tile位置
        if (mousePos != Camera.main.ScreenToWorldPoint(Input.mousePosition))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            nowTileObject.transform.position = new Vector3Int(Mathf.CeilToInt(mousePos.x), Mathf.CeilToInt(mousePos.y), nowLayer);
        }
        //左键点击放置Tile,已经有物体的位置不能放置
        if (Input.GetMouseButton(0) && IsValidPosition(mousePos))
        {
            GameObject des = tileMap.transform.Find(mousePos.x.ToString() + mousePos.y.ToString() + nowLayer.ToString()).gameObject;
            if (!des)
            {
                SetNowTileToTileMap(nowTileId);
            }
        }
        //右键
        if (Input.GetMouseButton(1) && IsValidPosition(mousePos))
        {
            GameObject des = tileMap.transform.Find(mousePos.x.ToString() + mousePos.y.ToString() + nowLayer.ToString()).gameObject;
            foreach (TileData t in mapData.tileList)
            {
                if (t.position == Vector3Int.CeilToInt(new Vector3(mousePos.x, mousePos.y, nowLayer)))
                {
                    mapData.tileList.Remove(t);
                }
            }
            DestroyImmediate(des);
        }
    }

    /*各种初始化*/
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
        //分页按钮
        backgroundPageBtn.GetComponent<Button>().onClick.AddListener(() => { SwitchTilePanel(TILE_BACKGROUND); });
        platformHasColliderPageBtn.GetComponent<Button>().onClick.AddListener(() => { SwitchTilePanel(TILE_PLATFORMHASCOLLIDER); });
        platformNoColliderPageBtn.GetComponent<Button>().onClick.AddListener(() => { SwitchTilePanel(TILE_PLATFORMNOCOLLIDER); });
        itemsPageBtn.GetComponent<Button>().onClick.AddListener(() => { SwitchTilePanel(TILE_ITEM); });
        npcPageBtn.GetComponent<Button>().onClick.AddListener(() => { SwitchTilePanel(TILE_NPC); });
        enemyPageBtn.GetComponent<Button>().onClick.AddListener(() => { SwitchTilePanel(TILE_ENEMY); });

        foreach (int key in PublicDataManager.instance.GetTilePrefabTableKeys())
        {
            //创建按钮绑定点击函数
            GameObject btn = Instantiate(tileButton, Vector3.zero, Quaternion.identity);
            btn.tag = key.ToString();
            btn.GetComponent<Button>().name = PublicDataManager.instance.GetTilePrefabName(key);
            btn.GetComponent<Image>().sprite = tilePrefabs[key].GetComponent<SpriteRenderer>().sprite;
            btn.GetComponent<Button>().onClick.AddListener(() => { OnTileButtonClick(key); });
            //根据Tpye加到不同的分页下
            switch (PublicDataManager.instance.GetTilePrefabType(key))
            {
                case TILE_BACKGROUND: btn.transform.SetParent(backgroundPage.transform); break;
                case TILE_PLATFORMHASCOLLIDER: btn.transform.SetParent(platformHasColliderPage.transform); break;
                case TILE_PLATFORMNOCOLLIDER: btn.transform.SetParent(platformNoColliderPage.transform); break;
                case TILE_ITEM: btn.transform.SetParent(itemsPage.transform); break;
                case TILE_NPC: btn.transform.SetParent(npcPage.transform); break;
                case TILE_ENEMY: btn.transform.SetParent(enemyPage.transform); break;
                default: break;
            }
            //初始显示背景分页
            SwitchTilePanel(TILE_BACKGROUND);
        }
    }
    //初始化已有Level列表
    void InitLevelList()
    {
        foreach (int key in PublicDataManager.instance.GetLevelTableKeys())
        {
            GameObject btn = Instantiate(levelButton, Vector3.zero, Quaternion.identity);
            btn.tag = key.ToString();
            btn.name = PublicDataManager.instance.GetLevelName(key);
            btn.GetComponent<Image>().sprite = LoadLevelImage(key);
            btn.GetComponent<Button>().onClick.AddListener(() => { LoadLevel(key); });
            btn.transform.SetParent(LevelPanel.transform);
        }
    }

    /*各种按钮响应函数*/
    //tile按钮
    void OnTileButtonClick(int _ID)
    {
        if (nowTileObject != null)
        {
            DestroyImmediate(nowTileObject);
        }
        //设置当前ID
        nowTileId = _ID;
        //设置当前选中Tile图标
        nowTileImage.GetComponent<Image>().sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        //创建tile
        SetNowTileToTileMap(_ID);
    }
    //切换tile分页
    void SwitchTilePanel(int _panelType)
    {
        backgroundPage.SetActive(false);
        platformHasColliderPage.SetActive(false);
        platformNoColliderPage.SetActive(false);
        itemsPage.SetActive(false);
        npcPage.SetActive(false);
        enemyPage.SetActive(false);
        switch (_panelType)
        {
            case TILE_BACKGROUND: backgroundPage.SetActive(true); break;
            case TILE_PLATFORMHASCOLLIDER: platformHasColliderPage.SetActive(true); break;
            case TILE_PLATFORMNOCOLLIDER: platformNoColliderPage.SetActive(true); break;
            case TILE_ITEM: itemsPage.SetActive(true); break;
            case TILE_NPC: npcPage.SetActive(true); break;
            case TILE_ENEMY: enemyPage.SetActive(true); break;
            default: break;
        }
    }

    /*各种Set*/
    //记录Tile数据并加入到tileList
    void SetTileDataAndAddToMapData(int _ID, Vector3Int _pos)
    {
        TileData tileInfo = new TileData();
        tileInfo.tileId = _ID;
        tileInfo.position = _pos;
        mapData.tileList.Add(tileInfo);
    }
    void SetNowTileToTileMap(int _ID)
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        nowTileObject = Instantiate(tilePrefabs[_ID], new Vector3Int(Mathf.CeilToInt(mousePos.x), Mathf.CeilToInt(mousePos.y), nowLayer), Quaternion.identity);
        nowTileObject.name = mousePos.x.ToString() + mousePos.y.ToString() + nowLayer.ToString();
        nowTileObject.tag = _ID.ToString();
        nowTileObject.transform.SetParent(tileMap.transform);
    }

    /*各种判断*/
    //放置位置是否合法
    bool IsValidPosition(Vector3 pos)
    {
        return pos.x >= 0 && pos.x <= MAX_WIDTH && pos.y >= 0 && pos.y <= MAX_HEIGHT && pos.z >= 0 && pos.z <= MAX_LAYERS;
    }

    /*读写Level*/
    //保存地图
    public void SaveLevel()
    {
        //文件夹路径：/Level/作者名/地图名文件夹
#if UNITY_IOS || UNITY_ANDROID      
        string saveDirPath = Application.persistentDataPath + "\\Level\\" + mapData.makerName + "\\" + mapData.mapName;
#elif UNITY_STANDALONE_WIN
        string saveDirPath = Application.streamingAssetsPath + "\\Level\\" + mapData.makerName + "\\" + mapData.mapName;
#endif
        if (!Directory.Exists(saveDirPath))
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
                    writer.WriteLine(tile.tileId + "#" + tile.position.x + "," + tile.position.y + "," + tile.position.z);
                }
                //关卡封面

                writer.Close();
            }
            catch (Exception e)
            {
                //文件写入失败

            }
        }
    }
    //从level文件读取Level
    public void LoadLevel(int _mapId)
    {
        try
        {
#if UNITY_IOS || UNITY_ANDROID
                FileStream fs = new FileStream(Application.persistentDataPath + "\\Level\\" + levelInfo.LevelFilePath, FileMode.Open);

#elif UNITY_STANDALONE_WIN
            FileStream fs = new FileStream(Application.streamingAssetsPath + PublicDataManager.instance.GetLevelFilePath(_mapId) + ".level", FileMode.Open);
#endif
            StreamReader reader = new StreamReader(fs);
            mapData.mapId = int.Parse(reader.ReadLine());
            mapData.mapName = reader.ReadLine();
            mapData.makerName = reader.ReadLine();
            string tileInfoLine;//读取的一行
            string[] tileInfo;//以#分二段
            string[] posInfo;//position以,分三段
            while ((tileInfoLine = reader.ReadLine()) != null)
            {
                tileInfo = tileInfoLine.Split('#');
                TileData t = new TileData();
                t.tileId = int.Parse(tileInfo[0]);

                posInfo = tileInfo[1].Split(',');
                t.position = new Vector3Int(int.Parse(posInfo[0]), int.Parse(posInfo[1]), int.Parse(posInfo[2]));

                GameObject obj = Instantiate(tilePrefabs[t.tileId], t.position, Quaternion.identity);
                nowTileObject.name = t.position.x.ToString() + t.position.y.ToString() + t.position.z.ToString();
                nowTileObject.tag = t.tileId.ToString();
                obj.transform.SetParent(tileMap.transform);

                mapData.tileList.Add(t);
            }

        }
        catch (Exception e)
        {
            //读取level文件失败
        }

    }
    //读取level封面
    private Sprite LoadLevelImage(int _mapId)
    {
#if UNITY_IOS || UNITY_ANDROID
                WWW www = new WWW("file://"+Application.persistentDataPath + PublicDataManager.instance.GetLevelFilePath(_mapId)+".png");

#elif UNITY_STANDALONE_WIN
        WWW www = new WWW("file:///"+Application.streamingAssetsPath + PublicDataManager.instance.GetLevelFilePath(_mapId) + ".png");
#endif
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            return Sprite.Create(www.texture,new Rect(0,0,www.texture.width,www.texture.height),Vector2.zero);
        }
        else
            return null;
    }
    //自动保存
    IEnumerator AutoSave()
    {
        if (Time.time - lastSaveTime >= saveSpan)
        {
            if (mapData.tileList.Count != 0)
            {
                SaveLevel();
            }
        }
        yield return null;
    }
}
