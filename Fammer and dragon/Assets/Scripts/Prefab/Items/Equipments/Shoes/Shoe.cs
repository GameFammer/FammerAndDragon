using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoe : Equipment {

    private int quickeningValue = 1;


    public void SetOwner(Collider2D _collision)
    {
        if (_collision.gameObject.name != "player")
        {
            return;
        }

        var playerProperties = _collision.gameObject.GetComponent<PlayerProperties>();

        if (playerProperties.equipments.ContainsKey("Shoe"))
        {
            playerProperties.equipments["Shoe"] = this.gameObject.name;
        }
        else
        {
            playerProperties.equipments.Add("Shoe", this.gameObject.name);
        }

        playerProperties.moveSpeed = playerProperties.moveBasicSpeed + quickeningValue;
    }

}