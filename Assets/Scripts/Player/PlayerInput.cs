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
    
    private Item itemToPickup = null; // Reference to the item we want to pickup.

    void Start()
    {
        player = GetComponent<Player>();
        inventoryUI = GameObject.FindGameObjectWithTag("Inventory"); ;
    }

    void FixedUpdate()
    {
        // Check jump.
        if (jumpTimer > Time.time
            && (player.IsGrounded() || player.IsWallSliding() || player.CanCoyoteJump()))
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

        // HORIZONTAL MOVEMENTS
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

        // JUMPING
        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }
        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }

        // DASH
        if (Input.GetButtonDown("Dash"))
        {
            player.OnDashInputDown();
        }

        // TODO: Get player movement input if not attacking. Maybe make player stop moving when attacking
        // ATTACKS
        if (Input.GetButtonDown("Attack1"))
        {
            StartCoroutine(player.Attack1Co());
        }

        // TODO: Maybe move this into the InventoryUI script.
        // INVENTORY
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

        // PICK UP ITEMS
        if (Input.GetButtonDown("Interact") && itemToPickup != null)
        {
            player.PickUpItem(itemToPickup);
            Destroy(itemToPickup.gameObject);
            itemToPickup = null;
        }
    }

    /** Get reference of the item in the world if the player gets near it.
     * Then, when the player presses the Interact button, we will pickup the item.
     * 
     * TODO: We can't pickup multiple items that are overlapping.
     *      Issue: Pickup one item, go out of the collision box of the other item, then come back in
     *              to trigger this function.
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if other gameobject is an item.
        itemToPickup = other.GetComponent<Item>();
    }
}
