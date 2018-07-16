/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-7-16
  *Description: 风力漩涡-加速区域-主角进入失重状态
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindPositive : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.gameObject.tag == "Player")
        {
            //主角进入失重状态
            _col.gameObject.GetComponent<PlayerAction>().ChangeStatus(PlayerStatus.Player_Weightlessness);
        }
    }
}
