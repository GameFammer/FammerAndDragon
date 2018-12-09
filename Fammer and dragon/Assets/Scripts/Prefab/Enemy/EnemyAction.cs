using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction : MonoBehaviour, IBeHitMessage
{
    protected EnemyProperties properties;
    
    // Use this for initialization
    void Start()
    {
        properties = gameObject.GetComponent<EnemyProperties>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void BeHit(int _damage, HitEffect _effect)
    {
        //挨打效果
        _effect.Show(this.gameObject);
        properties.hp -= _damage;
        if (properties.hp <= 0)
        {
            //死亡效果
            properties.dieAction.Die();
            //hp空延迟销毁
            Destroy(this.gameObject, properties.destroyDelay);
        }
    }
}
