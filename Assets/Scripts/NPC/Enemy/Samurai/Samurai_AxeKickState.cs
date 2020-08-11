using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samurai_AxeKickState : MeleeAttackState
{
    private EnemySamurai enemy;

    public Samurai_AxeKickState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_MeleeAttackState stateData, EnemySamurai enemy) : base(entity, stateMachine, animBoolName, stateData)
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

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
            else if (!isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }

    public override void SetCombo(bool combo)
    {
        base.SetCombo(combo);
    }
}
