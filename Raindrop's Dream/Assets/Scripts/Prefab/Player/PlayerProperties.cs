using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties: MonoBehaviour{
    
    public int hpMaxValue;//最大血量
    public int hpCurrentValue;//当前血量

    public float moveSpeed;//移动速度
    public float maxJumpSpeed;//跳跃最小，最大速度
    public float maxJumpTime = 1f;//最长跳跃时间
    public float shootSpan;//射击间隔
    public bool canDoubleJump = false;//是否解锁二段跳

    public int itemsMaxAmount = 5;
    public Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();//道具列表

}