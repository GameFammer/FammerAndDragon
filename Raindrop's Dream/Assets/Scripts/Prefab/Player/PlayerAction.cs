/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-4-25
  *Description: palyer move,jump,shoot
**********************************************************************************/


using System.Collections;
using System;
using UnityEngine;
using RDUI;
using Cinemachine;
using System.Reflection;

public class PlayerAction : MonoBehaviour, IBeHitMessage, ILevelEditor
{
    private PlayerProperties properties;

    public GameObject checkIsGround;//用于判断是否在地面上
    public GameObject checkIsHitHead;//用于判断是否撞头
    public GameObject leftShootPosition;//左边子弹初始位置
    public GameObject rightShootPosition;//右边子弹初始位置

    private Rigidbody2D rb2d;//刚体
    private Animator animator;//动画控制器
    private SpriteRenderer renderer;//渲染器
    private Transform cameraTarget;//相机跟随物

    public Vector2 viewUpCameraPosition;//向上移动摄像机后位置
    public Vector2 viewDownCameraPosition;//向下移动摄像机后位置

    private float headDistance = 0.2f;//小于0.2即可认为在撞头了
    private float groundDistance = 0.2f;//小于0.2即可认为在地面上
    private float moveHorizontal;//水平移动

    private bool isOnGround = true;//判断是否位于地面  
    private bool isHitHead = false;//判断是否撞头
    private bool isDoubleJump = false;//判断是否二段跳
    private bool isDamaged = false;//判断是否处于受伤状态


    private float nextShootTime = 0;//下次射击时间


    void Start()
    {
        //获取组件
        rb2d = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        renderer = this.GetComponent<SpriteRenderer>();
        properties = this.gameObject.GetComponent<PlayerProperties>();//玩家属性
        cameraTarget = this.transform.Find("CameraTarget");

        rb2d.gravityScale = properties.gravityScale;

        EditorProperty[] p = GetProperties();
        EditorProperty a;
        a.name = "HpCurrentValue";
        a.value = 1;
        SetProperty(a);
        Debug.Log(properties.HpCurrentValue);
    }

    void FixedUpdate()
    {
        ApplyStatus();
        //获取水平输入
        moveHorizontal = Input.GetAxis("Horizontal");
        //判断是否位于地面/撞头
        isOnGround = Physics2D.Raycast(checkIsGround.transform.position, Vector2.down, groundDistance).collider == null ? false : true;
        isHitHead = Physics2D.Raycast(checkIsHitHead.transform.position, Vector2.up, headDistance).collider == null ? false : true;
        Move(moveHorizontal);
        Jump();
        Shoot();
        MoveView();
        //test
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (properties.HpCurrentValue > 0)
                properties.HpCurrentValue -= 1;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (properties.HpCurrentValue < properties.HpMaxValue)
                properties.HpCurrentValue += 1;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (properties.HpMaxValue < PlayerProperties.HP_MaxLimit_Value)
                properties.HpMaxValue += 1;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (properties.HpMaxValue > 0)
                properties.HpMaxValue -= 1;
        }
    }
    //应用状态效果
    void ApplyStatus()
    {
        switch(properties.status)
        {
            case PlayerStatus.Player_Normal:rb2d.gravityScale = properties.gravityScale; break;
            case PlayerStatus.Player_Weightlessness:rb2d.gravityScale = 0; break;
            default:break;
        }
    }
    //移动
    void Move(float _h)
    {
        if(properties.status<= PlayerStatus.Player_CanntMove)
        {
            if (_h != 0)
            {
                rb2d.velocity = new Vector2(_h * properties.moveSpeed, rb2d.velocity.y);
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
        if(properties.status==PlayerStatus.Player_Weightlessness)
        {
            if (_h != 0)
            {
               
            }
        }
    }
    //跳跃,受伤状态不能跳跃
    void Jump()
    {
        if (!isDamaged && !isHitHead)
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
                    rb2d.velocity = new Vector2(rb2d.velocity.x, properties.maxJumpSpeed);
                    //开启协程控制速度
                    StartCoroutine("JumpUpdate");
                }
            }
            else
            {
                //二段跳
                if (properties.canDoubleJump)
                {
                    if (Input.GetButtonDown("Jump") && !isDoubleJump)
                    {
                        //播放二段跳跃动画（暂时没有）
                        rb2d.velocity = new Vector2(rb2d.velocity.x, properties.maxJumpSpeed);
                        //开启协程控制速度
                        isDoubleJump = true;
                        StartCoroutine("JumpUpdate");
                    }
                }
            }
        }
    }
    //射击
    void Shoot()
    {

        if (Input.GetButton("Fire1") && Time.time > nextShootTime)
        {
            try
            {
                string bulletName = properties.equipments["Weapon"];
                //从子弹池中获取子弹
                GameObject bullet = BulletsPool.instance.GetBulletByName(bulletName);
                if (bullet != null)
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
                    nextShootTime = Time.time + properties.shootSpan;
                }
            }
            finally { }
        }
    }
    //移动视野
    void MoveView()
    {
        float vertical=Input.GetAxis("Vertical");
        if (vertical > 0)
        {
            cameraTarget.localPosition = viewUpCameraPosition;
        }
        else if (vertical < 0)
        {
            cameraTarget.localPosition = viewDownCameraPosition;
        }
        else
        {
            cameraTarget.localPosition = Vector3.zero;
        }
    }
    //根据按键时间控制跳跃速度（高度）
    IEnumerator JumpUpdate()
    {
        float timer = 0f,jumpSpeed = 0f;
        while (Input.GetButton("Jump") && timer < properties.maxJumpTime)
        {
            //撞头、受伤时停止跳跃
            if (isHitHead || isDamaged)
            {
                break;
            }
            jumpSpeed = Mathf.Lerp(properties.maxJumpSpeed, 0f, timer / properties.maxJumpTime);
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
    }
    //改变状态
    public void ChangeStatus(PlayerStatus _status)
    {
        properties.status = _status;
    }
    //被攻击
    public void BeHit(int _damage, HitEffect _effect)
    {
        if (properties.HpCurrentValue > 0 && !isDamaged)
        {
            _effect.Show(this.gameObject);
            properties.HpCurrentValue -= _damage;
            StartCoroutine("Damaged");
            
        }
    }
    //受伤
    IEnumerator Damaged()
    {
        isDamaged = true;
        //更换贴图
        renderer.color = Color.red;
        yield return new WaitForSeconds(properties.invincibleTime);
        //恢复贴图
        isDamaged = false;
        renderer.color = Color.white;
    }
    //进入下一关
    public void EnterNextLevel()
    {
        GameObject entrannce=GameObject.FindGameObjectWithTag("LevelEntrance");
        if(entrannce)
        {
            transform.position = entrannce.transform.position;
        }
    }
    public EditorProperty[] GetEditorProperties()
    {
        return properties.GetEditorProperties();
    }
    public void SetEditorProperty(EditorProperty _property)
    {

    }
    public void Sleep()
    {

        GetComponent<Rigidbody2D>().simulated = false;
    }

    public void WakeUp()
    {
        GetComponent<Rigidbody2D>().simulated = true;
    }
}

