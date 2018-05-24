/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  //完成日期 
  *Description:  //用于主要说明此程序文件完成的主要功能 
                //与其他模块或函数的接口、输出值、取值范围、 
                //含义及参数间的控制、顺序、独立及依赖关系 
  *Changes://修改记录
**********************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;
public class MapManager : MonoBehaviour
{
    public static MapManager mapManager = null;
    public int tileSize = 4;
    public GameObject tile;
    private List<Room> rooms=new List<Room>();//房间列表
    private List<Room> mainRooms= new List<Room>();//主房间列表
    private List<GameObject> mainRoomsObject = new List<GameObject>();
    private List<Vector3> preUpdatePos = new List<Vector3>();
    private bool isCreatedCorridor = false;
    void Awake()
    {
        if (mapManager == null)
            mapManager = this;
        else if (mapManager != this)
            Destroy(gameObject);

    }
    void Start()
    {
        RandomCreatMap();
        
    }

    // Update is called once per frame
    void Update()
    {
       if (!isCreatedCorridor)
        {
            if (IsSeparated())
            {
                CreatCorridor();
                isCreatedCorridor = true;
            }
        }
    }
    //随机生成地图
    public void RandomCreatMap()
    {
        InitMap();//初始化地图
        foreach (Room r in rooms)
        {
            GameObject b = Instantiate(tile, r.position, Quaternion.identity);
            b.transform.localScale = new Vector3(r.width, r.height, 1);
            if (r.isMainRoom)
            {
                b.GetComponent<Renderer>().material.color = Color.red;
                mainRoomsObject.Add(b);
                preUpdatePos.Add(b.transform.position);
            }
        }
    }

    //初始化地图
    void InitMap()
    {
        for(int i = 0; i < 100; i++)
        {
            Room newRoom=new Room();
            newRoom.id = i;
            newRoom.isMainRoom = false;
            newRoom.position = GetRandomPointInEllipse(40, 20);//获取房间随机坐标
            Vector2 roomSize = new Vector2(Random.Range(20,100), Random.Range(20, 100));//获取房间随机宽高
            newRoom.width = roomSize.x;
            newRoom.height = roomSize.y;
            //判断是否为主房间
            if (roomSize.x > 60 || roomSize.y > 60)
            {
                float thresholdValue = roomSize.x / roomSize.y;//宽高比
                if (thresholdValue < 1.25f && thresholdValue > 0.8f)
                {
                    newRoom.isMainRoom = true;
                    mainRooms.Add(newRoom);//加入主房间列表
                }
            }
           
            rooms.Add(newRoom);//加入房间列表
        }
    }
    //获取椭圆内随机点
    Vector2 GetRandomPointInEllipse(int _width,int _height)
    {
        float randomR1 = 2 * Mathf.PI * Random.Range(0, 1.0f);//随机弧度
        float randomR2 = Random.Range(0,1.0f) + Random.Range(0, 1.0f);//随机弧度
        float r = 0;
        if (randomR2 > 1)
        {
            r = 2 - randomR2;
        }
        else
        {
            r = randomR2;
        }
        Vector2 randomPos = new Vector2(RandomFixTileSize(_width * r*Mathf.Cos(randomR1),tileSize),RandomFixTileSize(_height*r*Mathf.Sin(randomR1),tileSize));
        return randomPos;
    }
    //使随机坐标满足tile大小的公倍数
    float RandomFixTileSize(float _a,float _b)
    {
        return Mathf.Floor(((_a + _b - 1) / _b) * _b);
    }
    //生成走廊
    public void CreatCorridor()
    {
        List<Vector2> points = new List<Vector2>();//主房间中点坐标列表
        foreach(Room r in mainRooms)
        {
            points.Add(r.position);
        }
       
        Rect size = new Rect(-100,100, 1000, 1000);
        Voronoi voronoi = new Voronoi(points, null, size); //使用Delaunay库

        List<LineSegment> spanningTree = voronoi.SpanningTree("minimum");//获取最小生成树
        foreach (LineSegment point in spanningTree)
        {
                Debug.DrawLine(point.p0.position, point.p1.position, Color.white,1000);
        }
    }
    public bool IsSeparated()
    {
        for(int i = 0; i < mainRoomsObject.Count; i++)
        {
            
            if (mainRoomsObject[i].transform.position != preUpdatePos[i])
            {
                preUpdatePos[i] = mainRoomsObject[i].transform.position;
                return false;
            }
            mainRooms[i].position = mainRoomsObject[i].transform.position;
        }
        return true;
    }
    //取房间中点坐标
    Vector2 GetMidPos(Room r)
    {
        return new Vector2(r.position.x + r.width / 2.0f, r.position.y - r.height / 2.0f);
    }

}
