using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Archer enemy that will take care of its finite state machine.
 * This enemy script is created for testing purposes.
 * 
 * @author ShifatKhan
 */
public class EnemyArcher : EntityNPC
{
    public Archer_MoveState moveState { get; private set; }
    public Archer_IdleState idleState { get; private set; }
    public Archer_PlayerDetectedState playerDetectedState { get; private set; }
    public Archer_MeleeAttackState meleeAttackState { get; private set; }
    public Archer_LookForPlayerState lookForPlayerState { get; private set; }
    public Archer_StunState stunState { get; private set; }
    public Archer_DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedStateData;
    [SerializeField] private D_MeleeAttackState meleeAttackStateData;
    [SerializeField] private D_LookForPlayerState lookForPlayerStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;

    public override void Start()
    {
        base.Start();

        // Initialize states.
        moveState = new Archer_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Archer_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Archer_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);
        meleeAttackState = new Archer_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackStateData, this);
        lookForPlayerState = new Archer_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new Archer_StunState(this, stateMachine, "stun", stunStateData, this);
        deadState = new Archer_DeadState(this, stateMachine, "dead", deadStateData, this);

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
}
