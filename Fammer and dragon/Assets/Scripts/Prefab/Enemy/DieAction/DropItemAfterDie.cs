using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemAfteFDie : DieAction
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void Die()
    {
        Destroy(gameObject);
    }
}
