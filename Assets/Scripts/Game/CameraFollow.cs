using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Camera class that follows the player around.
 * @Special thanks to Sebastian Lague
 */
public class CameraFollow : MonoBehaviour
{
    public Controller2D target;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    void Start()
    {
        if (target == null)
        {
            GameObject follow = GameObject.FindWithTag("Player");
            if (follow == null)
            {
                Debug.LogError("CameraFollow: No Player found in current Scene.", follow);
            }
            else
            {
                target = follow.GetComponent<Controller2D>();
            }
        }

        focusArea = new FocusArea(target.boxCollider2D.bounds, focusAreaSize);
    }

    void LateUpdate()
    {
        focusArea.Update(target.boxCollider2D.bounds);

        Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0)
        {
            // Make camera move a bit more in the direction player is moving.
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    // Stop camera moving ahead if we stop moving.
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
        }


        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10; // Camera is in front of level.
    }

    /** Debug: Draw camera's focus area
     */
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, .2f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }

    /** Contains information about the view box, camera bounds and target.
     */
    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity; // How far camera moved.
        float left, right;
        float top, bottom;


        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            // Check whether to move in X-axis
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            // Check whether to move in Y-axis
            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

}
