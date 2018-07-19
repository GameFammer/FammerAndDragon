/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-6-30
  *Description: 可移动平台，需要RigidBoday 2D
**********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour, ISleepWakeUp
{
    [Header("默认循环移动")]
    public bool isLoop = true;//是否循环移动
    [Header("移动速度")]
    public Vector2 speed;//移动速度
    [Header("最大移动距离")]
    public float maxDistance;//最大移动距离
    [Header("停留时间")]
    public float anchorTime;//停留时间
    private Rigidbody2D rb2d;
    private Vector2 startPosition;//起始位置
    private float stopTime;//刚停止移动的时间
    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            ChangeSpeed();
        }

        if (Time.time - stopTime >= anchorTime)
        {
            rb2d.velocity = speed;
        }
    }
    void OnCollisionEnter2D(Collision2D collider)
    {

        if (Time.time - stopTime >= anchorTime)
        {
            if (collider.gameObject.tag != "Player"
            && collider.gameObject.tag != "Enemy"
            && collider.gameObject.tag != "Item")
            {
                ChangeSpeed();
            }
        }

    }
    void ChangeSpeed()
    {
        rb2d.velocity = Vector2.zero;
        if (isLoop)
        {
            //反向移动
            startPosition = transform.position;
            speed = (-1f) * speed;
            stopTime = Time.time;
        }
        else
        {
            speed = Vector2.zero;
        }
    }

    public void Sleep()
    {
        GetComponent<Rigidbody2D>().simulated = false;
    }

    public void WakeUp()
    {
        GetComponent<Rigidbody2D>().simulated = true;
    }
}
