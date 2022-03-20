using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : IState
{
    private FSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;

    public Death(FSM fsm)
    {
        this.manager = fsm;
        this.parameter = fsm.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("Death");
        parameter.trueDeath = true;
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime > .95f)
        { 
            
        }
    }

    public void OnExit()
    {
        
    }


}
