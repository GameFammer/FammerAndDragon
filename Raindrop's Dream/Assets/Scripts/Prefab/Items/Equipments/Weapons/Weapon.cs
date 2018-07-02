using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equipment
{

    public void SetOwner(Collider2D _collision)
    {
        if (_collision.gameObject.name != "player")
        {
            return;
        }

        var playerProperties = _collision.gameObject.GetComponent<PlayerProperties>();

        if (playerProperties.equipments.ContainsKey("Weapon"))
        {
            playerProperties.equipments["Weapon"] = this.gameObject;
        }
        else
        {
            playerProperties.equipments.Add("Weapon", this.gameObject);
        }
    }
}