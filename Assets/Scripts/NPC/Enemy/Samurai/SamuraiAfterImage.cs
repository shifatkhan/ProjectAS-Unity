using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiAfterImage : PlayerAfterImage
{
    public override void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Boss_Samurai").transform;
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        spriteRenderer.sprite = playerSpriteRenderer.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        transform.localScale = player.localScale;

        timeActivated = Time.time;
    }
}
