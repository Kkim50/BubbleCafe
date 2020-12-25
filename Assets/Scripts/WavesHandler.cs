using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesHandler : MonoBehaviour {

    public WaveSpring springPrefab;
    public SpriteRenderer waterBounds;
    public Particle particlePrefab;

    // Determines how strong or weak each "spring" of the wave is
    public float springConst = 0.05f;

    // Determines how fast each "spring" slows down
    public float dampingConst = 0.05f;

    // Determines how much influence each "spring" exerts on its neighbors
    public float influence = 0.5f;

    private float targetHeight;
    private List<WaveSpring> springs = new List<WaveSpring>();

    void Start() {

        Bounds bounds = waterBounds.bounds;
        Bounds springBounds = springPrefab.GetComponent<SpriteRenderer>().bounds;

        targetHeight = bounds.max.y - springBounds.extents.y;

        float startX = bounds.min.x + springBounds.extents.x;
        float rangeX = bounds.max.x - startX;
        int numSprings = (int) (rangeX / springBounds.size.x) + 1;
        for (int i = 0; i < numSprings; i++) {
            Vector3 pos = new Vector3(startX + i * springBounds.size.x, targetHeight, 0f);
            WaveSpring spring = Instantiate(springPrefab, pos, Quaternion.identity);
            spring.targetHeight = targetHeight;
            spring.springConst = springConst;
            spring.dampingConst = dampingConst;
            springs.Add(spring);
        }
    }

    void Update() {
        if (Input.GetKeyDown("space")) {
            SplashMiddle(0.2f, -5);
        }

        for (int i = 0; i < springs.Count; i++) {
            springs[i].UpdateSpring();
        }

        // Apply pulling from neighbors
        for (int i = 0; i < springs.Count; i++) {
            if (i > 0) {
                float diff = springs[i].transform.position.y - springs[i - 1].transform.position.y;
                springs[i - 1].velocity.y += influence * diff;
                // springs[i - 1].transform.position = springs[i - 1].transform.position + new Vector3(0, influence * diff, 0);
            }
            if (i < springs.Count - 1) {
                float diff = springs[i].transform.position.y - springs[i + 1].transform.position.y;
                springs[i + 1].velocity.y += influence * diff;
                // springs[i + 1].transform.position = springs[i + 1].transform.position + new Vector3(0, influence * diff, 0);
            }
        }
    }

    void SplashMiddle(float percent, float speed) {
        int num = (int) (springs.Count * percent);
        int mid = (int) (springs.Count / 2);
        int midLow = mid - num / 2;
        int midHigh = mid + num / 2;
        for (int i = midLow; i <= midHigh; i++) {
            springs[i].velocity.y += speed;
        }

        for (int i = 0; i < Random.Range(10, 20); i++) {
            Vector3 vel = new Vector3(Random.Range(-5f, 5f), Random.Range(0f, 5f), 0);
            Particle particle = Instantiate(particlePrefab, springs[mid].transform.position, Quaternion.identity);
            particle.velocity = vel;
        }
    }
}
