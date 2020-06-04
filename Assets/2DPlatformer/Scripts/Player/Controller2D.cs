using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class that handles player collisions and interactions.
 * @author ShifatKhan
 * @Special thanks to Sebastian Lague
 */
public class Controller2D : RaycastController
{
    // TODO: Change visibility of variables.

    float maxClimbAngle = 80; // Up to which angle player can climb slope.
    float maxDescendAngle = 75;
    
    [HideInInspector]
    public CollisionInfo collisions;

    public bool showRaycast = true; // Enable/Disable raycast debugging

    [HideInInspector]
    public Vector2 playerInput; // Stores player input.

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
    }

    /** Overloading Move method that does not need input.
     */
    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    /** Move the Player by given moveAmount.
     */
    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();

        collisions.Reset();
        collisions.moveAmountOld = moveAmount;
        playerInput = input;

        // Update direction player is facing.
        if (moveAmount.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }
        
        HorizontalCollisions(ref moveAmount);

        // Update collision checks ONLY if player is actually moving in Y-axis.
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        // Allow jumping if player is on moving platform.
        if(standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir; // left = -1, right = +1
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        // Allows us to detect the wall even if we are not moving (hugging a wall)
        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            // CREATE RAYS
            // If we're moving left, start ray from Bottom left. If moving right, start from Bottom right.
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            // Check if any collisions were found.
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            // Debug: draw collision rays.
            if (showRaycast) { Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red); }

            if (hit)
            {
                // If platform is squashing player, we skip checking current ray.
                if (hit.distance == 0)
                {
                    continue;
                }

                // CLIMBING SLOPES
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                // Climb slope if angle is low enough.
                if(i == 0 && slopeAngle <= maxClimbAngle)
                {
                    // Fix issue for when we come in comtact with 2 descending slopes (V shaped)
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }

                    // Fix issue for when player climbs a slope by hovering over it.
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) // If climbing a new slope.
                    {
                        // Connect player with slope to avoid hovering.
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref moveAmount, slopeAngle);

                    // Restore player once stopped climbing slope.
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                // Change all raycast distance to be the same length.
                // This is for the scenario where you might be on a ledge -> 1st ray hits ledge,
                // 2nd ray hits ground below ledge (don't want player to stand on ground and go through ledge).
                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    //// Fix for when climbing a new slope WHILE on a slope already.
                    //moveAmount.x = Mathf.Min(Mathf.Abs(moveAmount.x), (hit.distance - skinWidth)) * directionX;
                    //rayLength = Mathf.Min(Mathf.Abs(moveAmount.x) + skinWidth, hit.distance);

                    // Stop jittering when climbing slope and colliding with something on the side.
                    if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    // Set collisions to left or right.
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }

            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y); // down = -1, up = +1
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            // CREATE RAYS
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);

            // Check if any collisions were found.
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            // Debug: draw collision rays.
            if (showRaycast) { Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red); }

            // If the ray hit something, change all raycast distance to be the same length.
            // This is for the scenario where you might be on a ledge -> 1st ray hits ledge,
            // 2nd ray hits ground below ledge (don't want player to stand on ground and go through ledge).
            if (hit)
            {
                // If we want to go through a platform that allows it.
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        // Fix issue where player goes on top of platform even if it shouldn't.
                        continue;
                    }
                    if (hit.collider == collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1 && Input.GetKey(KeyCode.Space)) // If player pressed down on platform+space, we can fall through.
                    {
                        collisions.fallingThroughPlatform = hit.collider;
                        continue;
                    }
                }

                // Reset previous platform we fell through if we hit a ground.
                collisions.fallingThroughPlatform = null;

                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                // Stop jittering when climbing slope and colliding with something above.
                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                // Set collisions to below or above.
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        // Fix for when climbing a new slope WHILE on a slope already.
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            // If player hits new slope, change climb to new slope angle.
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    /** Calculate distance to move player to climb slope.
     */
    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        // Allow jumping even when climbing.
        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    /** Calculate distance to move player to descend slope.
     */
    void DescendSlope(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            // Check if player can descend based on max descend angle (and if it's not flat)
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                /// If player is actually descending down the slope.
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    // If player is in contact with said slope.
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendmoveAmountY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    /** Contains information about whether there's a collision or not.
     */
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle;
        public float slopeAngleOld; // Slope angle in the previous frame.
        public Vector2 moveAmountOld;

        public int faceDir; // Last direction player was facing.

        public Collider2D fallingThroughPlatform;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
