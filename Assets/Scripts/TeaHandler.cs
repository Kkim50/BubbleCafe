using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.EventSystems;

public class TeaHandler : MonoBehaviour
{
    private float holdStartTime;
    private bool held;
    // Start is called before the first frame update
    void Start()
    {
        holdStartTime = 0f;
        held = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown() {
        holdStartTime = Time.time;
        held = true;
    }

    void OnMouseDrag() {
        if (held) {
            float timeHeld = Time.time - holdStartTime;
            Debug.Log("Held: " + timeHeld);
        }
    }

    void OnMouseExit() {
        held = false;
    }
}
