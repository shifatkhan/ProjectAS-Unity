using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_DeadState : DeadState
{
    private EnemySkeleton enemy;

    public Skeleton_DeadState(Enemy entity, FiniteStateMachine stateMachine, string animBoolName, DeadStateObject stateData, EnemySkeleton enemy) : base(entity, stateMachine, animBoolName, stateData)
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
