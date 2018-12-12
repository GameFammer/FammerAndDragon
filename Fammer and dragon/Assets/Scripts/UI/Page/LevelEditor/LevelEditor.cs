using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FDUI;
using Cinemachine;

public class LevelEditor : BasePage
{
    public static LevelEditor instance = null;
    /*UI*/
    [Header("预制体")]
    public GameObject tileButton;//tile按钮预制体
    public GameObject levelButton;//已有关卡按钮

    [SerializeField, HeaderAttribute("Panel")]
    public GameObject leftToolPanel;//左
    public GameObject rightTilePanel;//右
    public GameObject downLevelPanel;//下
    //Left
    [Header("左侧工具栏")]
    public Button layerBackgroundButton;//图层按钮
    public Button layerPlayerButton;
    public Button layerOverPlayerButton;
    public Toggle hideOtherToggle;//隐藏其它图层
    //Right
    [Header("右侧素材栏")]
    public GameObject staticScrollView;//用于切页
    public GameObject functionScrollView;
    public GameObject itemScrollView;
    public GameObject npcScrollView;
    public GameObject enemyScrollView;

    //用于设置tileButton父对象
    public GameObject staticPage;
    public GameObject functionPage;
    public GameObject itemsPage;
    public GameObject npcPage;
    public GameObject enemyPage;

    public Button staticPageBtn;//静止地形Tile分页按钮
    public Button functionPageBtn;//功能性地形Tile分页按钮
    public Button itemsPageBtn;//道具分页按钮
    public Button npcPageBtn;//NPC分页按钮
    public Button enemyPageBtn;//enemy分页按钮

    public Image nowTileImage;//当前tile图标
    //Up
    [Header("顶部功能栏")]
    public Button playButton;//开始测试按钮
    public Button stopButton;//结束测试按钮
    public Toggle hideUIToggle;//隐藏UI选项
    public InputField levelNameInputField;//关卡名输入框
    public InputField makerNameInputField;//关卡制作者输入框
    public Button saveButton;//保存按钮
    //Down
    [Header("底部关卡栏")]
    public GameObject LevelPanel;//已有level面板

    /*Data*/
    private Dictionary<string, GameObject> tilePrefabs;//Tile预制体
    private string nowTileName;//当前选中Tile名
    private string nowLevelName;//当前关卡名称
    private string nowMakerName;//当前关卡制作者名

    /*Object*/
    private Camera mainCamera;//主相机
    public CinemachineVirtualCamera sceneCamera;//场景相机
    public Transform sceneTarget;
    [Header("层级父对象")]
    public GameObject layerBackground;//最底层，例如背景等,对应Sorting Layer:Background
    public GameObject layerPlayer;//与主角同层物体，例如平台，敌人，道具等，对应Sorting Layer:Player
    public GameObject layerOverPlayer;//遮盖物，例如迷雾等,对应Sorting Layer:OverPlayer
    private GameObject player;//主角
    //Layer
    private const int LAYER_BACKGROUND = 8;
    private const int LAYER_PLAYER = 9;
    private const int LAYER_OVERPLAYER = 10;

    private Dictionary<int, TileInfo[,]> tileInfos;//存储Tile信息

    private GameObject nowTileObject;//当前选中tile
    private int nowTileObjectWidth;//当前选中tile宽度
    private int nowTileObjectHeight;//当前选中tile高度

    /*Mouse*/
    private Vector3Int mousePos;//鼠标指针位置坐标
    /*图层*/
    private int nowLayer;
    /*运行*/
    private bool isPlaying=false;//是否开始测试运行

    /*TileType*/
    private const int TILE_STATIC = 1;
    private const int TILE_FUNCTION = 2;
    private const int TILE_ITEM = 3;
    private const int TILE_NPC = 4;
    private const int TILE_ENEMY = 5;

    /*地图大小*/
    private const int MAX_WIDTH = 50;
    private const int MAX_HEIGHT = 50;

    /*自动保存*/
    [Header("自动保存时间间隔")]
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
        tilePrefabs = new Dictionary<string, GameObject>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerAction>().Sleep();
        //初始化Tile数组
        InitTileInfos();
        //初始化Tile预制体
        InitTilePrefabs();
        //初始化顶部功能按钮
        InitUpFunButtons();
        //初始化工具栏
        InitLeftToolButtons();
        //初始化Tile选择按钮
        InitRightTileButtons();
        //初始化已有Level列表按钮
        InitDownLevelButtons();
        nowLayer = LAYER_PLAYER;//默认Player层
        nowTileName = string.Empty;
        nowLevelName = string.Empty;

