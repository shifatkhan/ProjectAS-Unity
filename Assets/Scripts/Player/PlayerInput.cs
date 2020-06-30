using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class handles Inputs from the player.
 * 
 * @author ShifatKhan
 * @Special thanks to Sebastian Lague
 */
[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;

    private float lastClickTime; // Used for double clicking.
    private const float DOUBLE_CLICK_TIME = .2f;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        GetPlayerInput();
    }

    void GetPlayerInput()
    {
        // Get player input.
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetButtonDown("Horizontal"))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if(timeSinceLastClick <= DOUBLE_CLICK_TIME)
            {
                // Double clicked
                player.SetSprinting(true);
            }

            lastClickTime = Time.time;
        }
        if (Input.GetButtonUp("Horizontal"))
        {
            player.SetSprinting(false);
        }
        if (Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
        }
        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }

        // TODO: Get player movement input if not attacking. Maybe make player stop moving when attacking
        if (Input.GetButtonDown("Attack1"))
        {
            StartCoroutine(player.Attack1Co());
        }
    }
}
