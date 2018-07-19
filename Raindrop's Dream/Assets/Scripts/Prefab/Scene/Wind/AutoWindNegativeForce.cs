/********************************************************************************* 
  *Author:AICHEN
  *Date:2018.7.16
  *Description:风力漩涡-减速区域-根据速度期望值动态设置减速区域力大小
**********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoWindNegativeForce : MonoBehaviour, ISleepWakeUp
{
    [Header("离开时速度期望值")]
    public float exitSpeed;
    [Header("力最大值")]
    public float maxForce;
    [Header("是否为水平方向")]
    public bool isHorizontal;
    private AreaEffector2D areaEffector;
    // Use this for initialization
    void Start()
    {
        areaEffector = GetComponent<AreaEffector2D>();
    }
    void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.gameObject.tag == "Player")
        {
            _col.gameObject.GetComponent<PlayerAction>().ChangeStatus(PlayerStatus.Player_Weightlessness);
            float speed = 0;
            if (isHorizontal)
            {
                speed = Mathf.Abs(_col.gameObject.GetComponent<Rigidbody2D>().velocity.x);
            }

            else
            {
                speed = Mathf.Abs(_col.gameObject.GetComponent<Rigidbody2D>().velocity.y);
            }
            //负的力
            float force= (Mathf.Pow(exitSpeed, 2) - Mathf.Pow(speed, 2)) / 2.0f;
            if (force >maxForce)
            {
                areaEffector.forceMagnitude = force;
            }
            else
            {
                areaEffector.forceMagnitude = maxForce;
            }

        }
    }
    public void Sleep()
    {
        GetComponent<AreaEffector2D>().enabled = false;
    }

    public void WakeUp()
    {
        GetComponent<AreaEffector2D>().enabled = true;
    }
}
