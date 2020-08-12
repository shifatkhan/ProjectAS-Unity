using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Martial_Attack1State : MeleeAttackState
{
    private AllyMartial ally;

    public Martial_Attack1State(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_MeleeAttackState stateData, AllyMartial ally) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.ally = ally;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();

        ally.CheckIfPlayersTargetDead();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (combo)
            {
                ally.attack2State.SetCombo(Utils.GetRandomBool());
                stateMachine.ChangeState(ally.attack2State);
            }
            else if (!ally.CheckTargetInCloseRangeAction() && ally.GetAttackEnemy())
            {
                ally.moveState.SetFollowTarget(true);
                stateMachine.ChangeState(ally.moveState);
            }
            else if (!ally.GetAttackEnemy())
            {
                if (ally.CheckTargetInRadius())
                {
                    stateMachine.ChangeState(ally.idleState);
                }
                else
                {
                    stateMachine.ChangeState(ally.moveState);
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }

    public override void SetCombo(bool combo)
    {
        base.SetCombo(combo);
    }
}
