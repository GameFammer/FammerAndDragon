using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPotion : Consumable {

    private int recoveryValue = 10;

    public void Use()
    {
        int currentValue = this.owner.GetComponent<PlayerProperties>().HpCurrentValue;
        int maxValue = this.owner.GetComponent<PlayerProperties>().HpMaxValue;

        this.owner.GetComponent<PlayerProperties>().HpCurrentValue =
            currentValue + recoveryValue > maxValue ? maxValue : currentValue + recoveryValue;

        Destroy(this.gameObject, 1);
    }
}