using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {
    public Vector3 velocity;

    public float gravity = 9.8f;

    void Update() {
        this.transform.position += velocity * Time.deltaTime;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(this.transform.position);
        if (screenPosition.x < 0 || screenPosition.x > Screen.width || screenPosition.y < 0) {
            Destroy(this.gameObject);
        }
        velocity += new Vector3(0, -gravity * Time.deltaTime, 0);
    }
}
