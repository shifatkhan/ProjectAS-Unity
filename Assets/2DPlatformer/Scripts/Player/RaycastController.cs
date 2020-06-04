using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Creates rays to check for collisions.
 * @author ShifatKhan
 * @Special thanks to Sebastian Lague
 */
[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    // TODO: Change visibility of variables.
    [SerializeField] protected LayerMask collisionMask; // Which objects to collide with.

    protected const float skinWidth = .015f;

    // TODO: Make this a CONSTANT
    protected float dstBetweenRays = .1f; // Specify spacing between each rays.

    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D boxCollider2D;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    /** Stick the raycast points to the box collider consistently.
     */
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    /** Calculate where to put the rays along the box collider.
     * Also, calculate where to put rays based on dstBetweenRays (instead of manually
     * inputting number of rays to put).
     */
    public void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);
        
        // Equally space each ray along each axis
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    /** Contains information about raycast points
     *  AKA where to shoot raycasts from to check for collision.
     */
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
