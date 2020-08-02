﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/** This class holds basic data for an Enemy object.
 * 
 * TODO: Move state machine and AI into a separate parent class called "Entity" or "AI"
 * 
 * @author ShifatKhan
 */
public class Enemy : Entity
{
    [Header("Enemy vars")]
    [SerializeField] protected FloatVariable currentHealth; // TODO: Move to Entity script?
    [SerializeField] protected string enemyName = "Enemy"; // TODO: Move to Entity script?
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float attackSpeed = 0f;

    [Header("Hit")]
    public bool enableDeathParticle;
    public bool enableDeathAnimation;

    [SerializeField] private GameObject hitParticle;
    [SerializeField] private GameObject deathChunkParticle;
    [SerializeField] private GameObject deathBloodParticle;

    [SerializeField] private Color deathChunkColor;
    [SerializeField] private Color deathBloodColor;

    private Shader defaultShader; // Default color
    private Shader hitShader; // Color when hit
    private bool hitStopped; // Whether time is stopped or not
    private Vector3 hitDirection; // Direction at which to be knocked back
    private float knockTime; // Time Length of knockback

    [Header("AI")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform playerCheck;

    private LayerMask groundLayerMask;
    private LayerMask playerLayerMask;

    [Header("AI State machine")]
    public FiniteStateMachine stateMachine;
    public EnemyObject entityData;

    public override void Start()
    {
        base.Start();

        //// Hit
        //defaultShader = spriteRenderer.material.shader;
        //hitShader = Shader.Find("GUI/Text Shader"); // For all white sprite on Hit

        // AI
        SetDirectionalInput(new Vector2(1,0));

        groundLayerMask = controller.GetCollisionMask();
        playerLayerMask = LayerMask.GetMask("Player");

        stateMachine = new FiniteStateMachine();
    }

    public override void Update()
    {
        //base.Update();

        stateMachine.currentState.LogicUpdate();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, transform.right * faceDir, entityData.wallCheckDistance, groundLayerMask);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, entityData.groundCheckDistance, groundLayerMask);
    }

    // TODO: FIX player detected. Doesn't work when in Move state.
    public virtual bool CheckPlayerInMinAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right * faceDir, entityData.minAgroDistance, playerLayerMask);
    }

    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right * faceDir, entityData.maxAgroDistance, playerLayerMask);
    }

    //-- OTHER FUNCTIONS --------------------------------------------------------------------------------

    /** Makes current being knocked backwards. Used for when the being is hit.
     * This also calls the HitStop function (if hit stop is enabled).
     */
    public void Damage(float damage, Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        // TODO: Change how hit stop is called. What if GameObject is destroyed before Time is set back to normal?
        //      This will freeze the game. Need to call hitstop in hit animation maybe?

        // TODO: Turn enemy around if player attacks from behind.

        // Display hit effect. If player is hitting from the right, we flip the effect.
        Instantiate(hitParticle, transform.position, Quaternion.Euler(0.0f, direction.x < 0 ? 180.0f : 0.0f, Random.Range(0.0f, -90.0f)));

        if (hitStopEnabled)
            HitStop(hitStopDuration);

        this.hitDirection = direction;
        this.knockTime = knockTime;

        TakeDamage(damage);
    }

    /** Returns the state to idle after a certain time.
     */
    public IEnumerator DamageCo(float knockTime)
    {
        yield return new WaitForSeconds(knockTime);
        SwitchState(EntityState.move);
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

    public void TakeDamage(float damage)
    {
        currentHealth.RuntimeValue -= damage;
        if(currentHealth.RuntimeValue > 0)
        {
            SwitchState(EntityState.stagger);
        }
        else
        {
            SwitchState(EntityState.dead);
        }
    }

    /** We don't 'destroy' since it will call the garbage collector = inefficient.
     * Instead, we set the gameObject to 'inactive'.
     */
    public void Die()
    {
        gameObject.SetActive(false);
    }

    /** Debug: Draw checks
     */
    public virtual void OnDrawGizmos()
    {
        // AI
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - entityData.groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + entityData.wallCheckDistance * faceDir, wallCheck.position.y));

        Gizmos.DrawLine(playerCheck.position, new Vector2(playerCheck.position.x + entityData.maxAgroDistance * faceDir, playerCheck.position.y));
    }
}
