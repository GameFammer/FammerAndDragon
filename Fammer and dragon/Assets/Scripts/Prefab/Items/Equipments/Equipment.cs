using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item {
    public void Destroy()
    {
        Destroy(gameObject);
    }
}