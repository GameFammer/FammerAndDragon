/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-7-16
  *Description: 风力漩涡
**********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVortex : MonoBehaviour, ISleepWakeUp
{
    public Transform enemyTransform;
    public GameObject wallDown;
    public Transform[] windTransform;
    private GameObject player;
   // private Animator animator;

    private float animatorSpeed;
    void Start()
    {
        player = GameObject.Find("player");
       // animator = GetComponent<Animator>();
       // animatorSpeed = animator.speed;
    }
    void Update()
    {
        if (enemyTransform.childCount == 0)
        {
            //消失动画
            //AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
            //if (animatorInfo.normalizedTime >= 1.0f &&  )
                StartCoroutine(StopWind());
        }
    }
    IEnumerator StopWind()
    {
       // Sleep();
        //停止,消除下面遮挡
        wallDown.SetActive(false);
        //改变Player状态
        player.GetComponent<PlayerAction>().ChangeStatus(PlayerStatus.Player_Normal);
        yield return null;
    }

    public void Sleep()
    {
        //animator.speed = 0;
        foreach(Transform t in transform)
        {
            if(t.GetComponent<ISleepWakeUp>()!=null)
            {
                t.GetComponent<ISleepWakeUp>().Sleep();
            }
        }
    }

    public void WakeUp()
    {
       // animator.speed = animatorSpeed;
        foreach (Transform t in transform)
        {
            if (t.GetComponent<ISleepWakeUp>() != null)
            {
                t.GetComponent<ISleepWakeUp>().WakeUp();
            }   
        }
    }
}
