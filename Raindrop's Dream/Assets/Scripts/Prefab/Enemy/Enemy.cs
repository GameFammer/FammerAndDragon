/********************************************************************************* 
 *Author:AICHEN
 *Version:1.0
 *Date:  2018-4-16
 *Description: Enemy
 *Changes:
**********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IBeHitMessage
{

    public float hp;//生命值
    public float moveSpeed;//移动速度
    public float destroyDelay;//尸体销毁延迟

    public MoveAction moveAction;
    public HitAction hitAciton;
    public DieAction dieAction;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void BeHit(int _damage, HitEffect _effect)
    {
        //挨打效果
        _effect.Show(this.gameObject);
        hp -= _damage;
        if (hp <= 0)
        {
            //死亡效果
            dieAction.Die();
            //hp空延迟销毁
            Destroy(this.gameObject, destroyDelay);
        }
    }
}
