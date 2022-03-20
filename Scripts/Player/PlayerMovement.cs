using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    private CollisionManager coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationManager anim;

    [Space]
    [Header("弹跳数值")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Space]
    [Header("状态")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;
    public bool isAttack;

    [Space]
    [SerializeField]
    private bool groundTouch;
    [SerializeField]
    private bool hasDashed;
    private string attackType;      //攻击类型
    public int comboStep;
    [Header("打击感")]
    public float interval = 2f;    //间隔时间
    public float lightSpeed;
    
    public int attckPause;
    public float shakeTime;
    public float shakeStrength;
    private float time;

    public int side = 1;

    [Header("检测是否要补偿速度")]
    public GameObject playerDamage;

    [Space]
    [Header("粒子特效")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    private void Start()
    {
        coll = GetComponent<CollisionManager>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationManager>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);     //判断水平方向状态
        anim.SetHorizontalMovement(x, y, rb.velocity.y);
        Attack();

        //攀爬功能
        //在墙上时，且不处于登墙跳，当按住C，可以攀爬
        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if (side != coll.wallSide)
                anim.Flip(side * 1);
            wallGrab = true;       //正在攀爬
            wallSlide = false;      //不会下滑
        }


        //当C键松开，马上下滑,或者不在墙上了，或者在登墙跳
        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        //在地面上且没使用冲刺，这时候开启跳跃手感
        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BestJump>().enabled = true;
        }

        //在墙上抓取时，若无冲刺，会下落,当时若下落前有按方位键，将做抛物运动
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);
            float speedModifier = y > 0 ? 0.5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (speedModifier * speed));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
        {
            wallSlide = false;
        }


        //跳跃功能
        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");
            if (coll.onGround)                  //如果在地面，就执行跳
                Jump(Vector2.up,false);               //平地跳，使用平地跳粒子，此处后续加入粒子效果增加跳跃感
            if (coll.onWall && !coll.onGround)     //不在地面，且在墙上时，执行蹬墙跳
                WallJump();
        }


        //冲刺功能
        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);//粒子

        if (wallGrab || wallSlide || !canMove)
            return;

        //人物反转
        if (x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }

    }




    //判断水平方向状态，根据状态模拟手感
    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (playerDamage.activeSelf)
        {
            if(anim.sr.flipX)
            rb.velocity = new Vector2( -1*lightSpeed, rb.velocity.y);
            else
            rb.velocity = new Vector2(1 * lightSpeed, rb.velocity.y);
        }
        else if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }

    }

    //后续加入粒子
    private void Jump(Vector2 dir,bool wall)
    {
        //起跳粒子
        slideParticle.transform.parent.localScale = new Vector3(ParicleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;


        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        particle.Play();
    }


    private void WallJump()
    {
        //攀爬反转
        if ((side == 1 && coll.onRightWall) || (side == -1 && !coll.onRightWall))
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(0.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f),true);    //登墙跳，启用登墙跳粒子

        wallJumped = true;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private void WallSlide()
    {
        if (coll.wallSide != side)
            anim.Flip(side * -1);
        if (!canMove)
            return;

        //判断在墙下落时的水平方向是否有有效力
        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    //冲刺
    private void Dash(float x, float y)
    {
        //摄像机特效
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(0.2f, 0.5f, 14, 90, false, true);
        hasDashed = true;
        anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        //幻影特效
        FindObjectOfType<PlayerGhost>().ShowGhost();
        StartCoroutine(GroundDash());

        DOVirtual.Float(14, 0, 0.8f, RigidbodyDrag);  //增加阻力

        //锁住冲刺
        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BestJump>().enabled = false;
        wallJumped = true;
        isDashing = true;        //正在冲刺

        yield return new WaitForSeconds(0.3f);

        //结束冲刺
        dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BestJump>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(0.15f);
        if (coll.onGround)
            hasDashed = false;     
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    //
    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side=anim.sr.flipX?-1:1;

        jumpParticle.Play();//弹跳粒子效果
    }

    int ParicleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        //下滑时，粒子特效为白色
        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParicleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }


    void Attack()
    {
        if (Input.GetButtonDown("Fire2") && 
            !isAttack&&coll.onGround&&!isDashing && !coll.onWall)
        {
            isAttack = true;
            attackType = "Light";
            comboStep++;
            if (comboStep > 3)
                comboStep = 1;
            time = interval;
            anim.SetTrigger("attack");
            anim.SetComboInt(comboStep);
        }

        if (time != 0)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = 0;
                comboStep = 0;
                isAttack = false;
            }
        }
    }


}
