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

    public static BulletsPool bulletsPool = null;//单例
    public GameObject[] bulletObjs;//子弹prefab
    public int poolInitSize = 5;//池初始大小
    public bool isLockPoolSize = false;//是否锁定池大小

    //各种类型子弹的子弹列表
    private Dictionary<string, List<GameObject>> bullets = new Dictionary<string, List<GameObject>>();
    //记录每种子弹池的当前位置下标
    private Dictionary<string, int> currentIndexs = new Dictionary<string, int>();
    //维护一个子弹Tag和GameObject构成的字典，方便对子弹池进行操作
    private Dictionary<string, GameObject> bulletDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        //单例模式
        bulletsPool = this;
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    public void init(GameObject[] _bulletObjs)
    {
        //初始化子弹列表
        foreach(GameObject bullet in _bulletObjs)
        {
            bulletDictionary.Add(bullet.tag, bullet); 
            initOneTypeBulletsByOneBullet(bullet);
        }
    }

    private void initOneTypeBulletsByOneBullet(GameObject _bullet)
    {
        bullets.Add(_bullet.tag, new List<GameObject>());
        currentIndexs.Add(_bullet.tag, 0);

        //初始化List
        for (int i = 0; i < poolInitSize; i++)
        {
            GameObject bullet = Instantiate(_bullet);
            bullet.SetActive(false);
            bullets[_bullet.tag].Add(bullet);
        }
    }

    public GameObject GetBulletByTag(string _bulletTag)
    {
        //从当前位置开始遍历
        for (int i = 0; i < bullets[_bulletTag].Count; i++)
        {
            int index = (currentIndexs[_bulletTag] + i) % bullets[_bulletTag].Count;

            //找到未被激活的返回
            if (!bullets[_bulletTag][currentIndexs[_bulletTag]].activeInHierarchy)
            {
                //当前位置后移
                currentIndexs[_bulletTag] = (index + 1) % bullets[_bulletTag].Count;
                return bullets[_bulletTag][currentIndexs[_bulletTag]];
            }
        }

        //池中子弹都已经被使用，若没有锁定对象池大小，则创建子弹并添加到对象池中。
        if (!isLockPoolSize)
        {
            GameObject bullet = bulletDictionary[_bulletTag];
            bullets[_bulletTag].Add(bullet);
            return bullet;
        }

        //无可用子弹且锁定对象池大小，返回空
        return null;
    }
}
