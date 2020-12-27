using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidManager : MonoBehaviour {

    [Tooltip("The prefab to use as a splash particle effect.")]
    public Particle splashParticlePrefab;

    [Tooltip("Material for rendering the surface of the liquid.")]
    public Material surfaceMaterial;

    [Tooltip("Defines the width of the surface of the water.")]
    public float surfaceWidth = 0.1f;

    [Tooltip("Object defining the mesh for the body of liquid.")]
    public GameObject bodyMesh;

    [Tooltip("Object defining the bounds of the body of liquid.")]
    public SpriteRenderer boundsObject;

    [Tooltip("Determines how many edges make up the surface of the water.")]
    public int numEdges = 1;

    [Tooltip("Determines how strong the surface of the liquid behaves.")]
    public float springConstant = 0.05f;

    [Tooltip("Determines how fast the surface of the liquid settles back to normal.")]
    public float damping = 0.05f;

    [Tooltip("Determines how much a wave spreads across the surface of liquid.")]
    public float spread = 0.05f;

    private int numNodes;
    private float baseHeight;
    private float bottom;
    private float left;
    private float width;
    private float z;

    private float[] x_positions;
    private float[] y_positions;
    private float[] y_vels;

    private LineRenderer surfaceRenderer;
    private GameObject[] meshObjs;
    private Mesh[] meshes;

    void Start() {
        Debug.Assert(numEdges > 0, "numEdges must be greater than 0");
        numNodes = numEdges + 1;

        z = this.transform.position.z;

        // Get the bounds of the body of liquid
        Bounds bounds = boundsObject.bounds;
        baseHeight = bounds.max.y;
        bottom = bounds.min.y;
        left = bounds.min.x;
        width = bounds.size.x;

        // Initialize LineRenderer for drawing the surface of the water
        surfaceRenderer = this.gameObject.AddComponent<LineRenderer>();
        surfaceRenderer.material = surfaceMaterial;
        surfaceRenderer.material.renderQueue = 1000;
        surfaceRenderer.positionCount = numNodes;
        surfaceRenderer.startWidth = surfaceWidth;
        surfaceRenderer.endWidth = surfaceWidth;

        x_positions = new float[numNodes];
        y_positions = new float[numNodes];
        y_vels = new float[numNodes];

        // Set positions of the surface nodes
        for (int i = 0; i < numNodes; i++) {
            x_positions[i] = left + i * (width / numEdges);
            y_positions[i] = baseHeight;
            surfaceRenderer.SetPosition(i, new Vector3(x_positions[i], y_positions[i], z));
        }

        meshObjs = new GameObject[numEdges];
        meshes = new Mesh[numEdges];

        for (int i = 0; i < numEdges; i++) {

            // Set up mesh for each quadrilateral
            meshes[i] = new Mesh();

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(x_positions[i], y_positions[i], z);
            vertices[1] = new Vector3(x_positions[i + 1], y_positions[i + 1], z);
            vertices[2] = new Vector3(x_positions[i + 1], bottom, z);
            vertices[3] = new Vector3(x_positions[i], bottom, z);

            Vector2[] uvs = new Vector2[4];
            uvs[0] = new Vector2(0f, 1f);
            uvs[1] = new Vector2(1f, 1f);
            uvs[2] = new Vector2(1f, 0f);
            uvs[3] = new Vector2(0f, 0f);

            // Quadrilateral has two triangles: 012 and 230
            int[] triangles = new int[6]{0, 1, 2, 2, 3, 0};

            meshes[i].vertices = vertices;
            meshes[i].uv = uvs;
            meshes[i].triangles = triangles;

            meshObjs[i] = Instantiate(bodyMesh, Vector3.zero, Quaternion.identity);
            meshObjs[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshObjs[i].transform.parent = transform;
        }
    }

    void FixedUpdate() {
        // Compute independent update of each node
        for (int i = 0; i < numNodes; i++) {
            float y_disp = y_positions[i] - baseHeight;
            float y_accel = - springConstant * y_disp - damping * y_vels[i];

            y_positions[i] += y_vels[i] * Time.deltaTime;
            y_vels[i] += y_accel * Time.deltaTime;

            surfaceRenderer.SetPosition(i, new Vector3(x_positions[i], y_positions[i], z));
        }

        // Compute influence from neighbor nodes
        float[] diffs = new float[numNodes];
        for (int i = 0; i < 8; i++) {

            // Compute pulling force from neighbors
            for (int j = 0; j < numNodes; j++) {
                diffs[j] = 0;
                if (j > 0) {
                    diffs[j] += spread * (y_positions[j - 1] - y_positions[j]);
                }
                if (j < numNodes - 1) {
                    diffs[j] += spread * (y_positions[j + 1] - y_positions[j]);
                }
            }

            // Update velocity and position
            for (int j = 0; j < numNodes; j++) {
                y_vels[j] += diffs[j] * Time.deltaTime;
                // Behaves better when the next line is commented out
                // y_positions[j] += diffs[j] * Time.deltaTime;
            }
        }

        UpdateMeshPositions();
    }

    void Update() {
        if (Input.GetKeyDown("space")) {
            SplashPercent(Random.Range(left + 0.15f * width, left + 0.85f * width), 0.3f, -5f);
        }
    }

    // Splash a percent of nodes at a particular location
    public void SplashPercent(float x_pos, float percent, float vel) {
        if (x_pos < x_positions[0] || x_pos > x_positions[numNodes - 1]) {
            return;
        }

        int range = (int) (numNodes * percent);
        float percentPos = (x_pos - x_positions[0]) / (x_positions[numNodes - 1] - x_positions[0]);
        int mid = Mathf.RoundToInt((numNodes - 1) * percentPos);
        int midLow = mid - range / 2;
        int midHigh = mid + range / 2;
        if (midLow < 0) {
            midLow = 0;
            midHigh = range;
            mid = range / 2;
        } else if (midHigh > numNodes - 1) {
            midHigh = numNodes - 1;
            midLow = midHigh - range;
            mid = midHigh - range / 2;
        }

        for (int i = midLow; i <= midHigh; i++) {
            y_vels[i] += vel;
        }

        Vector3 pos = new Vector3(x_positions[mid], y_positions[mid], z);
        GenerateParticles(pos);
    }

    // Update the positions of the vertices of the meshes
    private void UpdateMeshPositions() {
        for (int i = 0; i < meshes.Length; i++) {
            Vector3[] vertices = meshes[i].vertices;
            vertices[0].y = y_positions[i];  // Only the top two vertices change
            vertices[1].y = y_positions[i + 1];
            meshes[i].vertices = vertices;
        }
    }

    // Generate particles with random velocity at a given position
    private void GenerateParticles(Vector3 pos) {
        for (int i = 0; i < Random.Range(10, 20); i++) {
            Vector3 particleVel = new Vector3(Random.Range(-5f, 5f), Random.Range(0f, 5f), 0f);
            Particle particle = Instantiate(splashParticlePrefab, pos, Quaternion.identity);
            particle.velocity = particleVel;
        }
    }
}
