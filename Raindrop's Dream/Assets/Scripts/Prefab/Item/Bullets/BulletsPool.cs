/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-4-9
  *Description: player子弹池
  *Changes:
**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsPool : MonoBehaviour
{

    public static BulletsPool instance = null;//单例
    public GameObject[] bulletObjs;//子弹prefab
    public int poolInitSize = 5;//池初始大小
    public bool isLockPoolSize = false;//是否锁定池大小

    //各种类型子弹的子弹列表 {"子弹名", 子弹池}
    private Dictionary<string, List<GameObject>> bullets = new Dictionary<string, List<GameObject>>();
    //记录每种子弹池的当前位置下标 {"子弹名", 子弹池当前位置}
    private Dictionary<string, int> currentIndexs = new Dictionary<string, int>();
    //维护一个子弹字典，方便对子弹池进行操作 {"子弹名", 子弹}
    private Dictionary<string, GameObject> bulletDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        //单例模式
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
   
    private void Start()
    {
        Init(bulletObjs);
    }

    // Use this for initialization
    public void Init(GameObject[] _bulletObjs)
    {
        //初始化子弹列表
        foreach (GameObject bullet in _bulletObjs)
        {
            //将该子弹加入子弹字典
            bulletDictionary.Add(bullet.name, bullet);
            InitOneTypeBulletsByName(bullet.name);
        }
    }

    //初始化该子弹对应的子弹池
    private void InitOneTypeBulletsByName(string _bulletName)
    {
        //创建该类型子弹的子弹池
        bullets.Add(_bulletName, new List<GameObject>());
        //记录该类型子弹池已使用子弹的编号
        currentIndexs.Add(_bulletName, 0);

        //初始化该类型子弹的子弹池
        for (int i = 0; i < poolInitSize; i++)
        {
            AddBulletToBulletsByName(_bulletName);
        }
    }

    public GameObject GetBulletByName(string _bulletName)
    {
        //从当前位置开始遍历
        for (int i = 0; i < bullets[_bulletName].Count; i++)
        {
            int index = (currentIndexs[_bulletName] + i) % bullets[_bulletName].Count;

            //找到未被激活的返回
            if (!bullets[_bulletName][currentIndexs[_bulletName]].activeInHierarchy)
            {
                //当前位置后移
                currentIndexs[_bulletName] = (index + 1) % bullets[_bulletName].Count;
                //返回当前找到的可用子弹
                return bullets[_bulletName][currentIndexs[_bulletName]];
            }
        }

        //池中子弹都已经被使用，若没有锁定对象池大小，则创建子弹并添加到对象池中。
        if (!isLockPoolSize)
        {
            AddBulletToBulletsByName(_bulletName);
            //返回刚才添加的最后一枚子弹
            return bullets[_bulletName][bullets[_bulletName].Count - 1];
        }

        //无可用子弹且锁定对象池大小，返回空
        return null;
    }

    //给该类型子弹池加入一个子弹
    private void AddBulletToBulletsByName(string _bulletName)
    {
        GameObject bullet = Instantiate(bulletDictionary[_bulletName]);
        bullet.SetActive(false);
        bullets[_bulletName].Add(bullet);
    }
}
