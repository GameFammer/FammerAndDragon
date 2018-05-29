/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-5-29
  *Description: 处理反射，主要用于CSV表读取
  *Changes:
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class ReflectUtil {

    //给属性赋值
    public static void PiSetValue<T>(string value, PropertyInfo pi, T obj)
    {
        /* ChangeType无法强制转换可空类型，所以需要这样特殊处理（参考：http://bbs.csdn.net/topics/320213735） */
        if (pi.PropertyType.FullName.IndexOf("Boolean") > 0)
        {
            /* 布尔类型要特殊处理  */
            pi.SetValue(obj, Convert.ChangeType(Convert.ToInt16(value), (Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType)), null);
        }
        else
        {
            pi.SetValue(obj, Convert.ChangeType(value, (Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType)), null);
        }
    }
}
