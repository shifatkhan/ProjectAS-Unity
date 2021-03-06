﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/** This class holds basic data for an NPC.
 * This will take care of checking player's position and will help the NPC's
 * finite state machine work.
 * 
 * @author ShifatKhan
 */
public class EntityNPC : Entity
{
    [Header("EntityNPC vars")]
    [SerializeField] protected float currentHealth; // TODO: Move to Entity script?
    [SerializeField] protected string enemyName = "NPC"; // TODO: Move to Entity script?
    [SerializeField] protected int attackDamage = 1; // TODO: Remove since it is in KnockBack script.
    [SerializeField] protected float attackSpeed = 0f;

    // Checks for when stunned.
    protected float currentStunResistance;
    protected float lastDamageTime;
    protected bool isStunned;
    protected bool isDead;

    [Header("Hit")]
    private Shader defaultShader; // Default color
    private Shader hitShader; // Color when hit
    private bool hitStopped; // Whether time is stopped or not
    public Vector3 hitDirection { get; private set; } // Direction at which to be knocked back
    private float knockTime; // Time Length of knockback

    [Header("AI checks")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform targetCheck;

    private Transform target;
    public AgroEvent playerAgroEvent;
    protected bool attackEnemy;

    private LayerMask groundLayerMask;
    
    [Header("AI State machine")]
    public FiniteStateMachine stateMachine;
    public D_Entity entityData;
    public AttackState attackState; // TODO: Remove

    public override void Start()
    {
        base.Start();

        currentStunResistance = entityData.stunResistance;
        currentHealth = entityData.maxHealth;

        // Hit
        defaultShader = spriteRenderer.material.shader;
        hitShader = Shader.Find("GUI/Text Shader"); // For all white sprite on Hit

        // AI
        SetDirectionalInput(new Vector2(1,0));
        groundLayerMask = controller.GetCollisionMask();
        ResetTarget(); // To get player's position.

        stateMachine = new FiniteStateMachine();
    }

    public override void Update()
    {
        stateMachine.currentState.LogicUpdate();
        animator.SetFloat("yVelocity", velocity.y);

        // Check if enemy was hit fast enough or not for stun.
        if (Time.time >= lastDamageTime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        stateMachine.currentState.PhysicsUpdate();
    }

    // -- CHECKS ----------------------------------------------------------------------------------------------------------------

    /** Check if there is a wall in front of game object.
    */
    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, transform.right * faceDir, entityData.wallCheckDistance, groundLayerMask);
    }

