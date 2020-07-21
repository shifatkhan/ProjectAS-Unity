using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** This class handles Inputs from the player.
 * 
 * @author ShifatKhan
 * @Special thanks to Sebastian Lague
 */
[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;

    // INVENTORY
    private GameObject inventoryUI;

    // DOUBLE CLICK
    private float lastClickTime; // Used for double clicking.
    private const float DOUBLE_CLICK_TIME = .2f;

    [Header("Jump buffer")]
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    void Start()
    {
        player = GetComponent<Player>();
        inventoryUI = GameObject.FindGameObjectWithTag("Inventory"); ;
    }

    void FixedUpdate()
    {
        // Check jump.
        if (jumpTimer > Time.time && player.CanJump())
        {
            player.OnJumpInputDown();
            
            jumpTimer = 0;
        }
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
            jumpTimer = Time.time + jumpDelay;
            //player.OnJumpInputDown();
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

        // TODO: Maybe move this into the InventoryUI script.
        // Open/Close Inventory UI
        if (Input.GetButtonDown("Inventory"))
        {
            if (inventoryUI.GetComponent<Image>().color.a == 1)
            {
                Color newColor = inventoryUI.GetComponent<Image>().color;
                newColor.a = 0;
                inventoryUI.GetComponent<Image>().color = newColor;

                inventoryUI.transform.Find("ItemSlotsContainer").gameObject.SetActive(false);
            }
            else
            {
                Color newColor = inventoryUI.GetComponent<Image>().color;
                newColor.a = 1;
                inventoryUI.GetComponent<Image>().color = newColor;

                inventoryUI.transform.Find("ItemSlotsContainer").gameObject.SetActive(true);
            }
        }
    }
}
