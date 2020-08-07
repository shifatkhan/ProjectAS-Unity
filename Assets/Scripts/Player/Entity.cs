using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class takes care of calculating the gravity and movement of a 2d Platformer entity.
 * This is mostly used for beings that are grounded (the player, humans, skeletons, etc.)
 * Might need to override all functions if used for flying entities (birds, helicopters, etc.)
 * @author ShifatKhan
 */
public enum EntityState
{
    idle,
    move, // Indicates state of when enemy is moving, which includes flying, running, walking, etc.
    attack,
    stagger,
    dead
}

[RequireComponent(typeof(Controller2D))]
public class Entity : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 6;

    protected float maxJumpHeight = 2.5f; // Max height a jump can attain.
    protected float minJumpHeight = 0.5f;
    protected float timeToJumpApex = .35f; // How long (seconds) before reaching jumpHeight.

    protected float accelerationTimeAirborne = .2f;
    protected float accelerationTimeGrounded = .1f;

    protected float gravity;
    protected float maxJumpVelocity;
    protected float minJumpVelocity;
    protected Vector3 velocity;
    private Vector2 velocityWorkspace; // Used as a temporary velocity to replace current velocity.
    protected float velocityXSmoothing;

    protected Controller2D controller;
    [SerializeField] protected Vector2 directionalInput;
    protected int faceDir = 1; // Hot fix. This is also in Controller2D, but this one is updated in Update().
    public Animator animator { get; protected set; }
    protected SpriteRenderer spriteRenderer;

    [SerializeField] // TODO: remove serialized
    protected EntityState currentState;

    public virtual void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Calculate gravity.
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        SwitchState(EntityState.idle);
    }
    
    public virtual void Update()
    {
        UpdateAnimator();
        UpdateState();
    }
    
    public virtual void FixedUpdate()
    {
        CalculateVelocity();

        Move();
    }

    // -- MOVEMENT ----------------------------------------------------------------------------------------------------------------

    /** Returns bool stating whether Being is grounded or not.
     */
    public bool IsGrounded()
    {
        return controller.collisions.below;
    }

    /** Return current velocity value.
     */
    public Vector3 GetVelocity()
    {
        return this.velocity;
    }

    /** Calculate and apply X and Y velocity to game object..
     */
    protected void CalculateVelocity()
    {
        // Smooth out change in directionX (when changing direction)
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        // Apply gravity to velocity.
        velocity.y += gravity * Time.deltaTime;
    }

    /** Makes being move based on the directionalInput's value.
     * If the being is grounded or hits a ceiling, it will stop moving 
     * in the Y-axis.
     */
    protected void Move()
    {
        controller.Move(velocity * Time.deltaTime, directionalInput);

        // Set velocity.y to 0 if gameObject is on ground or hit ceiling.
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    /** Applies a fixed directional forced to the entity's body.
     */
    public virtual void ApplyForce(Vector3 direction)
    {
        velocity = direction;
    }

    public virtual void SetVelocity(float force)
    {
        velocityWorkspace.Set(faceDir * force, velocity.y);
        velocity = velocityWorkspace;
    }

    /** A more sophisticated way of applying force to our body.
     */
    public virtual void SetVelocity(float force, Vector2 angle, int direction)
    {
        angle.Normalize();
        velocityWorkspace.Set(angle.x * force * direction, angle.y * force);
        velocity = velocityWorkspace;
    }

    // -- UPDATE DIRECTION ----------------------------------------------------------------------------------------------------------------

    /** Takes in input and assigns it.
    */
    public virtual void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;

        // Update facing direction.
        if (directionalInput.x != 0)
        {
            faceDir = (int)directionalInput.x;
        }

        UpdateSpriteDirection();
    }

    public virtual void FlipDirectionalInput()
    {
        SetDirectionalInput(new Vector2(directionalInput.x == 0 ? faceDir * -1 : directionalInput.x * -1, directionalInput.y));
    }

    public void MoveInFaceDir()
    {
        if(directionalInput.x == 0)
        {
            directionalInput.x = faceDir == 1 ? 1 : -1;
        }
    }

    public int GetFaceDir()
    {
        return this.faceDir;
    }

    /** Set beings state with a new state.
     */
    public virtual void SwitchState(EntityState newState)
    {
        if (currentState != newState)
            currentState = newState;
    }

    // -- UPDATE VISUALS ----------------------------------------------------------------------------------------------------------------

    /** Updates being's state based on whether it is moving or not.
     */
    public virtual void UpdateState()
    {
        SwitchState(directionalInput.x != 0 ? EntityState.move : EntityState.idle);
    }

    /** Updates animation.
     */
    public virtual void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("move", directionalInput.x != 0);

            animator.SetBool("airborne", !controller.collisions.below);

            animator.SetBool("stagger", currentState == EntityState.stagger);

            animator.SetFloat("yVelocity", velocity.y);
        }

        UpdateSpriteDirection();
    }

    protected void UpdateSpriteDirection()
    {
        if (directionalInput.x > 0) // Facing right
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (directionalInput.x < 0) // Facing left
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    [System.Obsolete("This function has been replaced by 'UpdateSpriteDirection()'")]
    protected void UpdateSpriteDirectionAndChildren()
    {
        if (spriteRenderer != null)
        {
            if (directionalInput.x > 0) // Facing right
            {
                spriteRenderer.flipX = false;
            }
            else if (directionalInput.x < 0) // Facing left
            {
                spriteRenderer.flipX = true;
            }
            
            // Flip CHILD gameObjects according to where the gameobject is facing.
            for (int i = 0; i < transform.childCount; i++)
            {
                Quaternion rotation = transform.GetChild(i).localRotation;
                rotation.y = spriteRenderer.flipX ? 180 : 0;
                transform.GetChild(i).localRotation = rotation;
            }
        }
    }
}
