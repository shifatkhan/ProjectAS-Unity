using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected MoveStateObject stateData;

    protected bool isDetectingWall;
    protected bool isDetectingGround;

    public MoveState(Enemy entity, FiniteStateMachine stateMachine, string animBoolName, MoveStateObject stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();
        entity.MoveInFaceDir();

        isDetectingGround = entity.CheckGround();
        isDetectingWall = entity.CheckWall();
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

        isDetectingGround = entity.CheckGround();
        isDetectingWall = entity.CheckWall();
    }
}
