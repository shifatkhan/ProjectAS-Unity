using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_MoveState : MoveState
{
    public EnemySkeleton enemy;

    public Skeleton_MoveState(Enemy entity, FiniteStateMachine stateMachine, string animBoolName, MoveStateObject stateData, EnemySkeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isDetectingWall || !isDetectingGround)
        {
            enemy.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
