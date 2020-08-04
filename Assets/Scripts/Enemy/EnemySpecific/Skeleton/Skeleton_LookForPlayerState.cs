using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_LookForPlayerState : LookForPlayerState
{
    private EnemySkeleton enemy;

    public Skeleton_LookForPlayerState(Enemy entity, FiniteStateMachine stateMachine, string animBoolName, LookForPlayerStateObject stateDate, EnemySkeleton enemy) : base(entity, stateMachine, animBoolName, stateDate)
    {
        this.enemy = enemy;
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

        if (isPlayerInMinAgroRange)
        {
            // If player was found.
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isAllTurnsTimeDone)
        {
            // If no player was found.
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
