using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_PlayerDetectedState : PlayerDetectedState
{
    private EnemyArcher enemy;

    public Archer_PlayerDetectedState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetectedState stateData, EnemyArcher enemy) : base(entity, stateMachine, animBoolName, stateData)
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
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
