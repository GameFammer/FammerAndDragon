using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAction : EnemyAction
{
    protected PatrolProperties properties;

    void Start()
    {
        properties = GetComponent<PatrolProperties>();

        properties.spriteRenderer = GetComponent<SpriteRenderer>();
        properties.animator = GetComponent<Animator>();
        properties.rb2d = GetComponent<Rigidbody2D>();


        properties.contactFilter.useTriggers = false;
        properties.contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        properties.contactFilter.useLayerMask = true;

        properties.initialPosition = GetComponent<Transform>().position;
        properties.player = GameObject.Find("Player");
    }

    protected void FixedUpdate()
    {
        Wander();
    }

    protected void Wander()
    {

    }

    protected void CheckPlayer()
    {

    }

    protected void CheckPosition()
    {

    }

    protected void Attack()
    {

    }
}