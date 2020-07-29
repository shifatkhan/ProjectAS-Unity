using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author ShifatKhan
 */
public class KnockBack : MonoBehaviour
{
    public float thrust;
    public float knockTime;
    public float damage;
    public bool hitStopEnabled;
    public float hitStopDuration = 0.1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player"))
        {
            // Get Direction (by normalizing) and multiply the attack with thrust power.
            Vector2 direction = other.transform.position - transform.position;
            direction = direction.normalized * thrust;

            if (other.gameObject.CompareTag("Enemy"))
            {
                other.GetComponent<Enemy>().Damage(damage, direction, knockTime, hitStopEnabled, hitStopDuration); ;
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<Player>().Damage(damage, direction, knockTime, hitStopEnabled, hitStopDuration); ;
            }
        }
    }
}
