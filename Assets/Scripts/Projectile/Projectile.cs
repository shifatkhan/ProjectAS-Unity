using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private float speed;
    private float travelDistance;
    private float xStartPos;

    [SerializeField] private float gravity;
    private Rigidbody2D rb;
    private bool isGravityOn;
    private bool hasHitGround; // If it's on the ground, we won't damage player.

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
        rb.velocity = transform.right * speed;

        isGravityOn = false;

        xStartPos = transform.position.x;
    }

    private void Update()
    {
        if (!hasHitGround && isGravityOn)
        {
            // Rotate projectile to simulate falling.
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void FixedUpdate()
    {
        if (!hasHitGround)
        {
            // Check if we have traveled far enough to Drop arrow.
            if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn)
            {
                isGravityOn = true;
                rb.gravityScale = gravity;
            }
        }
        else if(GetComponent<Collider2D>())
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the arrow hit the ground.
        if(other.gameObject.CompareTag("Level"))
        {
            hasHitGround = true;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
        }
    }

    public void FireProjectile(float speed, float travelDistance)
    {
        this.speed = speed;
        this.travelDistance = travelDistance;

        //if(direction < 0)
        //{
        //    transform.localScale = new Vector3(-1, 1, 1);
        //}
    }
}
