using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.EventSystems;

public class TeaHandler : MonoBehaviour
{
    public SaveDataManager saveManager;
    public CurrentOrderHandler currentOrderHandler;

    public List<Tea> teas;
    private int currentTeaIndex;
    private float holdStartTime;
    private bool held;

    // Start is called before the first frame update
    void Start()
    {
        teas = saveManager.saveData.availableTeas;
        currentTeaIndex = 0;
        holdStartTime = 0f;
        held = false;
    }

    void OnMouseDown() {
        holdStartTime = Time.time;
        held = true;
    }

    void OnMouseDrag() {
        if (held) {
            float timeHeld = Time.time - holdStartTime;
            if (timeHeld > 1.0f) {
                currentOrderHandler.AddTea(teas[currentTeaIndex], 10);
                holdStartTime = Time.time;
            }
        }
    }

    void OnMouseExit() {
        held = false;
    }
}
