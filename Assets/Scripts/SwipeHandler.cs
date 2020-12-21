using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float swipePercentThreshold = 0.2f;
    public float swipeSpeedThreshold = 1000f;
    public float easingTime = 0.25f;

    private Vector3 viewLocation;
    private float swipeStartTime = 0f;

    void Start()
    {
        viewLocation = transform.position;
    }

    public void OnBeginDrag(PointerEventData data) {
        swipeStartTime = Time.time;
    }

    public void OnDrag(PointerEventData data) {
        float diff = data.position.x - data.pressPosition.x;
        this.transform.position = viewLocation + new Vector3(diff, 0, 0);
    }

    public void OnEndDrag(PointerEventData data) {
        float swipePercent = (data.position.x - data.pressPosition.x) / Screen.width;
        float swipeTime = Time.time - swipeStartTime;
        float swipeSpeed = Mathf.Abs(data.position.x - data.pressPosition.x) / swipeTime;
        if (Mathf.Abs(swipePercent) >= swipePercentThreshold || swipeSpeed >= swipeSpeedThreshold) {
            Vector3 newLocation = viewLocation;
            if (swipePercent < 0) {
                // Swipe to the left, move screen right
                newLocation += new Vector3(-Screen.width, 0, 0);
            } else if (swipePercent > 0) {
                // Swipe to the right, move screen left
                newLocation += new Vector3(Screen.width, 0, 0);
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easingTime));
            viewLocation = newLocation;
        } else {
            StartCoroutine(SmoothMove(transform.position, viewLocation, easingTime));
        }
    }

    IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float moveTime) {
        float t = 0f;
        while (t <= moveTime) {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t / moveTime));
            yield return null;
        }
    }
}
