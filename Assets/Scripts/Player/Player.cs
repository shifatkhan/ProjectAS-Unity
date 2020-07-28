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

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float timeToReachDashVelocity = 1f;
    private float dashDuration = 0.2f;
    private bool isDashing;

    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 6;
    [SerializeField] private float sprintSpeed = 10;
    private bool isSprinting = false;

    [Header("Player vars")]
    [SerializeField] private float attackSpeed = 0.5f;
    
    [SerializeField] protected FloatVariable currentHealth;
    [SerializeField] protected GameEvent playerHealthEvent;

    [Header("Coyote Jump")]
    public float coyoteTime = 0.2f; // Variable for coyote jumping (when falling off ledge)
    private float coyoteCounter;
    private bool jumping = false; // Fixes double jumping because of coyote time.

    [Header("iFrame vars")]
    [SerializeField] private float invulnerableDuration = 1f;
    [SerializeField] private float flashPeriod = 0.16f; // Rate at which player sprite will flash.
    private Color invulnerableColor;
    private Color regularColor;
    private bool invulnerable = false;

    [Header("Particle effects")]
    public ParticleSystem dust;
    public float distanceBetweenAfterImages;
    private float lastAfterImageXPos;
    private float lastAfterImageYPos;
    
    [Header("Inventory system")]
    public InventoryObject inventory; // TODO: Serialize
    [SerializeField] protected GameEvent inventoryEvent;

    public override void Start()
    {
        base.Start();

        dust = GetComponentInChildren<ParticleSystem>();

        // iFrame color
        regularColor = Color.white;
        invulnerableColor = Color.white;
        invulnerableColor.a = 0.5f;
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
        UpdateCoyoteTime();
    }

    private void OnApplicationQuit()
    {
        // TODO: Remove this for the actual build.

        // Clear and delete the inventory.
        if(inventory)
            inventory.itemsContainer.Clear();
    }

    /** Takes in player input and assigns it.
    */
    public override void SetDirectionalInput(Vector2 input)
    {
        // Create dust when switching run direction.
        // Don't create dust if player comes to a halt.
        if (input.x != directionalInput.x && controller.collisions.below && input.x != 0)
        {
            CreateDust();
        }

        base.SetDirectionalInput(input);
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

    /** Updates Coyote jump timers.
     */
    private void UpdateCoyoteTime()
    {
        if (controller.collisions.below)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }
    
    /** Check whether player can Coyote jump or not.
     */
    public bool CanCoyoteJump()
    {
        return coyoteCounter > 0;
    }

    /** Check if player is wall-sliding or grounded sto allow jumping.
     */
    public bool IsWallSliding()
    {
        return wallSliding;
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
            if (!controller.canFallThrough) // If we're not falling through platform.
            {
                velocity.y = maxJumpVelocity;
                CreateDust();
                AudioManager.PlayJumpAudio();

                jumping = true;
            }
        }
        else if (CanCoyoteJump() && !jumping && !controller.canFallThrough)
        {
            velocity.y = maxJumpVelocity;
            CreateDust();
            AudioManager.PlayJumpAudio();

            jumping = true;
        }

        // Hot fix for when we TAP jump before landing and it makes a full jump instead of a hop.
        if (!Input.GetButton("Jump"))
        {
            OnJumpInputUp();
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

    /** Execute dash movement on the player towards the direction we are facing.
     * If no directional input is pressed OnDash, we dash towards the direction we
     * are currently facing.
     */
    public void OnDashInputDown()
    {
        if (!isDashing)
        {
            //if you're not holding any direction while dashing, default to facing direction.
            //Ternary operator to avoid dashing in wrong direction if you don't hold left/right
            float xdir = (directionalInput.x == 0 && directionalInput.y == 0) ? faceDir : directionalInput.x;

            //keeps direction so that you can't change direction while dashing
            Vector2 dir = new Vector2(xdir, directionalInput.y);

            //playerSoundController.PlaySound("dash"); TODO: Implement.

            StartCoroutine("Dashing", dir);
        }
    }

    /** Coroutine for dashing. It has a slight build up to achieve max
     * dash velocity. We also disable dashing (with isDashing) so we
     * can't dashing in the middle of dashing.
     */
    IEnumerator Dashing(Vector2 direction)
    {
        float targetX = (dashSpeed * direction.x) / timeToReachDashVelocity;
        float targetY = (dashSpeed * direction.y) / timeToReachDashVelocity;

        // Fix diagonal dashes going too far by multiplying x and y by 0.4f
        // This is an approximate instead of using a circle function
        // (x-h)^2 + (y-k)^2 = r^2
        // To save compute time. (TODO: maybe use circle func, computing might not be big)
        if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0)
        {
            targetX *= 0.7f;
            targetY *= 0.7f;
        }

        isDashing = true;
        float timer = 0; //Timer to check duration of dash

        //PlayDashingAnimation(direction); TODO: Implement

        while (dashDuration > timer) //how long to dash for i.e dashing for 0.1 sec
        {
            CreateAfterImage();
            timer += Time.deltaTime;
            velocity.x = targetX;
            velocity.y = targetY;

            yield return 0; //go to next frame
        }


        yield return new WaitForSeconds(dashCooldown); //cooldown for dashing
        isDashing = false;
    }

    /** Setter for isDashing.
     */
    private void SetIsDashing(bool dashing)
    {
        isDashing = dashing;
    }

    /** TODO: Implement
     */
    //private void PlayDashingAnimation(Vector2 direction)
    //{
    //    string wBall = "";

    //    if (ballCaught)
    //    {
    //        wBall = "_wBall";
    //    }
    //    //Dash Straight up
    //    if (direction.x == 0 && direction.y > 0)
    //    {
    //        animator.Play("dash_up" + wBall);
    //    }

    //    //Dash diagonal up
    //    else if (direction.x != 0 && direction.y > 0)
    //    {
    //        animator.Play("dash_diag_up" + wBall);
    //    }

    //    //Dash diagonal down
    //    else if (direction.x != 0 && direction.y < 0 && !rc.collisions.below)
    //    {
    //        animator.Play("dash_diag_down" + wBall);
    //    }

    //    //Straight down
    //    else if (direction.x == 0 && direction.y < 0 && !rc.collisions.below)
    //    {
    //        animator.Play("dash_down" + wBall);
    //    }

    //    //Dash straight ahead
    //    else if (direction.x != 0 && direction.y == 0)
    //    {
    //        if (rc.collisions.below)
    //        {
    //            animator.Play("dash_ground" + wBall);
    //        }
    //        else
    //        {
    //            animator.Play("dash_air" + wBall);
    //        }
    //    }
    //}

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

    public void PickUpItem(Item item)
    {
        if (inventory)
        {
            inventory.AddItem(item.item, item.amount);

            inventoryEvent.Raise();
        }
    }

    public void SetInvulnerable(bool invulnerable)
    {
        this.invulnerable = invulnerable;
    }

    public void EnableInvulnerability()
    {
        invulnerable = true;
    }

    public void DisableInvulnerability()
    {
        invulnerable = false;
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
        if (!invulnerable)
        {
            StartCoroutine(TakeDamageCo());

            currentHealth.RuntimeValue -= damage;
            playerHealthEvent.Raise();
            AudioManager.PlayHurtAudio();

            if (currentHealth.RuntimeValue <= 0)
            {
                animator.SetTrigger("dead");

                // TODO: Add "death" anim and Remove this and call Die() from animation "death"
                AudioManager.PlayDeathAudio();
                gameObject.SetActive(false);
            }
        }
    }

    /** Flashes the player's sprite and makes it invulnerable for a period of time.
     */
    private IEnumerator TakeDamageCo()
    {
        invulnerable = true;
        
        // Calculate how many times to flash to result in a total duration of "invulnerableDuration"
        float numberOfFlashes = (invulnerableDuration / (flashPeriod * 2));

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = invulnerableColor;
            yield return new WaitForSeconds(flashPeriod);

            spriteRenderer.color = regularColor;
            yield return new WaitForSeconds(flashPeriod);
        }

        invulnerable = false;
    }

    public override void KnockBack(Vector3 direction, float knockTime, bool hitStopEnabled, float hitStopDuration)
    {
        if (!invulnerable)
            base.KnockBack(direction, knockTime, hitStopEnabled, hitStopDuration);
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
    
    /** This is mainly used by Animation Events since we can't call other game object's
     * functions through the animation event.
     */
    public void PlayFootstepAudio()
    {
        AudioManager.PlayFootstepAudio();
    }

    private void CreateAfterImage()
    {
        if (Mathf.Abs(transform.position.x - lastAfterImageXPos) > distanceBetweenAfterImages
            || Mathf.Abs(transform.position.y - lastAfterImageYPos) > distanceBetweenAfterImages)
        {
            PlayerAfterImagePool.Instance.GetFromPool(directionalInput.x < 0);
            lastAfterImageXPos = transform.position.x;
            lastAfterImageYPos = transform.position.y;
        }
    }

    public void CreateDust()
    {
        if(dust != null)
            dust.Play();
    }

    public override void UpdateState()
    {
        if (currentState != State.attack)
        {
            base.UpdateState();
        }

        if (controller.collisions.below)
        {
            jumping = false;
        }
    }

    public override void UpdateAnimator()
    {
        if(currentState != State.attack)
            base.UpdateAnimator();

        if(animator != null)
        {
            animator.SetBool("isWallsliding", wallSliding);

            animator.SetBool("isSprinting", isSprinting);
        }
    }
}
