using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class takes care of calculating the gravity and movement of a 2d Platformer entity.
 * This is mostly used for beings that are grounded (the player, humans, skeletons, etc.)
 * Not for flying entities e.g.: birds, helicopters, etc.
 * @author ShifatKhan
 */
public enum State
{
    idle,
    move, // Indicates state of when enemy is moving, which includes flying, running, walking, etc.
    attack,
    stagger,
    dead
}

[RequireComponent(typeof(Controller2D))]
public class Movement2D : MonoBehaviour
{
    [Header("Movement")]
    protected float maxJumpHeight = 2.5f; // Max height a jump can attain.
    protected float minJumpHeight = 0.5f;
    protected float timeToJumpApex = .35f; // How long (seconds) before reaching jumpHeight.

    protected float accelerationTimeAirborne = .2f;
    protected float accelerationTimeGrounded = .1f;

    [SerializeField] protected float moveSpeed = 6;

    protected float gravity;
    protected float maxJumpVelocity;
    protected float minJumpVelocity;
    protected Vector3 velocity;
    protected float velocityXSmoothing;

    protected Controller2D controller;
    protected Vector2 directionalInput;
    protected int faceDir = 1; // Hot fix. This is also in Controller2D, but this one is updated in Update().
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    protected Shader defaultShader;
    protected Shader hitShader;
    protected bool hitStopped;

    [SerializeField] // TODO: remove serialized
    protected State currentState;

    public virtual void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        defaultShader = spriteRenderer.material.shader;
        hitShader = Shader.Find("GUI/Text Shader"); // For all white sprite on Hit

        // Calculate gravity.
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        SetCurrentState(State.idle);
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

    /** Applies a directional forced to the being's body.
     */
    public virtual void ApplyForce(Vector3 direction)
    {
        velocity = direction;
    }


    // TODO: Move KnockBack to another script. Movement2D should be only for movement.
    /** Makes current being knocked backwards. Used for when the being is hit.
     * This also calls the HitStop function (if hit stop is enabled).
     */
    public virtual void KnockBack(Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        // TODO: Change how hit stop is called. What if GameObject is destroyed before Time is set back to normal?
        //      This will freeze the game. Need to call hitstop in hit animation maybe?
        if(hitStopEnabled)
            HitStop(hitStopDuration);

        ApplyForce(direction);
        SetCurrentState(State.stagger);
        StartCoroutine(KnockBackCo(knockTime));
    }

    /** Returns the state to idle after a certain time.
     */
    public virtual IEnumerator KnockBackCo(float knockTime)
    {
        yield return new WaitForSeconds(knockTime);
        SetCurrentState(State.idle);
    }

    /** Applies a hit stop effect when hit by making the sprite White (flash)
     * and stopping time.
     * 
     * We then call a coroutine to reset.
     */
    public virtual void HitStop(float duration)
    {
        spriteRenderer.material.shader = hitShader;
        spriteRenderer.material.color = Color.white;

        if (hitStopped)
            return;
        Time.timeScale = 0.0f;
        StartCoroutine(HitWait(duration));
    }

    /** Resets the hit stop so the sprite and timeScale goes back to normal.
     */
    public virtual IEnumerator HitWait(float duration)
    {
        hitStopped = true;
        yield return new WaitForSecondsRealtime(duration);
        spriteRenderer.material.shader = defaultShader;
        spriteRenderer.material.color = Color.white;
        Time.timeScale = 1.0f;
        hitStopped = false;
    }

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
    }

    /** Set beings state with a new state.
     */
    public void SetCurrentState(State newState)
    {
        if (currentState != newState)
            currentState = newState;
    }

    /** Updates being's state based on whether it is moving or not.
     */
    public virtual void UpdateState()
    {
        SetCurrentState(directionalInput.x != 0 ? State.move : State.idle);
    }

    /** Updates animation.
     */
    public virtual void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", directionalInput.x != 0);

            animator.SetBool("isAirborne", !controller.collisions.below);

            animator.SetBool("isStaggered", currentState == State.stagger);
        }

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

            // TODO: Change approach since we might want some children not to change.
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
