using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]
    [Header("Collision")]
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset;
    public Vector2 rightOffset;
    public Vector2 leftOffset;

    private AnimationManager anim;
    private PlayerMovement move;
    

    private void Start()
    {
        anim = GetComponentInChildren<AnimationManager>();
        move = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //检测地面
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius,groundLayer);
        //检测是否在墙上
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        //检测右边墙
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius,groundLayer);
        //检测左边墙
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius,groundLayer);

        wallSide = onRightWall ? -1 : 1;    //当碰到右墙时，这个值将与我们反转比较
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //弹反
            if (collision.name == "AntiDamage")
            {
                CameraEffect.Instance.HitPause(10);
                CameraEffect.Instance.CameraShake(0.2f, 0.03f);
                if (anim.sr.flipX)
                    collision.GetComponentInParent<FSM>().GetHit(Vector2.right);
                else if (!anim.sr.flipX)
                    collision.GetComponentInParent<FSM>().GetHit(Vector2.left);
            }
            else 
            {
                CameraEffect.Instance.HitPause(move.attckPause);
                CameraEffect.Instance.CameraShake(move.shakeTime, move.shakeStrength);
                if (anim.sr.flipX)
                    collision.GetComponent<FSM>().GetHit(Vector2.right);
                else if (!anim.sr.flipX)
                    collision.GetComponent<FSM>().GetHit(Vector2.left);
            }
        }
        if (collision.CompareTag("EnemyDamage"))
        { 
        
        }
    }
}
