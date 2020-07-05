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
    [SerializeField] private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField] private float alphaSet = 0.8f;
    [SerializeField] private float alphaMultiplier = 0.85f;

    private Transform player;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerSpriteRenderer;

    private Color color;

    /** We use OnEnable since we'll be reusing this gameobject.
     */
    private void OnEnable()
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

    private void Update()
    {
        // Reduce alpha value.
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        spriteRenderer.color = color;

        // Add game object back to pool.
        if(Time.time >= (timeActivated + activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
