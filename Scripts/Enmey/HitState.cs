using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : IState
{
    private FSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;

    public HitState(FSM fsm)
    {
        this.manager = fsm;
        this.parameter = fsm.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("Hit");
        parameter.HitEffect.SetTrigger("hitBlood");
        parameter.health--;
        parameter.rb.velocity = parameter.backDirection * 3.5f;
    }
    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.health <= 0)
        {
            manager.TransitionState(StateType.Death);
        }
        if (info.normalizedTime > .50f)
        {
            manager.TransitionState(StateType.Chase);
        }
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }


}
