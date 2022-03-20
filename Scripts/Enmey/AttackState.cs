using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AttackState : IState
{
    private FSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;
    

    public AttackState(FSM fsm)
    {
        this.manager = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Attackone");
        for (int i = 0; i < manager.allAnimationName.Length; i++)
        {
            if (string.Equals(manager.allAnimationName[i].name, "Attacktwo"))
            {
                parameter.haveSecondAttack = true;
            }
        }
        //if (parameter.haveSecondAttack)
        //{
        //    Sequence s = DOTween.Sequence();
        //    s.AppendInterval(parameter.secondAttackTime);
        //    s.AppendCallback(() => parameter.animator.Play("Attacktwo"));
        //    s.AppendInterval(0.05f).OnComplete(() => parameter.attackOver = !parameter.attackOver).OnComplete(()=> TrueDeath()); 
        //}

    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.getHit)
        {
            manager.TransitionState(StateType.Hit);
        }
        if (parameter.haveSecondAttack && parameter.target != null&&info.normalizedTime>.95f)
        {
            manager.FlipTo(parameter.target);
            manager.TransitionState(StateType.AttackTwo);
        }
        else if (parameter.target == null&& info.normalizedTime > .95f)
        {
            manager.TransitionState(StateType.Chase);
        }

    }

    public void OnExit()
    {
        
    }

    //void TrueDeath()
    //{
    //    if (parameter.trueDeath)
    //        manager.TransitionState(StateType.Death);
    //    manager.TransitionState(StateType.Chase);
    //}

}
