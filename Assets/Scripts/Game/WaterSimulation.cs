using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSimulation : MonoBehaviour
{
    public LineRenderer Body;

    WavePoint[] wavePoints; // Points found on the surfave of the water.

    /*************** Mesh and Colliders ***************/
    GameObject[] meshobjects; // Game objects for each spring.
    Mesh[] meshes; // Mesh to draw the water springs.
    // TODO: Might not need. Replace with 1 big box collider.
    GameObject[] colliders; // So things can interact with our water.

    /*************** Water coords ***************/
    float baseheight;
    float left;
    float bottom;

    /*************** Water vars & constants ***************/
    const float springconstant = 0.02f; // Spring constant for forces applied by adjacent points
    const float springConstantBaseline = 0.02f; // Sprint constant for force applied to baseline (resting state)
    const float damping = 0.04f; // Damping to apply to speed changes
    const float spread = 0.05f; 
    const float z = -1f; // Change this according to where you want the water to appear.

    // Number of iterations of point-influences-point to do on wave per step
    // (this makes the waves animate faster)
    int iterations = 8;

    private float offset = 0; // A phase difference to apply to each sine
    private float waveSpeed = 0.05f; // The more backgroundWaveCompression gets bigger, the more waveSpeed should get smaller.

    private int numBackgroundWaves = 7; // Randomness of each wave.
    private float backgroundWaveMaxHeight = 0.1f;
    private float backgroundWaveCompression = 3f; // Amounts of waves in a small area (smaller width = more compression)

    private List<float> sineYOffsets = new List<float>(); // Amounts by which a particular sine is offset
    private List<float> sineAmplitudes = new List<float>(); // Amounts by which a particular sine is amplified
    private List<float> sineWavelength = new List<float>(); // Amounts by which a particular sine is stretched
    private List<float> offsetSpeed = new List<float>(); // Amounts by which a particular sine's offset is multiplied

    /*************** Renderer ***************/
    public Material mat; //The material we're using for the top of the water
    public GameObject watermesh; //The GameObject we're using for a mesh
    

    private void Start()
    {
        // Spawn our water
        GenerateRandomWaves();
        InitializeWater(-10, 20, 0, -3);
        Debug.Log($"sineOffset:{sineYOffsets[0]}");
        Debug.Log($"sineAmplitudes:{sineAmplitudes[0]}");
        Debug.Log($"sineStretches:{sineWavelength[0]}");
        Debug.Log($"offsetStretches:{offsetSpeed[0]}");
    }

    private void FixedUpdate()
    {
        offset = offset + waveSpeed;

        // Hooke's law & Euler method to handle all the physics of our springs.
        for (int i = 0; i < wavePoints.Length; i++)
        {
            float force = springconstant * (wavePoints[i].y - baseheight) + wavePoints[i].velocity * damping;
            wavePoints[i].acceleration = -force; // TODO: add mass to nodes with acceleration = -force/mass;. Maybe keep it as mass =1?
            wavePoints[i].y += wavePoints[i].velocity;
            wavePoints[i].velocity += wavePoints[i].acceleration;

            // RANDOM WAVES
            wavePoints[i].y = Mathf.Abs(OverlapSines(wavePoints[i].x));

            // Update line renderer
            Body.SetPosition(i, new Vector3(wavePoints[i].x, wavePoints[i].y, z));
        }

        // Calculate how wave points affects neighbours.

        // Store the difference in heights:
        float[] leftDeltas = new float[wavePoints.Length];
        float[] rightDeltas = new float[wavePoints.Length];

        // Iterations = 8 for fluidity.
        for (int j = 0; j < iterations; j++)
        {
            for (int i = 0; i < wavePoints.Length; i++)
            {
                // Check the heights of the nearby nodes, adjust velocities accordingly, record the height differences
                if (i > 0)
                {
                    leftDeltas[i] = spread * (wavePoints[i].y - wavePoints[i - 1].y);
                    wavePoints[i - 1].velocity += leftDeltas[i];
                }
                if (i < wavePoints.Length - 1)
                {
                    rightDeltas[i] = spread * (wavePoints[i].y - wavePoints[i + 1].y);
                    wavePoints[i + 1].velocity += rightDeltas[i];
                }
            }

            // Apply a difference in position
            for (int i = 0; i < wavePoints.Length; i++)
            {
                if (i > 0)
                    wavePoints[i - 1].y += leftDeltas[i];
                if (i < wavePoints.Length - 1)
                    wavePoints[i + 1].y += rightDeltas[i];
            }
        }
        //Finally we update the meshes to reflect this
        UpdateMeshes();
    }

    /** Set the initial body of water.
     */
    public void InitializeWater(float Left, float Width, float Top, float Bottom)
    {
        //Calculate the number of edges and nodes we have
        int edgecount = Mathf.RoundToInt(Width) * 5;
        int nodecount = edgecount + 1; // 1 extra at the end.

        // Render line body of water.
        Body = gameObject.AddComponent<LineRenderer>();
        Body.material = mat;
        Body.material.renderQueue = 1000;
        Body.positionCount = nodecount;
        Body.startWidth = 0.1f;

        // Initialize variables.
        wavePoints = new WavePoint[nodecount];

        meshobjects = new GameObject[edgecount];
        meshes = new Mesh[edgecount];
        colliders = new GameObject[edgecount];

        baseheight = Top;
        bottom = Bottom;
        left = Left;
        
        // Set line points.
        for (int i = 0; i < nodecount; i++)
        {
            wavePoints[i] = new WavePoint();

            wavePoints[i].x = Left + Width * i / edgecount;
            wavePoints[i].y = Top;
            wavePoints[i].acceleration = 0;
            wavePoints[i].velocity = 0;

            Body.SetPosition(i, new Vector3(wavePoints[i].x, wavePoints[i].y, z));
        }

        // TODO: Review COMP 371
        // Set the meshes.
        for (int i = 0; i < edgecount; i++)
        {
            meshes[i] = new Mesh();

            // Set the corners of the mesh (trapezoid)
            Vector3[] Vertices = new Vector3[4];
            Vertices[0] = new Vector3(wavePoints[i].x, wavePoints[i].y, z); // Top left
            Vertices[1] = new Vector3(wavePoints[i + 1].x, wavePoints[i + 1].y, z); // Top right
            Vertices[2] = new Vector3(wavePoints[i].x, bottom, z); // Bottom left
            Vertices[3] = new Vector3(wavePoints[i + 1].x, bottom, z); // Bottom right

            // Set coords for textures.
            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);
            UVs[1] = new Vector2(1, 1);
            UVs[2] = new Vector2(0, 0);
            UVs[3] = new Vector2(1, 0);

            //Set where the triangles should be (2 triangles, so 6 points)
            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };

            //Add all this data to the mesh.
            meshes[i].vertices = Vertices;
            meshes[i].uv = UVs;
            meshes[i].triangles = tris;

            //Create a holder for the mesh, set it to be the manager's child (to render)
            meshobjects[i] = Instantiate(watermesh, Vector3.zero, Quaternion.identity) as GameObject;
            meshobjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshobjects[i].transform.parent = transform;

            //Create our colliders, set them be our child
            colliders[i] = new GameObject();
            colliders[i].name = "Trigger";
            colliders[i].AddComponent<BoxCollider2D>();
            colliders[i].transform.parent = transform;

            //Set the position and scale to the correct dimensions
            colliders[i].transform.position = new Vector3(Left + Width * (i + 0.5f) / edgecount, Top - 0.5f, 0);
            colliders[i].transform.localScale = new Vector3(Width / edgecount, 1, 1);

            //Add a WaterDetector and make sure they're triggers
            colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
            colliders[i].AddComponent<WaterDetector>();
        }
    }

    /** Update and set the new mesh positions.
     */
    void UpdateMeshes()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            Vector3[] Vertices = new Vector3[4];

            Vertices[0] = new Vector3(wavePoints[i].x, wavePoints[i].y, z);
            Vertices[1] = new Vector3(wavePoints[i + 1].x, wavePoints[i + 1].y, z);
            Vertices[2] = new Vector3(wavePoints[i].x, bottom, z);
            Vertices[3] = new Vector3(wavePoints[i + 1].x, bottom, z);

            meshes[i].vertices = Vertices;
        }
        
        // Update line renderer
        //Body.SetPositions(ConvertSpringPointsToVector3());
    }

    /** Set each sine's values to a reasonable random value
     */
    private void GenerateRandomWaves()
    {
        for (int i = -0; i < numBackgroundWaves; i++)
        {
            //sineYOffsets.Add(-Mathf.PI + 2 * Mathf.PI * Random.value);
            sineYOffsets.Add(Random.Range(-0.1f, 0.1f));

            sineAmplitudes.Add(Random.value * backgroundWaveMaxHeight);

            sineWavelength.Add(Random.value * backgroundWaveCompression);

            offsetSpeed.Add(Random.value * backgroundWaveCompression);
        }
    }

    /** Make a splash on waves at a certain position.
     */
    public void Splash(float xpos, float velocity)
    {
        //If the position is within the bounds of the water:
        if (xpos >= wavePoints[0].x && xpos <= wavePoints[wavePoints.Length - 1].x)
        {
            //Offset the x position to be the distance from the left side
            xpos -= wavePoints[0].x;

            //Find which spring we're touching
            int index = Mathf.RoundToInt((wavePoints.Length - 1) * (xpos / (wavePoints[wavePoints.Length - 1].x - wavePoints[0].x)));

            //Add the velocity of the falling object to the spring
            wavePoints[index].velocity += velocity;

            // TODO: Add splash effect?
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
                sineYOffsets[i] +
                sineAmplitudes[i] *
                Mathf.Sin(x * sineWavelength[i] + offset * offsetSpeed[i]);
        }

        return result;
    }

    /** Converts wavePoints to vector3 array for drawing the linerenderer.
     */
    private Vector3[] ConvertSpringPointsToVector3()
    {
        Vector3[] wavePointsCoords = new Vector3[wavePoints.Length];

        for (int n = 0; n < wavePointsCoords.Length; n++)
        {
            wavePointsCoords[n].x = wavePoints[n].x;
            wavePointsCoords[n].y = wavePoints[n].y + OverlapSines(wavePoints[n].x);
        }

        return wavePointsCoords;
    }

    void OnTriggerStay2D(Collider2D Hit)
    {
        // TODO: Fill code here for making things float in the water.
        // Include a buoyancy constant unique to each object?
    }

    /** Holds information about a point on the water's surface (wave)
     */
    public class WavePoint
    {
        public float x;
        public float y;
        public float velocity;
        public float acceleration;

        public WavePoint()
        {
            x = 0;
            y = 0;
            velocity = 0;
            acceleration = 0;
        }
    }
}
