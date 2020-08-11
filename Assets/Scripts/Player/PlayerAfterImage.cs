using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Gets the current instance of the player's sprite so it can be used to 
 * display a copy of it behind the player as after image.
 * @author ShifatKhan
 * @Special thanks Bardent (https://youtu.be/ylsWcc4IP3E)
 */
public class PlayerAfterImage : MonoBehaviour
{
    // Variables for fading effect.
    [SerializeField] protected float activeTime = 0.1f;
    protected float timeActivated;
    protected float alpha;
    [SerializeField] protected float alphaSet = 0.8f;
    [SerializeField] protected float alphaDecay = 10f;

    protected Transform player;

    protected SpriteRenderer spriteRenderer;
    protected SpriteRenderer playerSpriteRenderer;

    protected Color color;

    public PlayerAfterImagePool afterImagePool;

    /** We use OnEnable since we'll be reusing this gameobject.
     */
    public virtual void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        spriteRenderer.sprite = playerSpriteRenderer.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        transform.localScale = player.localScale;

        timeActivated = Time.time;
    }

    public virtual void Update()
    {
        // Reduce alpha value.
        alpha -= alphaDecay * Time.deltaTime;
        color = new Color(1f, 1f, 1f, alpha);
        spriteRenderer.color = color;

        // Add game object back to pool.
        if(Time.time >= (timeActivated + activeTime))
        {
            afterImagePool.AddToPool(gameObject);
        }
    }
}
