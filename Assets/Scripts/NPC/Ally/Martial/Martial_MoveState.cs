﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Martial_MoveState : MoveState
{
    private AllyMartial ally;

    public Martial_MoveState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, AllyMartial ally) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.ally = ally;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        ally.CheckIfPlayersTargetDead();
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

        if (ally.GetAttackEnemy() && performCloseRangeAction)
        {
            ally.ChooseCombo();
        }
        else if (!ally.GetJoinedPlayer() || ally.CheckTargetInRadius())
        {
            stateMachine.ChangeState(ally.idleState);
        }
        else if (isDetectingWall || !isDetectingGround)
        {
            stateMachine.ChangeState(ally.jumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
