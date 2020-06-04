using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class holds basic data for an Enemy object.
 * @author ShifatKhan
 */
public class Enemy : Movement2D
{
    [SerializeField] protected float health = 3;
    [SerializeField] protected FloatVariable maxHealth;
    [SerializeField] protected string enemyName = "Enemy";
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float attackSpeed = 0f;

    public override void Start()
    {
        base.Start();
        health = maxHealth.InitialValue;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
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