    /** Check if there is ground in front of game object.
     */
    public virtual bool CheckGround()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, entityData.groundCheckDistance, groundLayerMask);
    }

    /** Check if the player is in agro range for actions.
     */
    public virtual bool CheckTargetInMinAgroRange()
    {
        return Physics2D.Raycast(targetCheck.position, transform.right * faceDir, entityData.minAgroDistance, LayerMask.GetMask(LayerMask.LayerToName(target.gameObject.layer)));
    }

    /** Check if the player is in agro range for actions.
     */
    public virtual bool CheckTargetInMaxAgroRange()
    {
        return Physics2D.Raycast(targetCheck.position, transform.right * faceDir, entityData.maxAgroDistance, LayerMask.GetMask(LayerMask.LayerToName(target.gameObject.layer)));
    }

    /** Check if the player is in melee range for melee attacks or actions.
     */
    public virtual bool CheckTargetInCloseRangeAction()
    {
        return Physics2D.Raycast(targetCheck.position, transform.right * faceDir, entityData.closeRangeActionDistance, LayerMask.GetMask(LayerMask.LayerToName(target.gameObject.layer)));
    }

    /** Checks whether player is on the right or left side of the NPC.
     */
    public virtual int CheckTargetHorizontalDir()
    {
        return target.position.x <= transform.position.x ? -1 : 1;
    }
    
    /** Checks whether player is above or below the NPC.
     */
    public virtual int CheckTargetVerticalDir()
    {
        return target.position.y <= transform.position.y ? -1 : 1;
    }

    public virtual bool CheckTargetInRadius()
    {
        return (target.position - transform.position).magnitude <= entityData.radiusAgroDistance;
    }

    // -- GETTERS & SETTERS ----------------------------------------------------------------------------------------------------------------

    public Transform GetTargetTransform()
    {
        return target;
    }

    public void SetTarget(Transform followTargetPos)
    {
        this.target = followTargetPos;
    }

    public void SetTargetToPlayersTarget()
    {
        if (playerAgroEvent != null)
        {
            attackEnemy = true;

            if (target != playerAgroEvent.target || target.GetComponent<EntityNPC>().GetCurrentHealth() <= 0)
            {
                target = playerAgroEvent.target;
            }
        }
    }

    public void CheckIfPlayersTargetDead()
    {
        if(target && target.GetComponent<EntityNPC>() && target.GetComponent<EntityNPC>().GetCurrentHealth() <= 0)
        {
            ResetTarget();
        }
    }

    public void ResetTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        attackEnemy = false;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return entityData.maxHealth;
    }

    public bool GetAttackEnemy()
    {
        return attackEnemy;
    }

    // -- ACTIONS ----------------------------------------------------------------------------------------------------------------

    /** Makes current being knocked backwards. Used for when the being is hit.
     * This also calls the HitStop function (if hit stop is enabled).
     */
    public virtual void Damage(float damage, Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        // Turn Entity around if the attacker attacks from behind.
        if (direction.x > 0)
            SetDirectionalInput(Vector2.left);
        else
            SetDirectionalInput(Vector2.right);

        // Display hit effect. If attacker is hitting from the right, we flip the effect.
        Instantiate(entityData.hitParticle, transform.position, Quaternion.Euler(0.0f, direction.x < 0 ? 180.0f : 0.0f, Random.Range(0.0f, -90.0f)));

        if (hitStopEnabled)
            HitStop(hitStopDuration);

        // Apply knock back force.
        this.hitDirection = direction;
        this.knockTime = knockTime;

        // Stun
        lastDamageTime = Time.time;
        currentStunResistance--;
        
        if (currentStunResistance <= 0)
        {
            isStunned = true;
        }

        ApplyForce(direction);
        StartCoroutine(DamageCo(knockTime));

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

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth > 0)
        {
            SwitchState(EntityState.stagger);
        }
        else
        {
            SwitchState(EntityState.dead);
            Time.timeScale = 1.0f;
            isDead = true;
        }
    }

    private void TriggerAttack()
    {
        attackState.TriggerAttack();
    }

    private void FinishAttack()
    {
        attackState.FinishAttack();
    }

    /** We don't 'destroy' since it will call the garbage collector = inefficient.
     * Instead, we set the gameObject to 'inactive'.
     */
    public virtual void Die()
    {
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }

    /** Makes enemy gameObject jump by adding force to its y-velocity.
     */
    public virtual void Jump()
    {
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
        }
    }

    // -- OTHER ----------------------------------------------------------------------------------------------------------------


    /** Debug: Draw checks
     */
    public virtual void OnDrawGizmos()
    {
        // Display ground and wall checks.
        Gizmos.color = new Color(1, 1, 1, 1);
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - entityData.groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + entityData.wallCheckDistance * faceDir, wallCheck.position.y));

        // Display melee attack check.
        Gizmos.color = new Color(1, 0, 0, 1);
        Gizmos.DrawWireSphere(targetCheck.position + (Vector3)(transform.right * faceDir * entityData.closeRangeActionDistance), 0.2f);

        // Display player detection check.
        Gizmos.color = new Color(0, 0, 1, 1);
        Gizmos.DrawWireSphere(targetCheck.position + (Vector3)(transform.right * faceDir * entityData.minAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(targetCheck.position + (Vector3)(transform.right * faceDir * entityData.maxAgroDistance), 0.2f);

        // Display player detection Radius check.
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, entityData.radiusAgroDistance);
    }
}