        //camera
        sceneCamera.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //跟随鼠标移动，更新Tile位置
        if(!isPlaying)
        {
            if (mousePos != Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (nowTileObject != null)
                {
                    nowTileObject.transform.position = new Vector3Int(mousePos.x, mousePos.y, 0);
                }
                ////长宽超过1并且是偶数增加偏移
                //if (nowTileObject != null)
                //{
                //    Vector3 pos = mousePos;
                //    if (nowTileObjectWidth > 1 && nowTileObjectWidth % 2 == 0)
                //    {
                //        pos.x -= 0.5f;
                //    }
                //    if (nowTileObjectHeight > 1 && nowTileObjectHeight % 2 == 0)
                //    {
                //        pos.y -= 0.5f;
                //    }
                //    nowTileObject.transform.position = new Vector3(pos.x, pos.y, 0);
                //}
            }
            //左键点击放置Tile,已经有物体的位置不能放置
            if (nowTileObject != null && !EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButton(0))
                {
                    //  Vector3 pos = GetTileBottomLetfPosition(nowTileObject,nowTileObjectWidth,nowTileObjectHeight);
                    if (IsValidPosition(mousePos))
                    {
                        if (FindTile(Vector3Int.RoundToInt(mousePos)) == null)
                        {
                            SetNowTileToTileMap(mousePos);
                        }
                    }

                }
            }
            //右键清除当前选择or橡皮擦
            if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (nowTileObject != null)
                {
                    Destroy(nowTileObject);
                    nowTileObject = null;
                    nowTileImage.GetComponent<Image>().sprite = null;
                    nowTileName ="";
                }
                Vector3Int pos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (IsValidPosition(pos))
                {
                    GameObject des = FindTile(pos);
                    if (des != null)
                    {
                        tileInfos[nowLayer][pos.x, pos.y].Reset();
                        Destroy(des.gameObject);
                    }
                }
            }
            //鼠标滑轮缩放
            if (!Equals(Input.GetAxis("Mouse ScrollWheel"), 0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (sceneCamera.m_Lens.OrthographicSize>= 0.5f)
                {
                    sceneCamera.m_Lens.OrthographicSize += Input.GetAxis("Mouse ScrollWheel");
                }
                else
                {
                    sceneCamera.m_Lens.OrthographicSize = 0.5f;
                }
            }
            //未开始运行水平、竖直输入移动相机Fllow
            if (!isPlaying && !EventSystem.current.IsPointerOverGameObject())
            {
                sceneTarget.position += new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * 5f, Input.GetAxis("Vertical") * Time.deltaTime * 5f, 0);
            }
            StartCoroutine(AutoSave());
        }      
    }

    /*各种初始化*/

    //初始化Tile数组
    void InitTileInfos()
    {
        tileInfos = new Dictionary<int, TileInfo[,]>();
        tileInfos.Add(LAYER_BACKGROUND, new TileInfo[MAX_WIDTH, MAX_HEIGHT]);
        tileInfos.Add(LAYER_PLAYER, new TileInfo[MAX_WIDTH, MAX_HEIGHT]);
        tileInfos.Add(LAYER_OVERPLAYER, new TileInfo[MAX_WIDTH, MAX_HEIGHT]);
        for (int i = 0; i < MAX_WIDTH; i++)
        {
            for (int j = 0; j < MAX_HEIGHT; j++)
            {
                tileInfos[LAYER_BACKGROUND][i, j] = new TileInfo();
            }
        }
        for (int i = 0; i < MAX_WIDTH; i++)
        {
            for (int j = 0; j < MAX_HEIGHT; j++)
            {
                tileInfos[LAYER_PLAYER][i, j] = new TileInfo();
            }
        }
        for (int i = 0; i < MAX_WIDTH; i++)
        {
            for (int j = 0; j < MAX_HEIGHT; j++)
            {
                tileInfos[LAYER_BACKGROUND][i, j] = new TileInfo();
            }
        }
        for (int i = 0; i < MAX_WIDTH; i++)
        {
            for (int j = 0; j < MAX_HEIGHT; j++)
            {
                tileInfos[LAYER_OVERPLAYER][i, j] = new TileInfo();
            }
        }
    }
    //初始化顶部功能栏
    void InitUpFunButtons()
    {
        hideUIToggle.onValueChanged.AddListener((_isOn) => HideUIPanel(hideUIToggle.GetComponent<Toggle>().isOn));
        saveButton.onClick.AddListener(() => OnSaveButtonClick());
        playButton.onClick.AddListener(() => OnPlayButtonClick());
        stopButton.onClick.AddListener(() => OnStopButtonClick());
    }
    //初始化左侧工具栏
    void InitLeftToolButtons()
    {
        layerBackgroundButton.onClick.AddListener(() => SetNowLayer(LAYER_BACKGROUND));
        layerPlayerButton.onClick.AddListener(() => SetNowLayer(LAYER_PLAYER));
        layerOverPlayerButton.onClick.AddListener(() => SetNowLayer(LAYER_OVERPLAYER));
        hideOtherToggle.onValueChanged.AddListener((_isOn) => HideOtherLayer(hideOtherToggle.GetComponent<Toggle>().isOn));
    }
    //加载TilePrefabs
    void InitTilePrefabs()
    {
        Dictionary<string, AssetBundle> assest = FDFileStream.ReadAllAssestBudle();
        foreach (string key in PublicDataManager.instance.GetSceneTileModelKeys())
        {
            string levelType = PublicDataManager.instance.GetSceneTileLevelType(key);
            GameObject prefab = assest[levelType].LoadAsset<GameObject>(PublicDataManager.instance.GetSceneTileName(key));
            tilePrefabs.Add(key, prefab);
        }
    }
    //初始化TileButton
    void InitRightTileButtons()
    {
        //分页按钮
        staticPageBtn.onClick.AddListener(() => { SwitchTilePanel(TILE_STATIC); });
        functionPageBtn.onClick.AddListener(() => { SwitchTilePanel(TILE_FUNCTION); });
        itemsPageBtn.onClick.AddListener(() => { SwitchTilePanel(TILE_ITEM); });
        npcPageBtn.onClick.AddListener(() => { SwitchTilePanel(TILE_NPC); });
        enemyPageBtn.onClick.AddListener(() => { SwitchTilePanel(TILE_ENEMY); });

        //SceneTile
        foreach (string key in PublicDataManager.instance.GetSceneTileModelKeys())
        {

            GameObject btn = null;
            //创建按钮绑定点击函数,根据Tpye加到不同的分页下
            switch (PublicDataManager.instance.GetSceneTileType(key))
            {
                case TILE_STATIC: btn = Instantiate(tileButton, staticPage.transform); break;
                case TILE_FUNCTION: btn = Instantiate(tileButton, functionPage.transform); break;
                default: btn = Instantiate(tileButton, staticPage.transform); break;
            }
            if (btn != null)
            {
                btn.GetComponent<Button>().name = PublicDataManager.instance.GetSceneTileName(key);
                if (tilePrefabs[key].GetComponent<SpriteRenderer>())
                {
                    btn.GetComponent<Image>().sprite = tilePrefabs[key].GetComponent<SpriteRenderer>().sprite;
                }
                btn.GetComponent<Button>().onClick.AddListener(() => { OnTileButtonClick(key); });
            }
        }

        //初始显示背景分页
        SwitchTilePanel(TILE_STATIC);
    }
    //初始化已有Level列表
    void InitDownLevelButtons()
    {
        foreach (string key in PublicDataManager.instance.GetLevelModelKeys())
        {
            GameObject btn = Instantiate(levelButton, LevelPanel.transform);
            btn.name = PublicDataManager.instance.GetLevelName(key);
          //  btn.GetComponent<Image>().sprite = LoadLevelImage(key);
            btn.GetComponent<Button>().onClick.AddListener(() => { RefreshLevel(); LoadLevel(key); });
        }

    }

    /*各种按钮响应函数*/
    //隐藏左右下UI按钮
    void HideUIPanel(bool _isOn)
    {
        if (_isOn)
        {
            leftToolPanel.SetActive(false);
            rightTilePanel.SetActive(false);
            downLevelPanel.SetActive(false);
        }
        else
        {
            leftToolPanel.SetActive(true);
            rightTilePanel.SetActive(true);
            downLevelPanel.SetActive(true);
        }
    }
    //隐藏其它图层选项
    void HideOtherLayer(bool _isOn)
    {
        if (_isOn)
        {
            layerBackground.SetActive(false);
            layerPlayer.SetActive(false);
            layerOverPlayer.SetActive(false);
            GetLayerObject(nowLayer).SetActive(true);
        }
        else
        {
            layerBackground.SetActive(true);
            layerPlayer.SetActive(true);
            layerOverPlayer.SetActive(true);
        }
    }
    //开始测试按钮
    public void OnPlayButtonClick()
    {
        isPlaying = true;
        //显示主角
        player.SetActive(true);
        player.GetComponent<PlayerAction>().EnterNextLevel();
        player.GetComponent<PlayerAction>().WakeUp();
        //激活物理组件
        SetAllChildPhysicsActivated(layerBackground.transform);
        SetAllChildPhysicsActivated(layerPlayer.transform);
        SetAllChildPhysicsActivated(layerOverPlayer.transform);
        
        //隐藏UI
        leftToolPanel.SetActive(false);
        rightTilePanel.SetActive(false);
        downLevelPanel.SetActive(false);

        //切换Camera
        sceneCamera.enabled = false;
    }
    //结束测试按钮
    void OnStopButtonClick()
    {
        isPlaying = false;
        //隐藏主角
        player.GetComponent<PlayerAction>().Sleep();
        player.SetActive(false);
        //冻结物理组件
        SetAllChildPhysicsFrozen(layerBackground.transform);
        SetAllChildPhysicsFrozen(layerPlayer.transform);
        SetAllChildPhysicsFrozen(layerOverPlayer.transform);

        //显示UI
        leftToolPanel.SetActive(true);
        rightTilePanel.SetActive(true);
        downLevelPanel.SetActive(true);

        //切换Camera
        sceneCamera.enabled = true;
    }
    //保存按钮
    void OnSaveButtonClick()
    {
        SaveLevel();
    }
    //tile按钮
    void OnTileButtonClick(string _name)
    {
        if (!isPlaying)
        {
            if (nowTileObject != null)
            {
                Destroy(nowTileObject);
            }
            //设置当前名
            nowTileName = _name;
            //设置当前选中Tile图标
            nowTileImage.sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
            //创建tile
            CreatTileObjectOnMousePosition();
        }
    }
    //切换tile分页
    void SwitchTilePanel(int _panelType)
    {
        staticScrollView.SetActive(false);
        functionScrollView.SetActive(false);
        itemScrollView.SetActive(false);
        npcScrollView.SetActive(false);
        enemyScrollView.SetActive(false);
        switch (_panelType)
        {
            case TILE_STATIC: staticScrollView.SetActive(true); break;
            case TILE_FUNCTION: functionScrollView.SetActive(true); break;
            case TILE_ITEM: itemScrollView.SetActive(true); break;
            case TILE_NPC: npcScrollView.SetActive(true); break;
            case TILE_ENEMY: enemyScrollView.SetActive(true); break;
            default: break;
        }
    }
    //切换关卡刷新显示
    void RefreshLevel()
    {
        for (int i = 0; i < layerBackground.transform.childCount; i++)
        {
            Destroy(layerBackground.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < layerPlayer.transform.childCount; i++)
        {
            Destroy(layerPlayer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < layerOverPlayer.transform.childCount; i++)
        {
            Destroy(layerOverPlayer.transform.GetChild(i).gameObject);
        }
    }
    /*各种find*/
    //根据图层查找子对象
    GameObject FindTile(Vector3Int _mousePosition)
    {
        if (tileInfos[nowLayer][_mousePosition.x, _mousePosition.y].isEmpty)
        {
            return null;
        }
        else
        {
            return tileInfos[nowLayer][_mousePosition.x, _mousePosition.y].tileObjcet;
        }
    }

    /*各种Set/Get*/
    //根据当前图层放置tile
    void SetNowTileToTileMap(Vector3 _pos)
    {
        if (!isPlaying)
        {
            nowTileObject.transform.SetParent(GetLayerObject(nowLayer).transform);
            nowTileObject.layer = GetLayerObject(nowLayer).layer;

            SetTileInfo(_pos, nowTileName, nowTileObject, nowTileObject.layer);

            CreatTileObjectOnMousePosition();
        }
    }
    //设置Tile数据
    void SetTileInfo(Vector3 _BottomLeftPos, string _name, GameObject _tileObject, int _layer)
    {
        int x = Mathf.RoundToInt(_BottomLeftPos.x);
        int y = Mathf.RoundToInt(_BottomLeftPos.y);

        tileInfos[_layer][x, y].name = _name;
        tileInfos[_layer][x, y].isEmpty = false;
        tileInfos[_layer][x, y].position = _tileObject.transform.position;
        tileInfos[_layer][x, y].rotation = _tileObject.transform.rotation;
        tileInfos[_layer][x, y].scale = _tileObject.transform.localScale;
        tileInfos[_layer][x, y].tileObjcet = _tileObject;
        tileInfos[_layer][x, y].layer = _layer;

    }
    //从鼠标位置创建Tile
    void CreatTileObjectOnMousePosition()
    {
        nowTileObjectWidth = 1;
        nowTileObjectHeight = 1;
        mousePos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        nowTileObject = Instantiate(tilePrefabs[nowTileName], new Vector3Int(mousePos.x, mousePos.y, 0), tilePrefabs[nowTileName].transform.rotation);
        if (nowTileObject.GetComponent<PublicProperties>()!=null)
        {
            nowTileObjectWidth = nowTileObject.GetComponent<PublicProperties>().width;
            nowTileObjectHeight = nowTileObject.GetComponent<PublicProperties>().height;
        }
        ////增加偏移
        //Vector3 pos = mousePos;
        //if (nowTileObjectWidth > 1 && nowTileObjectWidth % 2 == 0)
        //{
        //    pos.x -= 0.5f;
        //}
        //if (nowTileObjectHeight > 1 && nowTileObjectHeight % 2 == 0)
        //{
        //    pos.y -= 0.5f;
        //}
        //nowTileObject.transform.position = pos;

        //禁用物理组件
        if (nowTileObject.GetComponent<Collider2D>())
        {
            nowTileObject.GetComponent<Collider2D>().enabled = false;
        }
        if (nowTileObject.GetComponent<Rigidbody2D>())
        {
            nowTileObject.GetComponent<Rigidbody2D>().simulated = false;
        }
    }
    //设置当前图层
    void SetNowLayer(int _layer)
    {
        nowLayer = _layer;
        HideOtherLayer(hideOtherToggle.GetComponent<Toggle>().isOn);
    }
    ////获取Tile左下角块坐标,需要移除偏移来对应数组下标。
    //Vector3 GetTileBottomLetfPosition(GameObject _tile,int _width,int _height)
    //{
    //    Vector3 pos = _tile.transform.position;
    //    if (_width > 1)
    //    {
    //        if (_width % 2 == 0)
    //        { 
    //            pos.x += 0.5f;
    //            pos.x -= _width / 2.0f;
    //        }
    //        else
    //        {
    //            pos.x -= (_width - 1) / 2.0f;
    //        }
    //    }
    //    if (_height > 1)
    //    {
    //        if (_height % 2 == 0)
    //        {
    //            pos.y += 0.5f;
    //            pos.y -= _height / 2.0f;
    //        }
    //        else
    //        {
    //            pos.y -= (_height - 1) / 2.0f;
    //        }
    //    }
    //    return pos;
    //}
    //获取当前层对象
    GameObject GetLayerObject(int _layer)
    {
        switch (_layer)
        {
            case LAYER_BACKGROUND: return layerBackground;
            case LAYER_PLAYER: return layerPlayer;
            case LAYER_OVERPLAYER: return layerOverPlayer;
            default: return layerPlayer;
        }
    }
    //激活所有子物体
    void SetAllChildPhysicsActivated(Transform _father)
    {
        foreach (Transform t in _father)
        {
            if (t.GetComponent<Collider2D>())
            {
                t.GetComponent<Collider2D>().enabled = true;
            }
            if (t.GetComponent<ILevelEditor>() != null)
            {
                t.GetComponent<ILevelEditor>().WakeUp();
            }
        }
    }
    //冻结所有子物体
    void SetAllChildPhysicsFrozen(Transform _father)
    {
        foreach (Transform t in _father)
        {
            if (t.GetComponent<Collider2D>())
            {
                t.GetComponent<Collider2D>().enabled = false;
            }
            if (t.GetComponent<ILevelEditor>() != null)
            {
                t.GetComponent<ILevelEditor>().Sleep();
            }
        }
    }
    ////获取全部tile信息列表
    //List<TileInfo> GetTileInfoList()
    //{
    //    List<TileInfo> tiles = new List<TileInfo>();
    //    GetLayerTileInfos(ref tiles, layerBackground);
    //    GetLayerTileInfos(ref tiles, layerPlayer);
    //    GetLayerTileInfos(ref tiles, layerOverPlayer);
    //    return tiles;
    //}
    ////获取某层的tile信息
    //void GetLayerTileInfos(ref List<TileInfo> _tile, GameObject _layer)
    //{
    //    GameObject obj;
    //    for (int i = 0; i < _layer.transform.childCount; i++)
    //    {
    //        obj = _layer.transform.GetChild(i).gameObject;
    //        TileInfo tile = new TileInfo();
    //        tile.id = int.Parse(obj.name);
    //        tile.pos = Vector3Int.RoundToInt(obj.transform.position);
    //        _tile.Add(tile);
    //    }
    //}
    /*各种判断*/
    //放置位置是否合法
    bool IsValidPosition(Vector3 pos)
    {
        return pos.x >= 0 && pos.x < MAX_WIDTH && pos.y >= 0 && pos.y < MAX_HEIGHT;
    }

    /*读写Level*/

    //保存地图
    public void SaveLevel()
    {
        nowLevelName = levelNameInputField.text;
        if (nowLevelName == string.Empty)
        {
            //请输入关卡名
            return;
        }
        nowMakerName = makerNameInputField.text;
        if (nowMakerName == string.Empty)
        {
            //请输入制作者名
            return;
        }
        LevelInfo level = new LevelInfo();
        level.name = nowLevelName;
        level.name = nowLevelName;
        level.producer = nowMakerName;

        foreach (int key in tileInfos.Keys)
        {
            foreach (TileInfo tile in tileInfos[key])
            {
                if (!tile.isEmpty)
                {
                    level.tiles.Add(tile);
                }
            }
        }

        FDFileStream.WriteLevelFile(level);
        //存取封面

        //写入CSV

    }

    //从level文件读取Level
    public void LoadLevel(string _levelName)
    {
        LevelInfo level = FDFileStream.ReadLevelFile(_levelName);
        if (!level.IsEmpty())
        {
            nowLevelName = level.name;
            levelNameInputField.GetComponent<InputField>().text = level.name;
            makerNameInputField.GetComponent<InputField>().text = level.producer;
            foreach (TileInfo tile in level.tiles)
            {
                GameObject obj = Instantiate(tilePrefabs[tile.name], tile.position, tile.rotation);
                if (obj.GetComponent<Collider2D>())
                {
                    obj.GetComponent<Collider2D>().enabled = false;
                }
                if (obj.GetComponent<ILevelEditor>()!=null)
                {
                    obj.GetComponent<ILevelEditor>().Sleep();
                }
                obj.transform.localScale = tile.scale;
                obj.transform.SetParent(GetLayerObject(tile.layer).transform);
                tile.tileObjcet = obj;
                //int width = obj.GetComponent<PublicProperties>().width;
                //int height = obj.GetComponent<PublicProperties>().height;
                //Vector3 pos = GetTileBottomLetfPosition(obj, width, height);

                SetTileInfo(tile.position, tile.name, obj, tile.layer);
            }
        }
    }
    //读取level封面
    //private Sprite LoadLevelImage(int _mapId)
    //{
    //    WWW www = new WWW("file:///" + FDPlatform.DATA_PATH + PublicDataManager.instance.GetLevelFilePath(_mapId) + ".png");
    //    if (www != null && string.IsNullOrEmpty(www.error))
    //    {
    //        return Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
    //    }
    //    else
    //        return null;
    //}
    //自动保存
    IEnumerator AutoSave()
    {
        nowLevelName = levelNameInputField.text;
        nowMakerName = makerNameInputField.text;
        if (nowLevelName != null && nowMakerName != null)
        {
            if (Time.time - lastSaveTime >= saveSpan)
            {
                if (layerBackground.transform.childCount != 0 || layerPlayer.transform.childCount != 0 || layerOverPlayer.transform.childCount != 0)
                {
                    SaveLevel();
                }
            }
            yield return null;
        }
    }
}
