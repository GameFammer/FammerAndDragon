/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-4-16
  *Description:bullet move and hit.
  *Changes:
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour
{

    public int damage;//伤害
    public float speed;//速度
    public float activeTime;//存在时间

    public HitEffect hitEffect;
    public MoveAction moveAction;
    public HitAction hitAciton;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //开启定时“销毁”协程
        StartCoroutine(SetNotActive(activeTime));
    }
    void FixedUpdate()
    {

        //子弹移动
        moveAction.Move(speed); 
        //射线检测碰撞
        hitAciton.Hit(damage, hitEffect);
    }
    //定时“销毁”子弹
    IEnumerator SetNotActive(float _activeTime)
    {
        yield return new WaitForSeconds(_activeTime);
        this.gameObject.SetActive(false);
    }
}
