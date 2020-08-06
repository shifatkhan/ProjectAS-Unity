using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class takes care of the AI for EntityNPC Sun.
 * 
 * @author ShifatKhan
 */
[RequireComponent(typeof(Controller2D))]
public class EnemySun : EntityNPC
{
    [Header("Attack")]
    [SerializeField] private float chaseDistance = 6; // Distance where enemy will chase target.
    [SerializeField] private float attackDistance = 0.8f; // Distance where enemy will attack target.

    [Header("Location")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 homePosition; // Place to return to if player is gone.

    public override void Start()
    {
        base.Start();
        // Set default target to be the Player.
        if(target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        homePosition = gameObject.transform.position;
    }

    public override void FixedUpdate()
    {
        CheckDistance();
    }

    void CheckDistance()
    {
        // If target is inside chase radius and outside attack radius.
        if(Vector2.Distance(target.position, transform.position) <= chaseDistance
            && Vector2.Distance(target.position, transform.position) > attackDistance
            && currentState != EntityState.stagger && currentState != EntityState.attack)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            SwitchState(EntityState.move);
        }
        else if ((Vector2.Distance(homePosition, transform.position) <= chaseDistance && Vector2.Distance(homePosition, transform.position) > attackDistance)
            && Vector2.Distance(target.position, transform.position) >= chaseDistance
            && currentState != EntityState.stagger && currentState != EntityState.attack)
        {
            // Target is gone, so return to home position.
            transform.position = Vector2.MoveTowards(transform.position, homePosition, moveSpeed * Time.deltaTime);
            SwitchState(EntityState.move);
        }
        else
        {
            directionalInput.x = 0;
            SwitchState(EntityState.idle);
        }
    }
}
