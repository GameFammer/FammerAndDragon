using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Guid Id = new Guid();
    public string Name;
    public int MaxStackSize;
    public int stackSize;
    public bool hasOwner = false;

    public GameObject owner;

    private BoxCollider2D boxCollider;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    protected void OnMouseOver()
    {
        //悬停
        MouseOver();
        //左键按下
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseDown();
        }
        //右键按下
        else if (Input.GetMouseButtonDown(1))
        {
            RightMouseDown();
        }
        //左键抬起
        else if (Input.GetMouseButtonUp(0))
        {
            LeftMouseUp();
        }
        //右键抬起
        else if (Input.GetMouseButtonUp(1))
        {
            RightMouseUp();
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTrigger");
        //test
        var playerProperties = collision.gameObject.GetComponent<PlayerProperties>();
        playerProperties.backpack.Packing(this);
        if (stackSize <= 0)
        {
            Destroy(this.gameObject);
        }

        ////没有拥有者的道具首先会被捡起，否则调用该道具的使用效果
        //if(!hasOwner)
        //{
        //    SetOwner(collision);
        //}
        //else
        //{
        //    Effect(collision);
        //}

    }

    //为道具指定拥有者
    public virtual void SetOwner(Collider2D _collision)
    {
        if (_collision.gameObject.name != "player")
        {
            return;
        }

        var playerProperties = _collision.gameObject.GetComponent<PlayerProperties>();

        if (playerProperties.items.Count < playerProperties.itemsMaxAmount)
        {
            hasOwner = true;
            owner = _collision.gameObject;
            //将道具添加到目标的道具列表里
            owner.GetComponent<PlayerProperties>().items.Add(this.gameObject.name, this.gameObject);
        }
    }

    //子类继承Effect实现道具效果
    public virtual void Effect(Collider2D _collision)
    {
        Destroy(gameObject, 2);
    }
    public virtual void MouseOver()
    {

    }
    public virtual void LeftMouseDown()
    {
        //test
        boxCollider.isTrigger = true;
    }
    public virtual void RightMouseDown()
    {

    }
    public virtual void LeftMouseUp()
    {

    }
    public virtual void RightMouseUp()
    {

    }
}