using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitAction : MonoBehaviour
{
    public abstract void Hit(float _damage, HitEffect _hitEffect);
}
