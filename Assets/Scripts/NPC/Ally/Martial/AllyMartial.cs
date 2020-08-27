using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyMartial : EntityNPC
{
    public Martial_MoveState moveState { get; private set; }
    public Martial_IdleState idleState { get; private set; }
    public Martial_JumpState jumpState { get; private set; }
    public Martial_Attack1State attack1State { get; private set; }
    public Martial_Attack2State attack2State { get; private set; }

    [Header("States")]
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_JumpState jumpStateData;
    [SerializeField] private D_MeleeAttackState attack1StateData;
    [SerializeField] private D_MeleeAttackState attack2StateData;

    private bool joinedPlayer;

    public override void Start()
    {
        base.Start();

        // Initialize states.
        moveState = new Martial_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Martial_IdleState(this, stateMachine, "IDLE", idleStateData, this);
        jumpState = new Martial_JumpState(this, stateMachine, "jump", jumpStateData, this);
        attack1State = new Martial_Attack1State(this, stateMachine, "attack1", attack1StateData, this);
        attack2State = new Martial_Attack2State(this, stateMachine, "attack2", attack2StateData, this);

        stateMachine.Initialize(idleState);
    }

    public override void Damage(float damage, Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        base.Damage(damage, direction, knockTime, hitStopEnabled, hitStopDuration);

        if (isDead)
        {
            //stateMachine.ChangeState(deadState);
        }
    }

    public void ChooseCombo()
    {
        switch (Random.Range(1, 2)) // Random moveset
        {
            case 1: // ATTACK 1
                attack1State.SetCombo(Utils.GetRandomBool());
                stateMachine.ChangeState(attack1State);
                break;
            case 2: // ATTACK 2
                attack2State.SetCombo(Utils.GetRandomBool());
                stateMachine.ChangeState(attack2State);
                break;
        }
    }

    public bool GetJoinedPlayer()
    {
        return joinedPlayer;
    }

    public void SetJoinedPlayer(bool joinedPlayer)
    {
        this.joinedPlayer = joinedPlayer;
    }
}
