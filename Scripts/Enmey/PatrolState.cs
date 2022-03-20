using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private FSM manager;
    private Parameter parameter;

    private int patrolPoint;

    public PatrolState(FSM fsm)
    {
        this.manager = fsm;
        this.parameter = fsm.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("Walk");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.patrolPoints[patrolPoint]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position, parameter.patrolPoints[patrolPoint].position, parameter.moveSpeed * Time.deltaTime);


        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
        }
        if (parameter.target != null
            && parameter.target.position.x >= parameter.chasePoint[0].position.x
            && parameter.target.position.x <= parameter.chasePoint[1].position.x)
        {
            manager.TransitionState(StateType.React);//½øÈë¾¯Ìè×´Ì¬
        }
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPoint].position) < .1f)
        {
            manager.TransitionState(StateType.Idle);
        }
    }
    public void OnExit()
    {
        patrolPoint++;
        if (patrolPoint >= parameter.patrolPoints.Length)
        {
            patrolPoint = 0;
        }
    }


}
