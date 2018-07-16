using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour
{

    public float hp;//生命值
    public float moveSpeed;//移动速度
    public float destroyDelay;//尸体销毁延迟

    public MoveAction moveAction;
    public HitAction hitAciton;
    public DieAction dieAction;

    public float defendRadius;
    public GameObject player;
}
