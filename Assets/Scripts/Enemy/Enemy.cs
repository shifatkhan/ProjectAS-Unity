using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

/** This class holds basic data for an Enemy object.
 * @author ShifatKhan
 */
public class Enemy : Movement2D
{
    [Header("Enemy vars")]
    [SerializeField] protected FloatVariable currentHealth;
    [SerializeField] protected string enemyName = "Enemy";
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
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    private bool groundDetected, wallDetected;
    private LayerMask groundLayerMask;

    public override void Start()
    {
        base.Start();

        // Hit
        defaultShader = spriteRenderer.material.shader;
        hitShader = Shader.Find("GUI/Text Shader"); // For all white sprite on Hit

        // AI
        groundLayerMask = controller.GetCollisionMask();
        SwitchState(State.move);
    }

    public override void Update()
    {
        // We don't call base.Update() because we want to update the states ourselves.
        if (currentState != State.dead || currentState != State.stagger)
        {
            switch (currentState)
            {
                case State.move:
                    UpdateMovingState();
                    break;
                case State.stagger:
                    UpdateKnockbackState();
                    break;
                case State.dead:
                    UpdateDeadState();
                    break;
            }
        }
        
        UpdateAnimator();
    }

    //-- MOVING STATE --------------------------------------------------------------------------------

    private void EnterMovingState()
    {
        directionalInput.x = 1;
    }

    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayerMask);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayerMask);

        if (!groundDetected || wallDetected)
        {
            SetDirectionalInput(new Vector2(directionalInput.x * -1, directionalInput.y));
        }
    }

    private void ExitMovingState()
    {

    }

    //-- KNOCKBACK STATE -------------------------------------------------------------------------------

    private void EnterKnockbackState()
    {
        ApplyForce(hitDirection);
        StartCoroutine(DamageCo(knockTime));
    }

    private void UpdateKnockbackState()
    {

    }

    private void ExitKnockbackState()
    {

    }

    //-- DEAD STATE ---------------------------------------------------------------------------------------

    private void EnterDeadState()
    {
        if (enableDeathParticle)
        {
            GameObject chunks = Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
            GameObject blood = Instantiate(deathBloodParticle, transform.position, deathChunkParticle.transform.rotation);

            // Change Direction
            if (hitDirection.x < 0)
            {
                // TODO: Fix flipping. Currently affects y as well.
                chunks.transform.Rotate(new Vector3(180, 0, 0));
                blood.transform.Rotate(new Vector3(180, 0, 0));
            }

            // Change color
            MainModule mainChunk = chunks.GetComponent<ParticleSystem>().main;
            mainChunk.startColor = deathChunkColor;

            MainModule mainBlood = blood.GetComponent<ParticleSystem>().main;
            mainBlood.startColor = deathBloodColor;
        }

        if (enableDeathAnimation)
        {
            animator.SetTrigger("dead");
        }
        else
        {
            Time.timeScale = 1.0f;
            gameObject.SetActive(false);
        }
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    //-- OTHER FUNCTIONS --------------------------------------------------------------------------------

    public override void SwitchState(State newState)
    {
        switch (currentState)
        {
            case State.move:
                ExitMovingState();
                break;
            case State.stagger:
                ExitKnockbackState();
                break;
            case State.dead:
                ExitDeadState();
                break;
        }

        switch (newState)
        {
            case State.move:
                EnterMovingState();
                break;
            case State.stagger:
                EnterKnockbackState();
                break;
            case State.dead:
                EnterDeadState();
                break;
        }

        currentState = newState;
    }

    /** Makes current being knocked backwards. Used for when the being is hit.
     * This also calls the HitStop function (if hit stop is enabled).
     */
    public void Damage(float damage, Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        // TODO: Change how hit stop is called. What if GameObject is destroyed before Time is set back to normal?
        //      This will freeze the game. Need to call hitstop in hit animation maybe?

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
        SwitchState(State.move);
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
            SwitchState(State.stagger);
        }
        else
        {
            SwitchState(State.dead);
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
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
