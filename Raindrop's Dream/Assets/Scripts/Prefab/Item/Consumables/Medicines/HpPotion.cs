using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPotion : Consumable {
    
    private int recoveryValue;

    void Start()
    {
        recoveryValue = PublicDataManager.instance.GetPotionRecoveryValue(gameObject.name);
    }

    public void Use()
    {
        GameObject player = GameObject.Find("player");
        int currentValue = player.GetComponent<PlayerProperties>().hpCurrentValue;
        int maxValue = player.GetComponent<PlayerProperties>().hpMaxValue;

        player.GetComponent<PlayerProperties>().hpCurrentValue =
            currentValue + recoveryValue > maxValue ? maxValue : currentValue + recoveryValue;

        Destroy(this.gameObject, 1);
    }
}