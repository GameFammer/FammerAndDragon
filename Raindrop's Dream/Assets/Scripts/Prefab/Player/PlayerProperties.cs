using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RDUI;
public class PlayerProperties: MonoBehaviour{
    [SerializeField]
    private int hpLimitValue;//血量上限增加最大值
    public int hpMaxValue;//最大血量
    public int hpCurrentValue;//当前血量
    public float moveBasicSpeed = 2;//基础移动速度
    public float moveSpeed = 2;//移动速度
    public float maxJumpSpeed;//跳跃最小，最大速度
    public float maxJumpTime = 1f;//最长跳跃时间
    public float shootSpan;//射击间隔
    public bool canDoubleJump = false;//是否解锁二段跳

    //道具列表
    public int itemsMaxAmount = 5;
    public Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();

    //装备列表 { "装备位置" ，"装备名"}
    public Dictionary<string, string> equipments = new Dictionary<string, string>();
}