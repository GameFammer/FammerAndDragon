/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-4-16
  *Description:behit interface.
  *Changes:
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IBeHitMessage :IEventSystemHandler {
    void BeHit(int _damage,HitEffect _effect);
}
