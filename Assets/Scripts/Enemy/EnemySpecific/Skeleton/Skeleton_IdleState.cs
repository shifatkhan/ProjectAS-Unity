using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_IdleState : IdleState
{
    public EnemySkeleton enemy;

    public Skeleton_IdleState(Enemy entity, FiniteStateMachine stateMachine, string animBoolName, IdleStateObject stateData, EnemySkeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
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
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isIdleTimeOver)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
