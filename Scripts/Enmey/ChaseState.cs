using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private FSM manager;
    private Parameter parameter;

    private int patrolPoint;

    public ChaseState(FSM fsm)
    {
        this.manager = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Roll");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.target);


        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
        }

        if (parameter.target)
        {
            manager.transform.position = Vector2.MoveTowards(manager.transform.position, parameter.target.position,
                parameter.chaseSpeed * Time.deltaTime);
        }
        if (parameter.target == null || parameter.target.position.x < parameter.chasePoint[0].position.x
            || parameter.target.position.x > parameter.chasePoint[1].position.x)
        {
            manager.TransitionState(StateType.Idle);
        }
        //¹¥»÷ÅÐ¶Ï
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea,parameter.targetLayer))
        {
            manager.TransitionState(StateType.Attack);
        }
    }

    public void OnExit()
    {
        
    }


}
