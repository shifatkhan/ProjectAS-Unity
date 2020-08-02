using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_PlayerDetectedState : PlayerDetectedState
{
    private EnemySkeleton enemy;

    public Skeleton_PlayerDetectedState(Enemy entity, FiniteStateMachine stateMachine, string animBoolName, PlayerDetectedObject stateData, EnemySkeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        if (!isPlayerInMaxAgroRange)
        {
            Debug.Log("player detected: PLAYER NOT IN MAX");
            enemy.idleState.SetFlipAfterIdle(false);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
