using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float swipePercentThreshold = 0.2f;
    public float swipeSpeedThreshold = 1000f;
    public float easingTime = 0.25f;
    public List<GameObject> views;

    private Vector3 viewLocation;
    private Vector3 initialOffset;
    private int currView = 0;
    private float swipeStartTime = 0f;

    void Start()
    {
        viewLocation = transform.position;
        initialOffset = transform.position;
        currView = 0;
    }

    public void OnBeginDrag(PointerEventData data) {
        swipeStartTime = Time.time;
    }

    public void OnDrag(PointerEventData data) {
        float diff = data.position.x - data.pressPosition.x;
        this.transform.position = viewLocation + new Vector3(Mathf.Clamp(diff, -Screen.width, Screen.width), 0, 0);
    }

    public void OnEndDrag(PointerEventData data) {
        float swipePercent = (data.position.x - data.pressPosition.x) / Screen.width;
        float swipeTime = Time.time - swipeStartTime;
        float swipeSpeed = Mathf.Abs(data.position.x - data.pressPosition.x) / swipeTime;
        if (Mathf.Abs(swipePercent) >= swipePercentThreshold || swipeSpeed >= swipeSpeedThreshold) {
            if (swipePercent < 0) {
                // Swipe to the left, move screen right
                currView = GetNextViewIndex(currView);
            } else if (swipePercent > 0) {
                // Swipe to the right, move screen left
                currView = GetPrevViewIndex(currView);
            }
            Vector3 newLocation = initialOffset + new Vector3(currView * -Screen.width, 0, 0);
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

    public int GetNextViewIndex(int index) {
        return (index + 1) % views.Count;
    }

    public int GetPrevViewIndex(int index) {
        return (index - 1 + views.Count) % views.Count;
    }
}
