using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpring : MonoBehaviour {

    public Vector2 velocity = new Vector2(0f, 0f);
    public float targetHeight = 0f;
    public float springConst = 0f;
    public float dampingConst = 0f;

    public void UpdateSpring() {
        float dispY = this.transform.position.y - targetHeight;
        float accelY = - springConst * dispY - dampingConst * this.velocity.y;

        this.transform.position += new Vector3(velocity.x, velocity.y, 0f) * Time.deltaTime;
        velocity.y += accelY * Time.deltaTime;

        // TODO: Decay to zero velocity and original position
    }

}
