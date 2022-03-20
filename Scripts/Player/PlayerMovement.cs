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
    [Header("������ֵ")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Space]
    [Header("״̬")]
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
    private string attackType;      //��������
    public int comboStep;
    [Header("�����")]
    public float interval = 2f;    //���ʱ��
    public float lightSpeed;
    
    public int attckPause;
    public float shakeTime;
    public float shakeStrength;
    private float time;

    public int side = 1;

    [Header("����Ƿ�Ҫ�����ٶ�")]
    public GameObject playerDamage;

    [Space]
    [Header("������Ч")]
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

        Walk(dir);     //�ж�ˮƽ����״̬
        anim.SetHorizontalMovement(x, y, rb.velocity.y);
        Attack();

        //��������
        //��ǽ��ʱ���Ҳ����ڵ�ǽ��������סC����������
        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if (side != coll.wallSide)
                anim.Flip(side * 1);
            wallGrab = true;       //��������
            wallSlide = false;      //�����»�
        }


        //��C���ɿ��������»�,���߲���ǽ���ˣ������ڵ�ǽ��
        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        //�ڵ�������ûʹ�ó�̣���ʱ������Ծ�ָ�
        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BestJump>().enabled = true;
        }

        //��ǽ��ץȡʱ�����޳�̣�������,��ʱ������ǰ�а���λ�������������˶�
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


        //��Ծ����
        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");
            if (coll.onGround)                  //����ڵ��棬��ִ����
                Jump(Vector2.up,false);               //ƽ������ʹ��ƽ�������ӣ��˴�������������Ч��������Ծ��
            if (coll.onWall && !coll.onGround)     //���ڵ��棬����ǽ��ʱ��ִ�е�ǽ��
                WallJump();
        }


        //��̹���
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

        WallParticle(y);//����

        if (wallGrab || wallSlide || !canMove)
            return;

        //���ﷴת
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




    //�ж�ˮƽ����״̬������״̬ģ���ָ�
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

    //������������
    private void Jump(Vector2 dir,bool wall)
    {
        //��������
        slideParticle.transform.parent.localScale = new Vector3(ParicleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;


        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        particle.Play();
    }


    private void WallJump()
    {
        //������ת
        if ((side == 1 && coll.onRightWall) || (side == -1 && !coll.onRightWall))
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(0.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f),true);    //��ǽ�������õ�ǽ������

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

        //�ж���ǽ����ʱ��ˮƽ�����Ƿ�����Ч��
        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    //���
    private void Dash(float x, float y)
    {
        //�������Ч
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
        //��Ӱ��Ч
        FindObjectOfType<PlayerGhost>().ShowGhost();
        StartCoroutine(GroundDash());

        DOVirtual.Float(14, 0, 0.8f, RigidbodyDrag);  //��������

        //��ס���
        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BestJump>().enabled = false;
        wallJumped = true;
        isDashing = true;        //���ڳ��

        yield return new WaitForSeconds(0.3f);

        //�������
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

        jumpParticle.Play();//��������Ч��
    }

    int ParicleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        //�»�ʱ��������ЧΪ��ɫ
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
