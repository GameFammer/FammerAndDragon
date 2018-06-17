/********************************************************************************* 
  *Author:AICHEN
  *Version:1.1
  *Date:  2018-4-25
  *Description: palyer move,jump,shoot
  *Changes:4-25增加二段跳
**********************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour {
  
    public GameObject checkIsGround;//用于判断是否在地面上
    public GameObject leftShootPosition;//左边子弹初始位置
    public GameObject rightShootPosition;//右边子弹初始位置

    public float moveSpeed;//移动速度
    public float maxJumpSpeed;//跳跃最小，最大速度
    public float maxJumpTime = 1f;//最长跳跃时间
    public float shootSpan;//射击间隔
    public bool canDoubleJump = false;//是否解锁二段跳

    private Rigidbody2D rb2d;//刚体
    private Animator animator;//动画控制器
    private new SpriteRenderer renderer;//渲染器
    private float groundDistance = 0.2f;//小于0.2即可认为在地面上
    private float moveHorizontal;//水平移动

    private bool isOnGround=true;//判断是否位于地面  
    private bool isDoubleJump=false;//判断是否二段跳
    private float jumpSpeed = 0;

    private float nextShootTime = 0;//下次射击时间


    void Start ()
    {   
        //获取组件
        rb2d = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        renderer = this.GetComponent<SpriteRenderer>();
    }
	
	
	void FixedUpdate ()
    {
        //获取水平输入
        moveHorizontal = Input.GetAxis("Horizontal");
        //判断是否位于地面
        isOnGround= Physics2D.Raycast(checkIsGround.transform.position, Vector2.down, groundDistance).collider==null?false:true;
        Move(moveHorizontal);
        Jump();
        Shoot();
    }
    //移动
    void Move(float _h)
    {
        if (_h != 0)
        {
            rb2d.velocity= new Vector2(_h * moveSpeed, rb2d.velocity.y);
            if (_h > 0)
                renderer.flipX = false;
            if (_h < 0)
                renderer.flipX = true;
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }
    }
    //跳跃
    void Jump()
    {
        //在地面上
        if (isOnGround)
        {    
            //停止播放动画（暂时没有）

            isDoubleJump = false;
            //按下跳跃键
            if (Input.GetButtonDown("Jump"))
            {
                //播放跳跃动画（暂时没有）

                //开启协程控制速度
                StartCoroutine(JumpRoutine());             
            }
        }
        else
        {
            //二段跳
            if(canDoubleJump)
            {
                if (Input.GetButtonDown("Jump") && !isDoubleJump)
                {
                    //播放二段跳跃动画（暂时没有）

                    //开启协程控制速度
                    isDoubleJump = true;
                    StartCoroutine(JumpRoutine());
                }
            }
        }      
    }
    //射击
    void Shoot()
    {
        if(Input.GetButton("Fire1") && Time.time>nextShootTime)
        {
            //从子弹池中获取子弹
            GameObject bullet = BulletsPool.playerBulletsPool.GetBullet();
            if(bullet != null)
            {
                //面向右边
                if (!renderer.flipX)
                {
                    bullet.transform.position = rightShootPosition.transform.position;
                    bullet.GetComponent<SpriteRenderer>().flipX = false;
                }
                //面向左边
                else
                {
                    bullet.transform.position = leftShootPosition.transform.position;
                    //翻转子弹贴图
                    bullet.GetComponent<SpriteRenderer>().flipX = true;
                }

                bullet.SetActive(true);
                nextShootTime= Time.time + shootSpan;
            }
        }
    }

    //根据按键时间控制跳跃速度（高度）
    IEnumerator JumpRoutine()
    {
        float timer = 0f;
        while (Input.GetButton("Jump")&& timer <maxJumpTime)
        {

            jumpSpeed = Mathf.Lerp(maxJumpSpeed, 0f, timer / maxJumpTime);
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
