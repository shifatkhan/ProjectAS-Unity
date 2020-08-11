using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samurai_DodgeState : DodgeState
{
    private EnemySamurai enemy;

    public Samurai_DodgeState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_DodgeState stateData, EnemySamurai enemy) : base(entity, stateMachine, animBoolName, stateData)
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
        enemy.CreateAfterImage();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.ResetDodgeAngle();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isDodgeOver)
        {
            if (isPlayerInMaxAgroRange && performCloseRangeAction)
            {
                enemy.ChooseCombo();
            }
            else if (isPlayerInMaxAgroRange && !performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy.dodgeState);
            }
            else if (!isPlayerInMaxAgroRange)
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }
        else
        {
            enemy.CreateAfterImage();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
