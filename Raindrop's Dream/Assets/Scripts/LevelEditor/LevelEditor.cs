﻿using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor instance = null;
    /*UI*/
    public GameObject tileButton;//tile按钮预制体
    public GameObject levelButton;//已有关卡按钮
    //Right
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
    //Up
    public GameObject levelNameInputField;//关卡名输入框
    public GameObject makerNameInputField;//关卡制作者输入框
    public GameObject saveButton;//保存按钮

    /*Data*/
    private Dictionary<int, GameObject> tilePrefabs;//Tile预制体
    private int nowTileId;//当前选中TileID
    private int nowLevelId;//当前编辑关卡ID

    /*Object*/
    public GameObject tileMap;//tile父对象
    private GameObject nowTileObject;//当前选中tile
    /*Mouse*/
    private Vector3Int mousePos;//鼠标指针位置坐标
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
        nowLayer = -1;
        nowTileId = -1;
        nowLevelId = -1;
        tilePrefabs = new Dictionary<int, GameObject>();
        //初始化工具栏
        InitToolbars();
        //初始化Tile预制体
        InitTilePrefabs();
        //初始化Tile选择按钮
        InitTileButtons();
        //初始化已有Level列表
        InitLevelList();

    }

    // Update is called once per frame
    void Update()
    {
        //跟随鼠标移动，更新Tile位置
        if (mousePos != Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (nowTileObject != null)
            {
                nowTileObject.transform.position = new Vector3Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y), nowLayer);
            }
        }
        //左键点击放置Tile,已经有物体的位置不能放置
        if (nowTileObject != null)
        {
            if (Input.GetMouseButton(0) && IsValidPosition(mousePos))
            {
                if (FindTileInChild(new Vector3Int(mousePos.x, mousePos.y, nowLayer)) == null)
                {
                    SetNowTileToTileMap(nowTileId);
                }
            }
        }
        //右键清除当前选择or橡皮擦
        if (Input.GetMouseButton(1))
        {
            if (nowTileObject != null)
            {
                Destroy(nowTileObject);
                nowTileObject = null;
                nowTileImage.GetComponent<Image>().sprite = null;
                nowTileId = -1;
            }
            mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (IsValidPosition(mousePos))
            {
                GameObject des = FindTileInChild(new Vector3Int(mousePos.x, mousePos.y, nowLayer));
                if (des != null)
                {
                    Destroy(des.gameObject);
                }
            }
        }
        StartCoroutine(AutoSave());
    }

    /*各种初始化*/
    //初始化工具栏
    void InitToolbars()
    {

    }
    //加载TilePrefabs
    void InitTilePrefabs()
    {
        AssetBundle load = AssetBundle.LoadFromFile(PublicDataManager.DATA_PATH + "\\test.obj");
        foreach (int key in PublicDataManager.instance.GetTilePrefabTableKeys())
        {
            GameObject prefab = load.LoadAsset<GameObject>(PublicDataManager.instance.GetTilePrefabName(key));
            tilePrefabs.Add(key, prefab);
        }
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
            GameObject btn = null;
            //创建按钮绑定点击函数
            switch (PublicDataManager.instance.GetTilePrefabType(key))
            {
                case TILE_BACKGROUND: btn = Instantiate(tileButton, backgroundPage.transform); break;
                case TILE_PLATFORMHASCOLLIDER: btn = Instantiate(tileButton, platformHasColliderPage.transform); break;
                case TILE_PLATFORMNOCOLLIDER: btn = Instantiate(tileButton, platformNoColliderPage.transform); break;
                case TILE_ITEM: btn = Instantiate(tileButton, itemsPage.transform); break;
                case TILE_NPC: btn = Instantiate(tileButton, npcPage.transform); break;
                case TILE_ENEMY: btn = Instantiate(tileButton, enemyPage.transform); break;
                default: break;
            }
            if (btn != null)
            {
                btn.GetComponent<Button>().name = PublicDataManager.instance.GetTilePrefabName(key);
                btn.GetComponent<Image>().sprite = tilePrefabs[key].GetComponent<SpriteRenderer>().sprite;
                btn.GetComponent<Button>().onClick.AddListener(() => { OnTileButtonClick(key); });
            }
            //根据Tpye加到不同的分页下

            //初始显示背景分页
            SwitchTilePanel(TILE_BACKGROUND);
        }
    }
    //初始化已有Level列表
    void InitLevelList()
    {
        foreach (int key in PublicDataManager.instance.GetLevelTableKeys())
        {
            GameObject btn = Instantiate(levelButton, LevelPanel.transform);
            btn.name = PublicDataManager.instance.GetLevelName(key);
            btn.GetComponent<Image>().sprite = LoadLevelImage(key);
            btn.GetComponent<Button>().onClick.AddListener(() => { LoadLevel(key); });
        }
    }

    /*各种按钮响应函数*/
    //保存按钮
    void OnSaveButtonClick()
    {
        SaveLevel();
    }
    //tile按钮
    void OnTileButtonClick(int _ID)
    {
        if (nowTileObject != null)
        {
            Destroy(nowTileObject);
        }
        //设置当前ID
        nowTileId = _ID;
        //设置当前选中Tile图标
        nowTileImage.GetComponent<Image>().sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        //创建tile
        mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        nowTileObject = Instantiate(tilePrefabs[_ID], new Vector3Int(mousePos.x, mousePos.y, nowLayer), Quaternion.identity);
        nowTileObject.name = _ID.ToString();
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
    /*各种find*/
    //查找子对象
    GameObject FindTileInChild(Vector3Int _mpos)
    {
        GameObject pos;
        for (int i = 0; i < tileMap.transform.childCount; i++)
        {
            pos = tileMap.transform.GetChild(i).gameObject;
            if (Vector3Int.RoundToInt(pos.transform.position) == _mpos)
            {
                return pos;
            }
        }
        return null;
    }
    /*各种Set*/
    //放置tile
    void SetNowTileToTileMap(int _ID)
    {
        nowTileObject.transform.SetParent(tileMap.transform);

        mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        nowTileObject = Instantiate(tilePrefabs[_ID], new Vector3Int(mousePos.x, mousePos.y, nowLayer), Quaternion.identity);
        nowTileObject.name = _ID.ToString();
    }

    /*各种判断*/
    //放置位置是否合法
    bool IsValidPosition(Vector3 pos)
    {
        return pos.x >= 0 && pos.x <= MAX_WIDTH && pos.y >= 0 && pos.y <= MAX_HEIGHT;
    }

    /*读写Level*/
    //保存地图
    public void SaveLevel()
    {
        string levelName = levelNameInputField.GetComponent<InputField>().text;
        if (levelName == null)
        {
            //请输入关卡名
            return;
        }
        string makerName = makerNameInputField.GetComponent<InputField>().text;
        if (makerName == null)
        {
            //请输入制作者名
            return;
        }
        if (nowLevelId < 0)
        {
            nowLevelId = PublicDataManager.instance.GetLevelTableCount() + 1;
        }
        //文件夹路径：/Level/作者名/地图名文件夹
#if UNITY_IOS || UNITY_ANDROID      
        string saveDirPath = Application.persistentDataPath + "\\Level\\" +  makerName  + "\\" + levelName;
#elif UNITY_STANDALONE_WIN
        string saveDirPath = Application.streamingAssetsPath + "\\Level\\" + makerName + "\\" + levelName;
#endif
        if (!Directory.Exists(saveDirPath))
        {
            Directory.CreateDirectory(saveDirPath);
        }
        else
        {
            try
            {
                FileStream fs = new FileStream(saveDirPath + "\\" + levelName + ".level", FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);
                writer.WriteLine(nowLevelId);
                writer.WriteLine(levelName);
                writer.WriteLine(makerName);
                GameObject obj;
                for (int i = 0; i < tileMap.transform.childCount; i++)
                {
                    obj = tileMap.transform.GetChild(i).gameObject;
                    writer.WriteLine(obj.name + "#" + Mathf.Round(obj.transform.position.x) + "," + Mathf.Round(obj.transform.position.y) + "," + Mathf.Round(obj.transform.position.z));
                    //关卡封面
                }
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
                FileStream fs = new FileStream(Application.persistentDataPath +PublicDataManager.instance.GetLevelFilePath(_mapId)+".level", FileMode.Open);

#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            FileStream fs = new FileStream(Application.streamingAssetsPath + "\\Level\\"+PublicDataManager.instance.GetLevelFilePath(_mapId) + ".level", FileMode.Open);
#endif
            StreamReader reader = new StreamReader(fs);
            nowLevelId = int.Parse(reader.ReadLine());
            levelNameInputField.GetComponent<InputField>().text = reader.ReadLine();
            makerNameInputField.GetComponent<InputField>().text = reader.ReadLine();
            string tileInfoLine;//读取的一行
            string[] tileInfo;//以#分二段
            string[] posInfo;//position以,分三段
            while ((tileInfoLine = reader.ReadLine()) != null)
            {
                tileInfo = tileInfoLine.Split('#');

                posInfo = tileInfo[1].Split(',');
                Vector3Int position = new Vector3Int(int.Parse(posInfo[0]), int.Parse(posInfo[1]), int.Parse(posInfo[2]));

                GameObject obj = Instantiate(tilePrefabs[int.Parse(tileInfo[0])], position, Quaternion.identity);
                obj.name = tileInfo[0];
                obj.transform.SetParent(tileMap.transform);
            }

        }
        catch (Exception e)
        {
            //读取level文件失败\
            Debug.Log(e.ToString());
        }

    }
    //读取level封面
    private Sprite LoadLevelImage(int _mapId)
    {
        WWW www = new WWW("file:///" + PublicDataManager.DATA_PATH + PublicDataManager.instance.GetLevelFilePath(_mapId) + ".png");
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            return Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
        }
        else
            return null;
    }
    //自动保存
    IEnumerator AutoSave()
    {
        if (Time.time - lastSaveTime >= saveSpan)
        {
            if (tileMap.transform.childCount != 0)
            {
                SaveLevel();
            }
        }
        yield return null;
    }
}
