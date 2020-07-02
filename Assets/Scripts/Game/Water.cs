using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class that takes care of creating water physics and interactions.
 * Class is made executable in Edit mode so we can see if the water functions correctly.
 * It also allows us to level the water's surface.
 * 
 * @author ShifatKhan
 * @Special thanks to Michael Hoffman & Anko 
 * (https://gamedevelopment.tutsplus.com/tutorials/make-a-splash-with-dynamic-2d-water-effects--gamedev-236)
 */
//[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Water : MonoBehaviour
{
    public LineRenderer lineRenderer;

    /********************* WATER CONTAINER COORDS *********************/
    private BoxCollider2D boxCollider2D;
    private Vector3 topLeft;
    private Vector3 topRight;
    private Vector3 btmLeft;
    private Vector3 btmRight;

    /************************ WAVE POINTS VARS ************************/
    public int numPoints = 80; // Resolution of simulation
    public float width = 200; // Width of simulation
    private float yOffset = 200; // Vertical draw offset of simulation

    public float springConstant = 0.005f; // Spring constant for forces applied by adjacent points
    public float springConstantBaseline = 0.005f; // Sprint constant for force applied to baseline (resting state)
    public float damping = 0.99f; // Damping to apply to speed changes

    // Number of iterations of point-influences-point to do on wave per step
    // (this makes the waves animate faster)
    public int iterations = 5;

    private List<SpringPoint> wavePoints = new List<SpringPoint>(); // Points found on the surfave of the water.

    /************************* WAVE FLOW VARS *************************/
    private float offset = 0; // A phase difference to apply to each sine

    public int numBackgroundWaves = 7; // Randomness of each wave.
    public float backgroundWaveMaxHeight = 6;
    public float backgroundWaveCompression = 0.1f; // Amounts of waves in a small area (smaller width = more compression)

    private List<float> sineOffsets = new List<float>(); // Amounts by which a particular sine is offset
    private List<float> sineAmplitudes = new List<float>(); // Amounts by which a particular sine is amplified
    private List<float> sineStretches = new List<float>(); // Amounts by which a particular sine is stretched
    private List<float> offsetStretches = new List<float>(); // Amounts by which a particular sine's offset is multiplied

    Vector3 worldPosition = Vector3.zero;

    private void Start()
    {
        // Get Components.
        lineRenderer = GetComponent<LineRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        // Get Box collider coords
        // Special thanks to Alec-Slayden: http://answers.unity.com/answers/860223/view.html
        float top = boxCollider2D.offset.y + (boxCollider2D.size.y / 2f);
        float btm = boxCollider2D.offset.y - (boxCollider2D.size.y / 2f);
        float left = boxCollider2D.offset.x - (boxCollider2D.size.x / 2f);
        float right = boxCollider2D.offset.x + (boxCollider2D.size.x / 2f);

        topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        // Get initial wave values.
        //width = Vector3.Distance(topLeft, topRight);
        yOffset = Vector3.Distance(topLeft, btmLeft);

        GenerateRandomWaves();
        wavePoints = MakeWavePoints(numPoints);

        // Set line renderer values.
        lineRenderer.positionCount = wavePoints.Count;
        lineRenderer.SetPositions(ConvertSpringPointsToVector3());
    }

    private void Update()
    {
        // TODO: Remove mouse click. It's used to test the wave splash.
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
            worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

            float startX = transform.position.x;
            float endX = transform.position.x + width;
            float lineLength = endX - startX;

            float closestDistance = -1;
            int closestPoint = 0;
            for (int n = 0; n < wavePoints.Count; n++)
            {
                float distance = Mathf.Abs(worldPosition.x - wavePoints[n].x);
                if(closestDistance == -1)
                {
                    closestPoint = n;
                    closestDistance = distance;
                }
                else if (distance <= closestDistance)
                {
                    closestPoint = n;
                    closestDistance = distance;
                }
            }
            wavePoints[closestPoint].y = worldPosition.y;
        }

        // ------------------------
        offset = offset + 1;

        // Update positions of points.
        UpdateWavePoints(Time.deltaTime);

        // Update line renderer
        lineRenderer.SetPositions(ConvertSpringPointsToVector3());
    }

    /** Update the positions of each wave point.
     */
    private void UpdateWavePoints(float deltaTime)
    {
        for (int i = 0; i < iterations; i++)
        {
            for (int n = 0; n < wavePoints.Count; n++)
            {
                // Force to apply to this point.
                float force = 0;

                // Forces caused by the point immediately to the left or the right.
                float forceFromLeft = 0, forceFromRight= 0;
                
                /** TODO: Remove if not needed. 
                 * Commented Code below if for wrapping the force around (unrealistic)
                if (n == 0) // Wrap left-to-right
                {
                    float dy = wavePoints[wavePoints.Count - 1].y - wavePoints[n].y;
                    forceFromLeft = springConstant * dy;
                }
                else // Normally
                {
                    float dy = wavePoints[n - 1].y - wavePoints[n].y;
                    forceFromLeft = springConstant * dy;
                }

                if (n == wavePoints.Count - 1) // Wrap right-to-left
                {
                    float dy = wavePoints[0].y - wavePoints[n].y;
                    forceFromRight = springConstant * dy;
                }
                else // Normally
                {
                    float dy = wavePoints[n + 1].y - wavePoints[n].y;
                    forceFromRight = springConstant * dy;
                }
                */

                if (n != 0)
                {
                    float dy = wavePoints[n - 1].y - wavePoints[n].y;
                    forceFromLeft = springConstant * dy;
                }
                
                if(n != wavePoints.Count - 1)
                {
                    float dy = wavePoints[n + 1].y - wavePoints[n].y;
                    forceFromRight = springConstant * dy;
                }

                // Also apply force toward the baseline.
                float dy2 = yOffset - wavePoints[n].y;
                float forcetoBaseline = springConstantBaseline * dy2;

                // Sum up forces.
                force = force + forceFromLeft;
                force = force + forceFromRight;
                force = force +forcetoBaseline;

                // Calculate acceleration
                float acceleration = force / wavePoints[n].mass;

                // Apply acceleration (with damping)
                wavePoints[n].speed.y = damping * wavePoints[n].speed.y + acceleration;

                // Apply speed
                wavePoints[n].y = wavePoints[n].y + wavePoints[n].speed.y;
            }
        }
    }

    /** Converts wavePoints to vector3 array for drawing the linerenderer.
     */
    private Vector3[] ConvertSpringPointsToVector3()
    {
        Vector3[] wavePointsCoords = new Vector3[wavePoints.Count];

        for (int n = 0; n < wavePointsCoords.Length; n++)
        {
            wavePointsCoords[n].x = wavePoints[n].x;
            wavePointsCoords[n ].y = wavePoints[n].y + OverlapSines(wavePoints[n].x);
        }

        return wavePointsCoords;
    }

    /** Create points to be used on the water's surface.
     */
    private List<SpringPoint> MakeWavePoints(int numPoints)
    {
        List<SpringPoint> springs = new List<SpringPoint>();

        for (int n = 0; n < numPoints; n++)
        {
            // This represents a point on the wave
            SpringPoint newPoint = new SpringPoint()
            {
                x = (((float) n / numPoints) * width) + transform.position.x,
                y = yOffset,
                speed = Vector2.zero,
                mass = 1
            };
            springs.Add(newPoint);
        }

        return springs;
    }

    /** Set each sine's values to a reasonable random value
     */
    private void GenerateRandomWaves()
    {
        for (int i = -0; i < numBackgroundWaves; i++)
        {
            sineOffsets.Add(-Mathf.PI + 2 * Mathf.PI * Random.value);

            sineAmplitudes.Add(Random.value * backgroundWaveMaxHeight);

            sineStretches.Add(Random.value * backgroundWaveCompression);

            offsetStretches.Add(Random.value * backgroundWaveCompression);
        }
    }

    /** This function sums together the sines generated, given an input value x
     */
    private float OverlapSines(float x)
    {
        float result = 0;

        for (int i = 0; i < numBackgroundWaves; i++)
        {
            result = result +
                sineOffsets[i] +
                sineAmplitudes[i] *
                Mathf.Sin(x * sineStretches[i] + offset * offsetStretches[i]);
        }

        return result;
    }

    /** Holds information about a point on the water's surface (wave)
     */
    public class SpringPoint
    {
        public float x;
        public float y;
        public float mass;
        public Vector2 speed;
    }
}
