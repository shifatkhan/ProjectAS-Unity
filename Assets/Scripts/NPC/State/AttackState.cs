using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected bool isAnimationFinished;
    protected bool isPlayerInMinAgroRange;

    public AttackState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();

        entity.attackState = this;
        isAnimationFinished = false;
        entity.SetMoveSpeed(0);
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

    public virtual void TriggerAttack()
    {
        // To be overriden for different attacks (melee, ranged, etc)
    }

    public virtual void FinishAttack()
    {
        // To be overriden for different attacks (melee, ranged, etc)
        isAnimationFinished = true;
    }
}
