using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : AttackState
{
    protected D_RangedAttackState stateData;

    protected GameObject projectile;
    protected Projectile projectileScript;

    public RangedAttackState(EntityNPC entity, FiniteStateMachine stateMachine, string animBoolName, D_RangedAttackState stateData) : base(entity, stateMachine, animBoolName)
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        projectile = GameObject.Instantiate(stateData.projectile, entity.transform.position, entity.transform.rotation);
        projectileScript = projectile.GetComponent<Projectile>();

        // Change direction depending on facedirection direction.
        if (entity.GetFaceDir() < 0)
        {
            projectile.transform.localScale = new Vector3(-1, 1, 1);
        }

        projectileScript.FireProjectile(projectile.transform.localScale.x * stateData.projectileSpeed, stateData.projectileTravelDistance);
    }
}
