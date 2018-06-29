using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePlatform : MonoBehaviour, IBeHitMessage
{
    [Header("最大生命值")]
    public int maxHp;
    [Header("生命值一半一下对应Sprite")]
    public Sprite helfHpSprite;

    private int halfHp;
    private int hp;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    // Use this for initialization
    void Start()
    {
        hp = maxHp;
        halfHp = maxHp / 2;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void BeHit(int _damage, HitEffect _effect)
    {
        _effect.Show();
        hp -= _damage;
        //半血一下更换贴图
        if (hp <= halfHp)
        {
            if (spriteRenderer.sprite != helfHpSprite)
            {
                spriteRenderer.sprite = helfHpSprite;
            }          
        }
        if(hp<=0)
        {
            animator.SetBool("Broken", true);
        }
    }
}
