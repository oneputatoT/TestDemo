using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IdleState : IState
{
    private FSM manager;
    private Parameter parameter;

    private float timer;        //闲置等待时间

    public IdleState(FSM fsm)
    {
        this.manager = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Idle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;



        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
        }
        if (parameter.target != null
            && parameter.target.position.x >= parameter.chasePoint[0].position.x
            && parameter.target.position.x <= parameter.chasePoint[1].position.x)
        {
            manager.TransitionState(StateType.React);//进入警惕状态
        }
        if (timer >= parameter.idleTime)
        {
            manager.TransitionState(StateType.Walk);
        }

    }

    public void OnExit()
    {
        timer = 0;
    }


}
