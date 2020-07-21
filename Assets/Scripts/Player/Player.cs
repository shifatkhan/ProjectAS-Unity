using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class that handles player physics and stats.
 * @author ShifatKhan
 * @Special thanks to Sebastian Lague
 */
public class Player : Movement2D
{
    [Header("Wall jumping")]
    [SerializeField] private bool wallJumpingEnabled = true; // Enable/Disable the ability to walljump.
    private bool wallSliding;
    private int wallDirX;

    private Vector2 wallJumpClimb = new Vector2(7.5f, 16); // For climbing a wall (small jumps)
    private Vector2 wallJumpOff = new Vector2(8.5f, 7); // For getting off a wall (small jump off the wall to the ground)
    private Vector2 wallLeap = new Vector2(18, 17); // For jumping from wall to wall (big/long jump)

    private float wallSlideSpeedMax = 3; // Velocity at which we will descend a wall slide.
    private float wallStickTime = .1f; // Time after which player gets off the wall when no jump inputs were given (instead just getting off)
    private float timeToWallUnstick;
    
    [Header("Particle effects")]
    private float walkSpeed = 6;
    private float sprintSpeed = 10;
    private bool isSprinting = false;

    [Header("Player vars")]
    [SerializeField] private float attackSpeed = 0.5f;
    
    [SerializeField] protected FloatVariable currentHealth;
    [SerializeField] protected GameEvent playerHealthEvent;

    [Header("Particle effects")]
    public ParticleSystem dust;
    public float distanceBetweenAfterImages;
    private float lastAfterImageXPos;
    
    [Header("Inventory system")]
    public InventoryObject inventory; // TODO: Serialize
    [SerializeField] protected GameEvent inventoryEvent;

    public override void Start()
    {
        base.Start();

        dust = GetComponentInChildren<ParticleSystem>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (wallJumpingEnabled)
        {
            HandleWallSliding();
        }
    }

    public override void Update()
    {
        base.Update();
        CheckSprint();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: Add button to pickup item.
        // Check if other gameobject is an item.
        Item item = other.GetComponent<Item>();
        if (item && inventory)
        {
            inventory.AddItem(item.item, item.amount);
            Destroy(other.gameObject);

            inventoryEvent.Raise();
        }
    }

    private void OnApplicationQuit()
    {
        // TODO: Might remove this for the actual build.

        // Clear and delete the inventory.
        if(inventory)
            inventory.itemsContainer.Clear();
    }

    /** Takes in player input and assigns it.
    */
    public void SetDirectionalInput(Vector2 input)
    {
        // Create dust when switching run direction.
        // Don't create dust if player comes to a halt.
        if (input.x != directionalInput.x && controller.collisions.below && input.x != 0)
        {
            CreateDust();
        }

        directionalInput = input;
    }

    /** Set sprinting state.
     */
    public void SetSprinting(bool isSprinting)
    {
        if (isSprinting)
        {
            if (controller.collisions.below)
            {
                this.isSprinting = isSprinting;
            }
        }
        else
        {
            this.isSprinting = isSprinting;
        }
    }

    /** Update player's speed based on sprint state. 
     */
    private void CheckSprint()
    {
        if (isSprinting)
        {
            moveSpeed = sprintSpeed;

            CreateAfterImage();
            if(controller.collisions.below)
                CreateDust();
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }

    public bool CanJump()
    {
        return controller.collisions.below || wallSliding;
    }

    /** Handle player jump velocity (takes into account for all 3 types of wall jumps)
     */
    public void OnJumpInputDown()
    {
        animator.SetTrigger("jump");
        
        if (wallJumpingEnabled && wallSliding)
        {
            if (wallDirX == directionalInput.x) // For wall climbing (moving at same direction as where the wall is)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0) // For jumping off the wall to the ground.
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else // For leaping from the wall with a big jump.
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
            CreateDust();
            AudioManager.PlayJumpAudio();
        }
        if (controller.collisions.below)
        {
            if (directionalInput.y != -1) // If we're not falling through platform.
            {
                velocity.y = maxJumpVelocity;
                CreateDust();
                AudioManager.PlayJumpAudio();
            }
        }
    }

    /** Handle player velocity when jump is released.
     */
    public void OnJumpInputUp()
    {
        // If we let go off jump button, we want to fall quicker (with min jump velocity)
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    /** Check wallsliding state and apply jumping physics to player.
     */
    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;

        wallSliding = false;
        // We can wall-slide if player is pressing left or right. 
        if (velocity.x != 0 && (controller.collisions.left || controller.collisions.right) 
            && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            CreateDust();

            // Slowdown descent speed.
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            // Time after which player gets off the wall 
            // when no jump inputs were given (instead just getting off)
            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    public IEnumerator Attack1Co()
    {
        if(currentState != State.stagger && currentState != State.attack)
        {
            animator.SetTrigger("attack1");
            SetCurrentState(State.attack);

            yield return new WaitForSeconds(attackSpeed);

            SetCurrentState(State.idle);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthEvent.Raise();
        AudioManager.PlayHurtAudio();

        if (currentHealth.RuntimeValue <= 0)
        {
            animator.SetTrigger("dead");

            // TODO: Remove this and call Die() from animation "death"
            AudioManager.PlayDeathAudio();
            gameObject.SetActive(false);
        }
    }

     /** We don't 'destroy' since it will call the garbage collector = inefficient.
     * Instead, we set the gameObject to 'inactive'.
     */
    public void Die()
    {
        SetCurrentState(State.dead);
        AudioManager.PlayDeathAudio();
        gameObject.SetActive(false);
    }

    public override void UpdateState()
    {
        if (currentState != State.attack)
        {
            base.UpdateState();
        }
    }
    
    /** This is mainly used by Animation Events since we can't call other game object's
     * functions through the animation event.
     */
    public void PlayFootstepAudio()
    {
        AudioManager.PlayFootstepAudio();
    }

    private void CreateAfterImage()
    {
        if (Mathf.Abs(transform.position.x - lastAfterImageXPos) > distanceBetweenAfterImages)
        {
            PlayerAfterImagePool.Instance.GetFromPool(directionalInput.x < 0);
            lastAfterImageXPos = transform.position.x;
        }
    }

    public void CreateDust()
    {
        if(dust != null)
            dust.Play();
    }

    public override void UpdateAnimator()
    {
        base.UpdateAnimator();
        if(animator != null)
        {
            animator.SetBool("isWallsliding", wallSliding);

            animator.SetBool("isSprinting", isSprinting);
        }
    }
}
