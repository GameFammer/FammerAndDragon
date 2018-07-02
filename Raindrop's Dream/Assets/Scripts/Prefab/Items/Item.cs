using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public bool hasOwner = false;

    public GameObject owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //没有拥有者的道具首先会被捡起，否则调用该道具的使用效果
        if(!hasOwner)
        {
            SetOwner(collision);
        }
        else
        {
            Effect(collision);
        }
    }

    //为道具指定拥有者
    public void SetOwner(Collider2D _collision)
    {
        if (_collision.gameObject.name != "player")
        {
            return;
        }

        var playerProperties = _collision.gameObject.GetComponent<PlayerProperties>();

        if(playerProperties.items.Count < playerProperties.itemsMaxAmount)
        {
            hasOwner = true;
            owner = _collision.gameObject;
            //将道具添加到目标的道具列表里
            owner.GetComponent<PlayerProperties>().items.Add(this.gameObject.name, this.gameObject);
        }
    }

    //子类继承Effect实现道具效果
    public void Effect(Collider2D _collision)
    {
        Destroy(gameObject,2);
    }
}