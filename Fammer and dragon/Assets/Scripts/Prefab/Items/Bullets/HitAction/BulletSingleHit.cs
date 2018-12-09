using UnityEngine;
using UnityEngine.EventSystems;

public class BulletSingleHit : HitAction
{

    private Vector2 prePos;//上一次FixedUpdate位置
    private Rigidbody2D rb2d;//刚体
    // Use this for initialization
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //记录位置
        prePos = (Vector2)this.transform.position;
    }
    public override void Hit(int _damage, HitEffect _hitEffect)
    {
        //计算方向
        Vector2 dir = (prePos + rb2d.velocity * Time.deltaTime) - prePos;
        //计算射线距离
        float rayDistance = (rb2d.velocity * Time.deltaTime).magnitude;
        //获取第一次碰撞
        RaycastHit2D hitInfo2d = Physics2D.Raycast(prePos, dir, rayDistance);
        if (hitInfo2d.collider != null)
        {
            if(hitInfo2d.collider.gameObject.GetComponent<IBeHitMessage>()!=null)
            {
                //事件
                ExecuteEvents.Execute<IBeHitMessage>(hitInfo2d.collider.gameObject, null, (x, y) => x.BeHit(_damage, _hitEffect));
                //“销毁”
                this.gameObject.SetActive(false);
            }
        }
    }
}
