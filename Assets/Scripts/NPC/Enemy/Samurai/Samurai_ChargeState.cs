using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samurai_ChargeState : ChargeState
{
    private EnemySamurai enemy;

    public Samurai_ChargeState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, EnemySamurai enemy) : base(entity, stateMachine, animBoolName, stateData)
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

        if (performCloseRangeAction)
        {
            enemy.ChooseCombo();
        }
        else if (isDetectingWall || !isDetectingGround)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
        else if (isChargeTimeOver)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
