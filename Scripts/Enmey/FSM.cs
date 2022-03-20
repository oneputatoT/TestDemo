using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

//用枚举来存状态的键
public enum StateType
{
    Idle, Walk, Chase, React, Attack, Hit, Death, AttackTwo
}

//敌人属性
[Serializable]
public class Parameter
{
    public int health;
    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;
    public float secondAttackTime;
    [Header("巡逻点")]
    public Transform[] patrolPoints;
    [Header("追击范围")]
    public Transform[] chasePoint;
    public LayerMask targetLayer;   //通过图层获取玩家
    public Transform target;      //获取玩家位置信息为后序判断
    public Transform attackPoint;
    public float attackArea;      //攻击范围
    public Animator animator;
    public bool getHit;
    public bool haveSecondAttack;
    public Vector2 backDirection;     //攻击后退方向 
    public Transform backPoint;
    public bool trueDeath;
    public Rigidbody2D rb;
    public Animator HitEffect;
}


public class FSM : MonoBehaviour
{
    private IState currentState;
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
    public Parameter parameter;
    public AnimationClip[] allAnimationName;

    //注册状态
    private void Start()
    {
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Walk, new PatrolState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.Attack, new AttackState(this));
        states.Add(StateType.Hit, new HitState(this));
        states.Add(StateType.Death, new Death(this));
        states.Add(StateType.AttackTwo, new AttackTwo(this));
        parameter.animator = transform.GetComponent<Animator>();
        parameter.HitEffect = transform.GetChild(2).GetComponent<Animator>();
        parameter.rb = GetComponent<Rigidbody2D>();
        allAnimationName = parameter.animator.runtimeAnimatorController.animationClips;
        TransitionState(StateType.Idle);
        
    }

    private void Update()
    {
        currentState.OnUpdate();
    }

    public void TransitionState(StateType type)
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter();     
    }

    public void FlipTo(Transform ptrolPoint)
    {
        if (ptrolPoint != null)
        {
            if (transform.position.x > ptrolPoint.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (transform.position.x < ptrolPoint.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            parameter.target = collision.transform;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            parameter.target = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(parameter.attackPoint.position, parameter.attackArea);
    }

    public void GetHit(Vector2 dir)
    {
        if (parameter.health > 0)
        {
            transform.localScale = new Vector3(dir.x,1, 1);
            parameter.backDirection = -dir;
            parameter.getHit = true;
        }
    }

    
}
