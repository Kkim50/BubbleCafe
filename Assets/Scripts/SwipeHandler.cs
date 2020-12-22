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
    public int startingViewIndex = 0;

    private float swipeStartTime = 0f;
    private int currView;
    private int nextView;
    private int prevView;
    private Vector3 initialOffset;
    private Vector3 nextOffset;
    private Vector3 prevOffset;

    void Start()
    {
        currView = startingViewIndex;
        nextView = GetNextViewIndex(currView);
        prevView = GetPrevViewIndex(currView);

        initialOffset = transform.position;
        nextOffset = new Vector3(Screen.width, 0, 0);
        prevOffset = new Vector3(-Screen.width, 0, 0);

        views[currView].transform.position = initialOffset;
        views[nextView].transform.position = initialOffset + nextOffset;
        views[prevView].transform.position = initialOffset + prevOffset;
    }

    public void OnBeginDrag(PointerEventData data) {
        swipeStartTime = Time.time;
    }

    public void OnDrag(PointerEventData data) {
        float diff = data.position.x - data.pressPosition.x;
        Vector3 diffVec = new Vector3(Mathf.Clamp(diff, -Screen.width, Screen.width), 0, 0);
        views[currView].transform.position = initialOffset + diffVec;
        views[nextView].transform.position = initialOffset + diffVec + nextOffset;
        views[prevView].transform.position = initialOffset + diffVec + prevOffset;
    }

    public void OnEndDrag(PointerEventData data) {
        float swipePercent = (data.position.x - data.pressPosition.x) / Screen.width;
        float swipeTime = Time.time - swipeStartTime;
        float swipeSpeed = Mathf.Abs(data.position.x - data.pressPosition.x) / swipeTime;
        if (Mathf.Abs(swipePercent) >= swipePercentThreshold || swipeSpeed >= swipeSpeedThreshold) {
            Vector3 newLocation = initialOffset;
            if (swipePercent < 0) {
                // Swipe to the left, move screen right
                prevView = currView;
                currView = nextView;
                nextView = GetNextViewIndex(currView);
            } else if (swipePercent > 0) {
                // Swipe to the right, move screen left
                nextView = currView;
                currView = prevView;
                prevView = GetPrevViewIndex(currView);
            }
            StartCoroutine(SmoothMove(views[currView].transform.position, newLocation, easingTime));
        } else {
            StartCoroutine(SmoothMove(views[currView].transform.position, initialOffset, easingTime));
        }
    }

    IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float moveTime) {
        float t = 0f;
        while (t <= moveTime) {
            t += Time.deltaTime;
            Vector3 lerpedPos = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t / moveTime));
            views[currView].transform.position = lerpedPos;
            views[nextView].transform.position = lerpedPos + nextOffset;
            views[prevView].transform.position = lerpedPos + prevOffset;
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
