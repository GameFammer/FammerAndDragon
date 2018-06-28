using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour {
    [Header("默认水平方向")]
    public bool isHorizontal = true;//是否水平移动
    [Header("默认循环移动")]
    public bool isLoop = true;//是否循环移动
    public float speed;//移动速度
    public float maxDistance;//最大移动距离

    private Rigidbody2D rb2d;
	// Use this for initialization
	void Start () {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
