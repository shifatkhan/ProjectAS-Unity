using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samurai_PlayerDetectedState : PlayerDetectedState
{
    private EnemySamurai enemy;

    public Samurai_PlayerDetectedState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetectedState stateData, EnemySamurai enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        else if (performLongRangeAction)
        {
            switch(Random.Range(1, 3)) // Random moveset
            {
                case 1: // CHARGE
                    stateMachine.ChangeState(enemy.chargeState);
                    break;
                case 2: // DODGE
                    stateMachine.ChangeState(enemy.dodgeState);
                    break;
            }
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
