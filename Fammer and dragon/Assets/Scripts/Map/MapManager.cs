using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public const int MAX_WIDTH = 1100; //地图最大宽度
    public const int MAX_HEIGHT = 400; //地图最大高度
    public const int UNDERGROUND_HEIGHT = 300;//地平线以上高度
    public const int MIN_LEFT_X = 300; //左侧大陆最小X坐标
    public const int MAX_LEFT_X = 350; //左侧大陆最大X坐标
    public const int MIN_RIGHT_X = 800;//右侧大陆最小X坐标
    public const int MAX_RIGHT_X = 850;//右侧大陆最大X坐标
    public const int MIN_SPACE_WIDTH = 30;//最小大陆间距
    public const int MAX_SPACE_WIDTH = 70;//最大大陆间距


    private float hillExponent = 1f; //调整山脉高度指数
    private int seed;//地图种子
    public  int Seed
    {
        get;
        set;
    }
   
    public Tile tileTset;
    /*Nosie*/
    public FastNoiseUnity groundNoise;
    public FastNoiseUnity caveNoise;

    /*TileMap*/
    public Tilemap block;
    /*test*/


 

    // Use this for initialization
    void Start()
    {
        //Test
        Random.InitState((int)System.DateTime.Now.Ticks);
        seed = Random.Range(0, 10000);
        Generate(seed);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Generate(int _seed)
    {
        seed = _seed;
        StartCoroutine("GenerateMap");
    }
    IEnumerator GenerateMap()
    {
       
        groundNoise.fastNoise.SetSeed(seed);
        caveNoise.fastNoise.SetSeed(seed);

        int overGroundHeight = MAX_HEIGHT - UNDERGROUND_HEIGHT;
        int leftX = Random.Range(MIN_LEFT_X, MAX_LEFT_X);
        int rightX = Random.Range(MIN_RIGHT_X, MAX_RIGHT_X);
        int spaceWidthLeft = Random.Range(MIN_SPACE_WIDTH, MAX_SPACE_WIDTH);
        int spaceWidthRight = Random.Range(MIN_SPACE_WIDTH, MAX_SPACE_WIDTH);
        for (int i = 0; i < MAX_WIDTH; i++)
        {
            //左侧间隔
            if (i > leftX && i <= leftX + spaceWidthLeft)
            {

            }
            //右侧间隔
            else if (i >= rightX - spaceWidthRight && i < rightX)
            {

            }
            else
            {
                //地上部分
                float h1 = Mathf.Abs(groundNoise.fastNoise.GetNoise(i, i / 3f));
                //调整高度
                int overH = Mathf.FloorToInt(Mathf.Pow(h1, hillExponent) * overGroundHeight);
                Debug.Log(overH);
                for (int j = 0; j < overH; j++)
                {
                    //Instantiate(test, new Vector3(i, j + undergroundHeight, 0), Quaternion.identity);
                    block.SetTile(new Vector3Int(i, j + UNDERGROUND_HEIGHT, 0), tileTset);
                }
                //地下部分
                for (int j = 0; j < UNDERGROUND_HEIGHT; j++)
                {
                    //洞穴
                    float value = Mathf.Abs(caveNoise.fastNoise.GetNoise(i, j));
                    if (value < 0.5f)
                    {
                        //Instantiate(test, new Vector3(i, j, 0), Quaternion.identity);
                        block.SetTile(new Vector3Int(i, j, 0), tileTset);
                    }
                }
            }

        }
        yield return null;
    }
}
