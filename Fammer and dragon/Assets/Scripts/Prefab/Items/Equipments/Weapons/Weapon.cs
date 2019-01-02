using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equipment
{
    public GameObject bullet;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        SetOwner(collision);
        Destroy();
    }

    public override void SetOwner(Collider2D _collision)
    {
        if (_collision.gameObject.name != "Player")
        {
            return;
        }

        var playerProperties = _collision.gameObject.GetComponent<PlayerProperties>();

        if (playerProperties.equipments.ContainsKey("Weapon"))
        {
            playerProperties.equipments["Weapon"] = bullet.name;
        }
        else
        {
            playerProperties.equipments.Add("Weapon", bullet.name);
        }
    }
}