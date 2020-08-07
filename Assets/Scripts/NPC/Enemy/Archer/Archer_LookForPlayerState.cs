using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_LookForPlayerState : LookForPlayerState
{
    private EnemyArcher enemy;

    public Archer_LookForPlayerState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_LookForPlayerState stateDate, EnemyArcher enemy) : base(entity, stateMachine, animBoolName, stateDate)
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

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isAllTurnsTimeDone)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
