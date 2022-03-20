using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

//��ö������״̬�ļ�
public enum StateType
{
    Idle, Walk, Chase, React, Attack, Hit, Death, AttackTwo
}

//��������
[Serializable]
public class Parameter
{
    public int health;
    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;
    public float secondAttackTime;
    [Header("Ѳ�ߵ�")]
    public Transform[] patrolPoints;
    [Header("׷����Χ")]
    public Transform[] chasePoint;
    public LayerMask targetLayer;   //ͨ��ͼ���ȡ���
    public Transform target;      //��ȡ���λ����ϢΪ�����ж�
    public Transform attackPoint;
    public float attackArea;      //������Χ
    public Animator animator;
    public bool getHit;
    public bool haveSecondAttack;
    public Vector2 backDirection;     //�������˷��� 
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

    //ע��״̬
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
