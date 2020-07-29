using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Hit Stop")]
    private Shader defaultShader;
    private Shader hitShader;
    private bool hitStopped;

    public override void Start()
    {
        base.Start();

        // Hit stop color
        defaultShader = spriteRenderer.material.shader;
        hitShader = Shader.Find("GUI/Text Shader"); // For all white sprite on Hit
    }

    /** Makes current being knocked backwards. Used for when the being is hit.
     * This also calls the HitStop function (if hit stop is enabled).
     */
    public void KnockBack(float damage, Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        // TODO: Change how hit stop is called. What if GameObject is destroyed before Time is set back to normal?
        //      This will freeze the game. Need to call hitstop in hit animation maybe?
        if (hitStopEnabled)
            HitStop(hitStopDuration);

        TakeDamage(damage);
        ApplyForce(direction);
        SetCurrentState(State.stagger);
        StartCoroutine(KnockBackCo(knockTime));
    }

    /** Returns the state to idle after a certain time.
     */
    public IEnumerator KnockBackCo(float knockTime)
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

    public void TakeDamage(float damage)
    {
        currentHealth.RuntimeValue -= damage;
        if(currentHealth.RuntimeValue <= 0)
        {
            animator.SetTrigger("dead");
        }
    }

    /** We don't 'destroy' since it will call the garbage collector = inefficient.
     * Instead, we set the gameObject to 'inactive'.
     */
    public void Die()
    {
        SetCurrentState(State.dead);
        gameObject.SetActive(false);
    }
}
