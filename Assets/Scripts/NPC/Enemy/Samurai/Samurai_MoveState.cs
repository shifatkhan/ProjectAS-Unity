using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samurai_MoveState : MoveState
{
    private EnemySamurai enemy;

    public Samurai_MoveState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, EnemySamurai enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        SetFollowTarget(true);

        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Check if enemy is below 50% hp.
        if(!enemy.isInStage2 && enemy.GetCurrentHealth() / enemy.GetMaxHealth() <= 0.5)
        {
            enemy.SwitchToStage2();

            enemy.ResetStates();
            stateMachine.ChangeState(enemy.idleState);
        }
        else if (enemy.isInStage2 && Utils.GetRandomBool())
        {
            if(Time.time >= enemy.dodgeState.startTime + enemy.GetDodgeCooldown())
            {
                enemy.SetDodgeAngle(enemy.GetPlayerTransform().position - enemy.transform.position);
                stateMachine.ChangeState(enemy.dodgeState);
            }
            
        }

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isDetectingWall || !isDetectingGround)
        {
            enemy.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
