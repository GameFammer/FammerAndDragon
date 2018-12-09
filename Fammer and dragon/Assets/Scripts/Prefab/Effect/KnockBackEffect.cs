/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-7-13
  *Description: 击退、弹开效果
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackEffect : HitEffect
{
    public Vector2 knockVelocity;
    // Use this for initialization
    public override void Show(GameObject _victim)
    {
        _victim.GetComponent<Rigidbody2D>().velocity = knockVelocity;
    }
}
