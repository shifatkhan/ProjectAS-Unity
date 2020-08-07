using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class takes care of the AI for Enemy Skeleton.
 * 
 * @Special thanks to Bardent (https://youtu.be/K2SbThbGw6w)
 * @author ShifatKhan
 */
public class EnemySkeleton : EntityNPC
{
    public Skeleton_IdleState idleState { get; private set; }
    public Skeleton_MoveState moveState { get; private set; }
    public Skeleton_PlayerDetectedState playerDetectedState { get; private set; }
    public Skeleton_ChargeState chargeState { get; private set; }
    public Skeleton_LookForPlayerState lookForPlayerState { get; private set; }
    public Skeleton_MeleeAttackState meleeAttackState { get; private set; }
    public Skeleton_StunState stunState { get; private set; }
    public Skeleton_DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedData;
    [SerializeField] private D_ChargeState chargeStateData;
    [SerializeField] private D_LookForPlayerState lookForPlayerStateData;
    [SerializeField] private D_MeleeAttackState meleeAttackStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;

    public override void Start()
    {
        base.Start();

        // Initialize states.
        moveState = new Skeleton_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Skeleton_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Skeleton_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedData, this);
        chargeState = new Skeleton_ChargeState(this, stateMachine, "charge", chargeStateData, this);
        lookForPlayerState = new Skeleton_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        meleeAttackState = new Skeleton_MeleeAttackState(this, stateMachine, "attack1", meleeAttackStateData, this);
        stunState = new Skeleton_StunState(this, stateMachine, "stun", stunStateData, this);
        deadState = new Skeleton_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(moveState);
    }

    public override void Damage(float damage, Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        base.Damage(damage, direction, knockTime, hitStopEnabled, hitStopDuration);

        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
        else if (isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }
    }

    // TODO: Move attackSpeed into Skeleton_MeleeAttackState.
    [System.Obsolete("This function has been replaced by 'Skeleton_MeleeAttackState'")]
    public IEnumerator AttackCo()
    {
        if (currentState != EntityState.attack)
        {
            animator.SetTrigger("attack1");
            base.SwitchState(EntityState.attack);

            yield return new WaitForSeconds(attackSpeed);

            base.SwitchState(EntityState.idle);
        }
    }

    // TODO: remove since it's not used.
    public override void UpdateState()
    {
        if(currentState != EntityState.attack)
        {
            base.UpdateState();
        }
    }
}
