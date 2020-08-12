using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyMartial : EntityNPC
{
    public Martial_MoveState moveState { get; private set; }
    public Martial_IdleState idleState { get; private set; }
    public Martial_JumpState jumpState { get; private set; }

    [Header("States")]
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_JumpState jumpStateData;

    private bool followPlayer;

    public override void Start()
    {
        base.Start();

        // Initialize states.
        moveState = new Martial_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Martial_IdleState(this, stateMachine, "idle", idleStateData, this);
        jumpState = new Martial_JumpState(this, stateMachine, "jump", jumpStateData, this);

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
        //switch (Random.Range(1, 4)) // Random moveset
        //{
        //    case 1: // KICK
        //        kickState.SetCombo(Utils.GetRandomBool());
        //        stateMachine.ChangeState(kickState);
        //        break;
        //    case 2: // PUNCH
        //        punchState.SetCombo(Utils.GetRandomBool());
        //        stateMachine.ChangeState(punchState);
        //        break;
        //    case 3: // AXE KICK
        //        axeKickState.SetCombo(Utils.GetRandomBool());
        //        stateMachine.ChangeState(axeKickState);
        //        break;
        //}
    }

    public bool GetFollowPlayer()
    {
        return followPlayer;
    }

    public void SetFollowPlayer(bool followPlayer)
    {
        this.followPlayer = followPlayer;
    }
}
