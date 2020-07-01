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
public class Water : MonoBehaviour
{
    public LineRenderer lineRenderer;

    /********************* WATER CONTAINER COORDS *********************/
    [DraggablePoint] public Vector3 topLeft;
    [DraggablePoint] public Vector3 topRight;

    /************************ WAVE POINTS VARS ************************/
    public int numPoints = 80; // Resolution of simulation
    private float width = 600; // Width of simulation
    private float springConstant = 0.005f; // Spring constant for forces applied by adjacent points
    private float springConstantBaseline = 0.005f; // Sprint constant for force applied to baseline
    private float yOffset = 200; // Vertical draw offset of simulation
    private float damping = 0.99f; // Damping to apply to speed changes

    // Number of iterations of point-influences-point to do on wave per step
    // (this makes the waves animate faster)
    private int iterations = 5;

    private List<SpringPoint> wavePoints = new List<SpringPoint>(); // Points found on the surfave of the water.

    /************************* WAVE FLOW VARS *************************/
    private float offset = 0; // A phase difference to apply to each sine

    private int numBackgroundWaves = 7;
    private float backgroundWaveMaxHeight = 6;
    private float backgrounWaveCompression = 0.1f;

    private List<float> sineOffsets = new List<float>(); // Amounts by which a particular sine is offset
    private List<float> sineAmplitudes = new List<float>(); // Amounts by which a particular sine is amplified
    private List<float> sineStretches = new List<float>(); // Amounts by which a particular sine is stretched
    private List<float> offsetStretches = new List<float>(); // Amounts by which a particular sine's offset is multiplied

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        //width = Vector3.Distance(topLeft, topRight);
        //yOffset = 5;

        // Get initial wave values.
        GenerateRandomWaves();
        wavePoints = MakeWavePoints(numPoints);

        // Set linerenderer values.
        lineRenderer.positionCount = wavePoints.Count;
        lineRenderer.SetPositions(ConvertSpringPointsToVector3());
    }

    private void Update()
    {
//#if UNITY_EDITOR
//        // Make sure the vectors are leveled.
//        // This is only used in the Editor (when placing the water).
//        // We won't be changing this in game.
//        if (topLeft.y != transform.position.y)
//            topLeft.y = transform.position.y;
//        if (topRight.y != transform.position.y)
//            topRight.y = transform.position.y;

//        //lineRenderer.SetPositions(new Vector3[] { topLeft, topRight });
//#endif

        offset = offset + 1;

        // Update positions of points.
        UpdateWavePoints(Time.deltaTime);
        DrawSurface();
    }

    /** Draw wave.
     */
    private void DrawSurface()
    {
        //for (int n = 0; n < wavePoints.Count; n++)
        //{
        //    if (n != 0)
        //    {
        //        SpringPoint leftPoint = wavePoints[n - 1];
        //    }
        //}
        lineRenderer.SetPositions(ConvertSpringPointsToVector3());
    }

    /** Converts wavePoints to vector3 array for drawing the linerenderer.
     */
    private Vector3[] ConvertSpringPointsToVector3()
    {
        Vector3[] wavePointsCoords = new Vector3[wavePoints.Count];

        for (int n = 0; n < wavePointsCoords.Length; n++)
        {
            if (n != 0)
            {
                wavePointsCoords[n - 1].x = wavePoints[n - 1].x;
                wavePointsCoords[n - 1].y = wavePoints[n - 1].y + OverlapSines(wavePoints[n - 1].x);
            }
        }

        return wavePointsCoords;
    }

    /** Debug: Draw wave's spring.
     */
    void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0, 0, 1, .5f);

        //Debug.Log($"WAVES: {wavePoints.Count}");
        //for (int n = 0; n < wavePoints.Count; n++)
        //{
        //    Gizmos.DrawSphere(new Vector3(wavePoints[n].x, yOffset + OverlapSines(wavePoints[n].x)), 0.25f);
        //}
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
                x = ((float) n / numPoints) * width,
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

            sineStretches.Add(Random.value * backgrounWaveCompression);

            offsetStretches.Add(Random.value * backgrounWaveCompression);
        }
    }

    /** Converts trig values (sine, cos) to ints.
     */
    private float TrigToNum(float num)
    {
        return Mathf.Round(num * 1000) / 1000;
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
                float forceFromLeft, forceFromRight;

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

                // Also apply force toward the baseline.
                float dy2 = yOffset - wavePoints[n].y;
                float forcetoBaseline = springConstantBaseline * dy2;

                // Sum up forces.
                force += forceFromLeft;
                force += forceFromRight;
                force += forcetoBaseline;

                float acceleration = force / wavePoints[n].mass;

                // Apply acceleration (with damping)
                wavePoints[n].speed.y = damping * wavePoints[n].speed.y + acceleration;

                // Apply speed
                wavePoints[n].y = wavePoints[n].y + wavePoints[n].speed.y;
            }
        }
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
