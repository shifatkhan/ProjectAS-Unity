using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class takes care of the AI for Enemy Sun.
 * TODO: Find a better way to handle flying enemies.
 * @author ShifatKhan
 */
public class EnemySun : Enemy
{
    [SerializeField]
    private float chaseDistance = 6; // Distance where enemy will chase target.
    [SerializeField]
    private float attackDistance = 0.8f; // Distance where enemy will attack target.

    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 homePosition; // Place to return to if player is gone.

    Rigidbody2D rb;

    public override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            && currentState != State.stagger && currentState != State.attack)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            SetCurrentState(State.move);
        }
        else if ((Vector2.Distance(homePosition, transform.position) <= chaseDistance && Vector2.Distance(homePosition, transform.position) > attackDistance)
            && Vector2.Distance(target.position, transform.position) >= chaseDistance
            && currentState != State.stagger && currentState != State.attack)
        {
            // Target is gone, so return to home position.
            transform.position = Vector2.MoveTowards(transform.position, homePosition, moveSpeed * Time.deltaTime);
            SetCurrentState(State.move);
        }
        else
        {
            directionalInput.x = 0;
            SetCurrentState(State.idle);
        }
    }

    public override void ApplyForce(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.AddForce(direction);
        StartCoroutine(PushBack());
    }

    public IEnumerator PushBack()
    {
        yield return new WaitForSeconds(.33f);
        rb.isKinematic = true;
    }
}
