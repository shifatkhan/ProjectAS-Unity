using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Martial_JumpState : JumpState
{
    private AllyMartial ally;

    public Martial_JumpState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_JumpState stateData, AllyMartial ally) : base(entity, stateMachine, animBoolName, stateData)
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

        if (ally.IsGrounded())
        {
            stateMachine.ChangeState(ally.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
