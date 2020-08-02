using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    protected IdleStateObject stateData;

    protected bool flipAfterIdle; // For when we want to keep walking straight after idle.
    protected bool isIdleTimeOver;
    protected bool isPlayerInMinAgroRange;

    protected float idleTime;

    public IdleState(Enemy entity, FiniteStateMachine stateMachine, string animBoolName, IdleStateObject stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();

        entity.SetDirectionalInput(Vector2.zero);
        isIdleTimeOver = false;
        isPlayerInMinAgroRange = entity.CheckPlayerInMaxAgroRange();

        SetRandomIdleTime();
    }

    public override void Exit()
    {
        base.Exit();
        
        if (flipAfterIdle)
        {
            entity.FlipDirectionalInput();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Check if idle is over.
        if(Time.time >= startTime + idleTime)
        {
            isIdleTimeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public void SetFlipAfterIdle(bool flipAfterIdle)
    {
        this.flipAfterIdle = flipAfterIdle;
    }

    /** Set a random amount of time for which to idle.
     */
    private void SetRandomIdleTime()
    {
        idleTime = Random.Range(stateData.minIdleTime, stateData.maxIdleTime);
    }
}
