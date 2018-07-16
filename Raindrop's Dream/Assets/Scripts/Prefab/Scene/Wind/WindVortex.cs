/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-7-16
  *Description: 风力漩涡
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVortex : MonoBehaviour
{
    public Transform enemyTransform;
    public GameObject wallDown;
    public Transform[] windTransform;
    private float animationTime;
    private GameObject player;
    void Start()
    {
        player = GameObject.Find("player");
    }
    void Update()
    {
        if (enemyTransform.childCount == 0)
        {
            //消失动画
            StartCoroutine(StopWind());
        }
    }
    IEnumerator StopWind()
    {
        yield return new WaitForSeconds(animationTime);
        foreach (Transform wind in windTransform)
        {
            wind.gameObject.SetActive(false);
        }
        //停止,消除下面遮挡
        wallDown.SetActive(false);
        //改变Player状态
        player.GetComponent<PlayerAction>().ChangeStatus(PlayerStatus.Player_Normal);
    }
}
