/********************************************************************************* 
 *Author:AICHEN
 *Version:1.0
 *Date:  2018-4-16
 *Description: Enemy
 *Changes:
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,IBeHitMessage {

    public float hp;//生命值
    public float destroyDelay;//尸体销毁延迟
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void BeHit(float _damage, HitEffect _type)
    {   
        //挨打效果
        BeHitEffect(_damage,_type);
        if (hp<=0)
        {
            //死亡效果
            DeathEffect();
            //hp空延迟销毁
            Destroy(this.gameObject,destroyDelay);
        }
    }
    //被攻击效果
    protected virtual void BeHitEffect(float _damege,HitEffect _type)
    {
        //默认效果
        BeHitEffect_Default(_damege);
    }
    //默认被攻击效果
    protected virtual void BeHitEffect_Default(float _damege)
    {
        //挨打动画

        //减少生命值
        hp -= _damege;
    }
    protected virtual void DeathEffect()
    {
        //死亡动画

    }
}
