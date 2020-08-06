using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : KnockBack
{
    public float lastTouchDamageTime;
    public float touchDamageCooldown;

    private void Start()
    {
        lastTouchDamageTime = Time.time;
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time >= lastTouchDamageTime + touchDamageCooldown)
        {
            lastTouchDamageTime = Time.time;
            base.OnTriggerEnter2D(other);
        }
    }
}
