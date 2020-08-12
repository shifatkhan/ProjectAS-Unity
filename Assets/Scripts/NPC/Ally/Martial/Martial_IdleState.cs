using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Martial_IdleState : IdleState
{
    private AllyMartial ally;

    public Martial_IdleState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, AllyMartial ally) : base(entity, stateMachine, animBoolName, stateData)
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

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (ally.GetJoinedPlayer())
        {
            if (!ally.CheckTargetInRadius() || ally.GetAttackEnemy())
            {
                ally.moveState.SetFollowTarget(true);
                stateMachine.ChangeState(ally.moveState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
