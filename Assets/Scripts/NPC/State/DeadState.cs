using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class DeadState : State
{
    protected D_DeadState stateData;

    public bool enableDeathParticle;
    public bool enableDeathAnimation;

    public DeadState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        GameObject chunks = GameObject.Instantiate(stateData.deathChunkParticle, entity.transform.position, stateData.deathChunkParticle.transform.rotation);
        GameObject blood = GameObject.Instantiate(stateData.deathBloodParticle, entity.transform.position, stateData.deathBloodParticle.transform.rotation);

        // Change blood direction depending on hit direction.
        if(entity.hitDirection.x < 0)
        {
            // TODO: Fix y-flipping.
            chunks.transform.Rotate(new Vector3(180, 0, 0));
            blood.transform.Rotate(new Vector3(180, 0, 0));
        }

        // Change blood color
        MainModule mainChunk = chunks.GetComponent<ParticleSystem>().main;
        mainChunk.startColor = stateData.deathChunkColor;

        MainModule mainBlood = blood.GetComponent<ParticleSystem>().main;
        mainBlood.startColor = stateData.deathBloodColor;

        entity.gameObject.SetActive(false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
