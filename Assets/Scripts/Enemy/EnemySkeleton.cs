using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class takes care of the AI for Enemy Skeleton.
 * @author ShifatKhan
 */
[RequireComponent(typeof(Controller2D))]
public class EnemySkeleton : Enemy
{
    [SerializeField] private float chaseDistance = 6; // Distance where enemy will chase target.
    [SerializeField] private float attackDistance = 0.8f; // Distance where enemy will attack target.

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 homePosition; // Place to return to if player is gone.

    private bool followTarget;

    public bool showAttackRadius = true;

    public override void Start()
    {
        base.Start();

        // Set default target to be the Player.
        if (target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        homePosition = gameObject.transform.position;
    }

    // TODO: Separate update and fixed update functions (currently doing both in CheckDistance)
    public override void Update()
    {
        //CheckDistance();
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(currentState != State.dead && currentState != State.stagger)
            CheckDistance();
    }

    /** Check if target is in radius. If so, enemy follows target until it is in attack range.
     * If player is out of sight, enemy returns to initial position.
     */
    void CheckDistance()
    {
        // TODO: Change distance calculation to only check for X (not position) - fixes: skeleton keeps walking even when near home position but on a different Y level.
        //      OR make it so skeleton can't fall off ledge.
        // If target is inside chase radius and outside attack radius.
        if (Vector2.Distance(target.position, transform.position) <= chaseDistance
            && Vector2.Distance(target.position, transform.position) > attackDistance)
        {
            directionalInput = (target.position - transform.position).normalized;
        }
        else if ((Vector2.Distance(homePosition, transform.position) <= chaseDistance && Vector2.Distance(homePosition, transform.position) > attackDistance)
            && Vector2.Distance(target.position, transform.position) >= chaseDistance)
        {
            // Target is gone, so return to home position.
            directionalInput = (homePosition - transform.position).normalized;
        }
        else if (Vector2.Distance(target.position, transform.position) <= attackDistance)
        {
            // TODO: Make enemy stop moving while attacking.
            // Target is in attack radius.
            directionalInput.x = 0;
            StartCoroutine(AttackCo());
        }
        else
        {
            directionalInput.x = 0;
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
        if (currentState != State.attack)
        {
            animator.SetTrigger("attack1");
            SetCurrentState(State.attack);

            yield return new WaitForSeconds(attackSpeed);

            SetCurrentState(State.idle);
        }
    }

    public override void UpdateState()
    {
        if(currentState != State.attack)
        {
            base.UpdateState();
        }
    }

    /** Debug: Draw skelton's attack radius.
     */
    void OnDrawGizmos()
    {
        if (showAttackRadius)
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawSphere(transform.position, attackDistance);
        }
    }
}
