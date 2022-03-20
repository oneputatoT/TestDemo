using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTwo : IState
{
    private FSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;

    public AttackTwo(FSM fsm)
    {
        this.manager = fsm;
        this.parameter = fsm.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("Attacktwo");
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
        }
        if (info.normalizedTime > .95f)
        {
            manager.TransitionState(StateType.Chase);
        }
    }

    public void OnExit()
    {
        
    }


}
