using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpikeToPlayer : MonoBehaviour
{
    public int damage;
    public Vector2 sprangVelocity;
    public HitEffect hitEffect;
    
    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            ExecuteEvents.Execute<IBeHitMessage>(collider.gameObject, null, (x, y) => x.BeHit(damage, hitEffect));
            collider.gameObject.GetComponent<Rigidbody2D>().velocity = sprangVelocity;
        }
    }
}
