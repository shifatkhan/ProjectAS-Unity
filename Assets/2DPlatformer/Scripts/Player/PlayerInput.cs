using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class handles Inputs from the player.
 * @Special thanks to Sebastian Lague
 */
[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;

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
