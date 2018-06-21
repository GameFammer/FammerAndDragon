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

    public static BulletsPool playerBulletsPool = null;//单例
    public GameObject bulletObj;//子弹prefab
    public int poolInitSize = 5;//池初始大小
    public bool isLockPoolSize = false;//是否锁定池大小

    private List<GameObject> bullets;//子弹列表
    private int currentIndex = 0;//当前位置下标
    private void Awake()
    {
        //单例模式
        if (playerBulletsPool == null)
            playerBulletsPool = this;
        else if (playerBulletsPool != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start()
    {
        //初始化List
        bullets = new List<GameObject>();
        for (int i = 0; i < poolInitSize; i++)
        {
            GameObject obj = Instantiate(bulletObj);
            obj.SetActive(false);
            bullets.Add(obj);
        }
    }
    public GameObject GetBullet()
    {
        //从当前位置开始遍历
        for (int i = 0; i < bullets.Count; i++)
        {
            int index = (currentIndex + i) % bullets.Count;
            //找到未被激活的返回
            if (!bullets[currentIndex].activeInHierarchy)
            {
                //当前位置后移
                currentIndex = (index + 1) % bullets.Count;
                return bullets[index];
            }
        }
        //池中子弹都已经被使用，若没有锁定对象池大小，则创建子弹并添加到对象池中。
        if (!isLockPoolSize)
        {
            GameObject obj = Instantiate(bulletObj);
            bullets.Add(obj);
            return obj;
        }

        //无可用子弹且锁定对象池大小，返回空
        return null;
    }
}
