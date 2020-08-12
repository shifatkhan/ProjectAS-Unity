using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySamurai : EntityNPC
{
    public Samurai_MoveState moveState { get; private set; }
    public Samurai_IdleState idleState { get; private set; }
    public Samurai_PlayerDetectedState playerDetectedState { get; private set; }
    public Samurai_DodgeState dodgeState { get; private set; }
    public Samurai_KickState kickState { get; private set; }
    public Samurai_PunchState punchState { get; private set; }
    public Samurai_AxeKickState axeKickState { get; private set; }
    public Samurai_ChargeState chargeState { get; private set; }

    // STAGE 2
    public Samurai_AttackSword1 attackSwordState1 { get; private set; }
    public Samurai_AttackSword2 attackSwordState2 { get; private set; }
    public Samurai_AttackSword3 attackSwordState3 { get; private set; }

    [Header("States")]
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_PlayerDetectedState playerDetectedStateData;
    [SerializeField] private D_DodgeState dodgeStateData;
    [SerializeField] private D_MeleeAttackState kickStateData;
    [SerializeField] private D_MeleeAttackState punchStateData;
    [SerializeField] private D_MeleeAttackState axeKickStateData;
    [SerializeField] private D_ChargeState chargeStateData;

    // STAGE 2
    [SerializeField] private D_MeleeAttackState attackSword1StateData;
    [SerializeField] private D_MeleeAttackState attackSword2StateData;
    [SerializeField] private D_MeleeAttackState attackSword3StateData;

    public bool isInStage2 = false;

    [Header("After Images")]
    [SerializeField] private float distanceBetweenAfterImages;
    [SerializeField] private PlayerAfterImagePool afterImagePool;
    private float lastAfterImageXPos;
    private float lastAfterImageYPos;

    public override void Start()
    {
        base.Start();

        // Initialize states.
        moveState = new Samurai_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new Samurai_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Samurai_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);
        dodgeState = new Samurai_DodgeState(this, stateMachine, "dodge", dodgeStateData, this);
        kickState = new Samurai_KickState(this, stateMachine, "kick", kickStateData, this);
        punchState = new Samurai_PunchState(this, stateMachine, "punch", punchStateData, this);
        axeKickState = new Samurai_AxeKickState(this, stateMachine, "axeKick", axeKickStateData, this);
        chargeState = new Samurai_ChargeState(this, stateMachine, "charge", chargeStateData, this);

        attackSwordState1 = new Samurai_AttackSword1(this, stateMachine, "kick", attackSword1StateData, this);
        attackSwordState2 = new Samurai_AttackSword2(this, stateMachine, "punch", attackSword2StateData, this);
        attackSwordState3 = new Samurai_AttackSword3(this, stateMachine, "axeKick", attackSword3StateData, this);

        stateMachine.Initialize(moveState);

        if (!afterImagePool)
        {
            afterImagePool = GameObject.Find("SamuraiAfterImagePool").GetComponent<PlayerAfterImagePool>();
        }
    }

    public override void Damage(float damage, Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        base.Damage(damage, direction, knockTime, hitStopEnabled, hitStopDuration);

        if (isDead)
        {
            //stateMachine.ChangeState(deadState);
        }
        else if (isStunned )//&& stateMachine.currentState != stunState)
        {
            //stateMachine.ChangeState(stunState);
        }
        else if (CheckTargetInMinAgroRange())
        {
            //stateMachine.ChangeState(rangedAttackState);
        }
        else if (!CheckTargetInMinAgroRange())
        {
            //lookForPlayerState.SetTurnImmediately(true);
            //stateMachine.ChangeState(lookForPlayerState);
        }
    }

    public void ChooseCombo()
    {
        if (!isInStage2)
            switch (Random.Range(1, 4)) // Random moveset
            {
                case 1: // KICK
                    kickState.SetCombo(Utils.GetRandomBool());
                    stateMachine.ChangeState(kickState);
                    break;
                case 2: // PUNCH
                    punchState.SetCombo(Utils.GetRandomBool());
                    stateMachine.ChangeState(punchState);
                    break;
                case 3: // AXE KICK
                    axeKickState.SetCombo(Utils.GetRandomBool());
                    stateMachine.ChangeState(axeKickState);
                    break;
            }
        else
            switch (Random.Range(1, 4)) // Random moveset
            {
                case 1: // Sword attack 1
                    attackSwordState1.SetCombo(Utils.GetRandomBool());
                    stateMachine.ChangeState(attackSwordState1);
                    break;
                case 2: // Sword attack 2
                    attackSwordState2.SetCombo(Utils.GetRandomBool());
                    stateMachine.ChangeState(attackSwordState2);
                    break;
                case 3: // Sword attack 3
                    attackSwordState3.SetCombo(Utils.GetRandomBool());
                    stateMachine.ChangeState(attackSwordState3);
                    break;
            }
    }

    public void SetDodgeAngle(Vector2 angle)
    {
        angle.x = -Mathf.Abs(angle.x);
        dodgeStateData.runtimeDodgeAngle = angle;
    }

    public void ResetDodgeAngle()
    {
        dodgeStateData.runtimeDodgeAngle = dodgeStateData.dodgeAngle;
    }

    public float GetDodgeCooldown()
    {
        return dodgeStateData.dodgeCooldown;
    }

    public void SwitchToStage2()
    {
        isInStage2 = true;
        animator.SetBool("stage2", true);
    }

    public void ResetStates()
    {
        animator.SetBool("move", false);
        animator.SetBool("idle", false);
        animator.SetBool("playerDetected", false);
        animator.SetBool("punch", false);
        animator.SetBool("kick", false);
        animator.SetBool("axeKick", false);
        animator.SetBool("dodge", false);
        animator.SetBool("charge", false);
    }

    public void CreateAfterImage()
    {
        if (Mathf.Abs(transform.position.x - lastAfterImageXPos) > distanceBetweenAfterImages
            || Mathf.Abs(transform.position.y - lastAfterImageYPos) > distanceBetweenAfterImages)
        {
            afterImagePool.GetFromPool();
            lastAfterImageXPos = transform.position.x;
            lastAfterImageYPos = transform.position.y;
        }
    }
}
