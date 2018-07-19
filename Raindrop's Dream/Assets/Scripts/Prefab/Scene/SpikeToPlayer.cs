/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-7-13
  *Description: 尖刺陷阱
**********************************************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpikeToPlayer : MonoBehaviour, ISleepWakeUp
{
    public int damage;
    public HitEffect hitEffect;

    public void WakeUp()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    public void Sleep()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D _collider)
    {
        if (_collider.gameObject.tag == "Player")
        {
            ExecuteEvents.Execute<IBeHitMessage>(_collider.gameObject, null, (x, y) => x.BeHit(damage, hitEffect));
        }
    }
}
