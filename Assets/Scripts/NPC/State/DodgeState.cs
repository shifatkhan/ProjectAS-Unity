using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Don't dodge off ledge.
public class DodgeState : State
{
    protected D_DodgeState stateData;

    protected bool performCloseRangeAction;
    protected bool isPlayerInMaxAgroRange;
    protected bool isDodgeOver; // Cooldown for dodging.

    public DodgeState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_DodgeState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        performCloseRangeAction = entity.CheckTargetInCloseRangeAction();
        isPlayerInMaxAgroRange = entity.CheckTargetInMaxAgroRange();
    }

    public override void Enter()
    {
        base.Enter();

        isDodgeOver = false;

        entity.SetVelocity(stateData.dodgeSpeed, stateData.runtimeDodgeAngle, -entity.GetFaceDir());
    }

    public override void Exit()
    {
        base.Exit();

        //entity.SetVelocity(0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + stateData.dodgeTime && entity.IsGrounded())
        {
            isDodgeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
