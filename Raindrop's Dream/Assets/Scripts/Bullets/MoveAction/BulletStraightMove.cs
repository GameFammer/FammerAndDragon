using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStraightMove : MoveAction
{

    private Rigidbody2D rb2d;//刚体
    private SpriteRenderer spriteRenderer;//渲染器
                                          // Use this for initialization
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    public override void Move(float _speed)
    {
        rb2d.velocity = new Vector2(spriteRenderer.flipX ? (-1) * _speed : _speed, 0);
    }
}
