﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    [Header("默认循环移动")]
    public bool isLoop = true;//是否循环移动
    [Header("移动速度")]
    public Vector2 speed;//移动速度
    [Header("最大移动距离")]
    public float maxDistance;//最大移动距离

    private Rigidbody2D rb2d;
    private Vector2 startPosition;//起始位置
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
        rb2d.velocity = speed;
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            if (isLoop)
            {
                //反向移动
                startPosition = transform.position;
                speed = (-1f) * speed;
            }
            else
            {
                speed =Vector2.zero;
            }
        }
    }
}
