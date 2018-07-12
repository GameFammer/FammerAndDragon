using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpikeToPlayer : MonoBehaviour
{
    public int damage;
    public HitEffect hitEffect;
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            ExecuteEvents.Execute<IBeHitMessage>(collider.gameObject, null, (x, y) => x.BeHit(damage, hitEffect));
        }
    }
}
