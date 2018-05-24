/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  //完成日期 
  *Description:  //用于主要说明此程序文件完成的主要功能 
                //与其他模块或函数的接口、输出值、取值范围、 
                //含义及参数间的控制、顺序、独立及依赖关系 
  *Changes://修改记录
**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    private bool isCreatCorridor = false;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
    
    }

  
}
