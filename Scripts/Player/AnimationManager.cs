using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationManager : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement move;
    private CollisionManager coll;

    [HideInInspector]
    public SpriteRenderer sr;

    private void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<CollisionManager>();
        move = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        anim.SetBool("onGround", coll.onGround);
        anim.SetBool("onWall", coll.onWall);
        anim.SetBool("onRightWall", coll.onRightWall);
        anim.SetBool("wallGrab", move.wallGrab);
        anim.SetBool("wallSlide", move.wallSlide);
        anim.SetBool("canMove", move.canMove);
        anim.SetBool("isDashing", move.isDashing);
    }

    public void SetHorizontalMovement(float x, float y, float yVel)
    {
        anim.SetFloat("HorizontalAxis", x);
        anim.SetFloat("VerticalAxis", y);
        anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void SetComboInt(int comboStep)
    {
        anim.SetInteger("ComboStep", comboStep);
    }

    public void Flip(int side)
    {
        //�����ķ�ת
        if (move.wallGrab || move.wallSlide)
        {
            if (side == -1 && !sr.flipX)
                return;
            if (side == 1 && sr.flipX)
                return;   
        }

        //���������ұ�ʱ��flipXΪfalse
        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }

    public void AttackOver()
    {
        move.isAttack = false;
    }
}
