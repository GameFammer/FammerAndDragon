/********************************************************************************* 
  *Author:AICHEN
  *Version:1.0
  *Date:  2018-4-16
  *Description:bullet move and hit.
  *Changes:
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour {
    public HitEffect type;//子弹攻击类型
    public float damage;//伤害
    public float speed;//速度
    public float activeTime;//存在时间

    private Rigidbody2D rb2d;//刚体
    private SpriteRenderer spriteRenderer;//渲染器

    private Vector2 prePos;//上一次FixedUpdate位置
    private float rayDistance;//射线判定距离
    private RaycastHit2D hitInfo2d;//射线碰撞检测信息
    // Use this for initialization
    void Start ()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //开启定时“销毁”协程
        StartCoroutine(SetNotActive(activeTime));        
	}
    void FixedUpdate()
    {
        //记录位置
         prePos = (Vector2)this.transform.position;
        //子弹移动
        BulletMove();
        //射线检测碰撞,仅在第8层（Enemy）9层（BreakEnable）
        Hit();
    }
    //定时“销毁”子弹
    IEnumerator SetNotActive(float _activeTime)
    {
        yield return new WaitForSeconds(_activeTime);
        this.gameObject.SetActive(false);
    }
    //子弹移动，子类可重写
    protected virtual void BulletMove()
    {
        rb2d.velocity = new Vector2(spriteRenderer.flipX ? (-1) * speed : speed, 0);
    }
    //子弹碰撞，子类可重写
    protected virtual void Hit()
    {
        //计算方向
        Vector2 dir = (prePos + rb2d.velocity*Time.deltaTime) - prePos;
        //计算射线距离
        rayDistance = (rb2d.velocity * Time.deltaTime).magnitude;
        //获取第一次碰撞
        hitInfo2d = Physics2D.Raycast(prePos, dir, rayDistance, 1 << 8 | 1 << 9);
        if (hitInfo2d.collider != null)
        {
            //事件
            ExecuteEvents.Execute<IBeHitMessage>(hitInfo2d.transform.gameObject, null, (x, y) => x.BeHit(damage, type));
            //“销毁”
            this.gameObject.SetActive(false);
        }
    }
}
