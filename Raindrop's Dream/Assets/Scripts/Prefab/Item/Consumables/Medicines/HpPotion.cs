using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPotion : Consumable {

    private int recoveryValue = 10;

    public void Use()
    {
        int currentValue = this.owner.GetComponent<PlayerProperties>().hpCurrentValue;
        int maxValue = this.owner.GetComponent<PlayerProperties>().hpMaxValue;

        this.owner.GetComponent<PlayerProperties>().hpCurrentValue =
            currentValue + recoveryValue > maxValue ? maxValue : currentValue + recoveryValue;

        Destroy(this.gameObject, 1);
    }
}