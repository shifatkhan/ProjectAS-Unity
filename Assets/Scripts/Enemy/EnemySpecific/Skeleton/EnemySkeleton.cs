using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class takes care of the AI for Enemy Skeleton.
 * 
 * @Special thanks to Bardent (https://youtu.be/K2SbThbGw6w)
 * @author ShifatKhan
 */
[RequireComponent(typeof(Controller2D))]
public class EnemySkeleton : Enemy
{
    [Header("Attack")]
    [SerializeField] private float chaseDistance = 6; // Distance where enemy will chase target.
    [SerializeField] private float attackDistance = 0.8f; // Distance where enemy will attack target.

    [Header("Location")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 homePosition; // Place to return to if player is gone.

    private bool followTarget;

    //------------
    public Skeleton_IdleState idleState { get; private set; }
    public Skeleton_MoveState moveState { get; private set; }
    public Skeleton_PlayerDetectedState playerDetectedState { get; private set; }

    [Header("States")]
    [SerializeField] private IdleStateObject idleStateData;
    [SerializeField] private MoveStateObject moveStateData;
    [SerializeField] private PlayerDetectedObject playerDetectedData;
    //------------

    [Header("Debug")]
    public bool showAttackRadius = true;

    //-- LIFE CYCLES --------------------------------------------------------------------------------

    public override void Start()
    {
        base.Start();

        // Set default target to be the Player.
        //if (target == null)
        //{
        //    target = GameObject.FindWithTag("Player").transform;
        //}

        //homePosition = gameObject.transform.position;

        moveState = new Skeleton_MoveState(this, stateMachine, "isMoving", moveStateData, this);
        idleState = new Skeleton_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new Skeleton_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedData, this);

        stateMachine.Initialize(moveState);
    }

    /** Check if target is in radius. If so, enemy follows target until it is in attack range.
     * If player is out of sight, enemy returns to initial position.
     */
    [System.Obsolete("This Skeleton AI is being replaced by a state machine AI from Enemy.cs class")]
    void CheckDistance()
    {
        // TODO: Change distance calculation to only check for X (not position) - fixes: skeleton keeps walking even when near home position but on a different Y level.
        //      OR make it so skeleton can't fall off ledge.
        // If target is inside chase radius and outside attack radius.
        if (Mathf.Abs(target.position.x - transform.position.x) <= chaseDistance
            && Mathf.Abs(target.position.x - transform.position.x) > attackDistance)
        {
            SetDirectionalInput((target.position - transform.position).normalized);
        }
        else if ((Mathf.Abs(homePosition.x - transform.position.x) <= chaseDistance && Mathf.Abs(homePosition.x - transform.position.x) > attackDistance)
            && Mathf.Abs(target.position.x - transform.position.x) >= chaseDistance)
        {
            // Target is gone, so return to home position.
            SetDirectionalInput((homePosition - transform.position).normalized);
        }
        else if (Vector2.Distance(target.position, transform.position) <= attackDistance)
        {
            // TODO: Make enemy stop moving while attacking.
            // Target is in attack radius.
            SetDirectionalInput(Vector2.zero);
            StartCoroutine(AttackCo());
        }
        else
        {
            SetDirectionalInput(Vector2.zero);
        }
    }

    public void Jump()
    {
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
        }
    }

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

    /** Debug: Draw skelton's attack radius.
     */
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (showAttackRadius)
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawSphere(transform.position, attackDistance);
        }
    }
}
