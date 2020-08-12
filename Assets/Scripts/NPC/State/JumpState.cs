using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State
{
    protected D_JumpState stateData;

    public JumpState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_JumpState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        entity.Jump();
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
