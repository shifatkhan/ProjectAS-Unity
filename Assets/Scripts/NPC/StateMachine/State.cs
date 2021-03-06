﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected FiniteStateMachine stateMachine;
    protected EntityNPC entity;

    public float startTime { get; protected set; }

    protected string animBoolName;

    /** Constructor
     */
    public State(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        startTime = Time.time;
        entity.animator.SetBool(animBoolName, true);
        DoChecks();
    }

    public virtual void Exit()
    {
        entity.animator.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public string GetAnimBoolName()
    {
        return this.animBoolName;
    }

    public virtual void DoChecks()
    {

    }
}
