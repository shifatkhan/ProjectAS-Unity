using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected D_MoveState stateData;

    protected bool isDetectingWall;
    protected bool isDetectingGround;
    protected bool isPlayerInMinAgroRange;

    protected bool followTarget;

    public MoveState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isDetectingGround = entity.CheckGround();
        isDetectingWall = entity.CheckWall();
        isPlayerInMinAgroRange = entity.CheckPlayerInMaxAgroRange();

        if (followTarget)
        {
            entity.SetDirectionalInput(new Vector2(entity.CheckTargetHorizontalDir(), 0));
        }
    }

    public override void Enter()
    {
        base.Enter();

        if (followTarget)
        {
            entity.SetDirectionalInput(new Vector2(entity.CheckTargetHorizontalDir(), 0));
        }

        entity.SetMoveSpeed(stateData.movementSpeed);
        entity.MoveInFaceDir();
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

    public void SetFollowTarget(bool followTarget)
    {
        this.followTarget = followTarget;
    }
}
