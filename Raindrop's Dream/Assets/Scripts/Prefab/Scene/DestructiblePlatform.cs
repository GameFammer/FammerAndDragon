/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-6-30
  *Description: 可破坏物体，需要Collider 2D、销毁动画
**********************************************************************************/
using UnityEngine;

public class DestructiblePlatform : MonoBehaviour, IBeHitMessage
{
    [Header("最大生命值")]
    public int maxHp;
    [Header("生命值一半一下对应Sprite")]
    public Sprite helfHpSprite;
    [Header("破坏销毁延迟")]
    public float destroyDelay;

    private int halfHp;
    private int hp;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    // Use this for initialization
    void Start()
    {
        hp = maxHp;
        halfHp = maxHp / 2;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
    }
    public void BeHit(int _damage, HitEffect _effect)
    {
        _effect.Show();
        hp -= _damage;
        //半血一下更换贴图
        if (hp <= halfHp)
        {
            if (spriteRenderer.sprite != helfHpSprite)
            {
                spriteRenderer.sprite = helfHpSprite;
            }          
        }
        if(hp<=0)
        {
            animator.SetBool("Broken", true);
            Destroy(gameObject, destroyDelay);
        }
    }
}
