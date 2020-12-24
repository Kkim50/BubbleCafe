using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour
{
    public CurrentOrderHandler currentOrderHandler;
    public float screenWidthDistance;
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
    private Vector3 swipeStartPos;

    void Start()
    {
        currView = startingViewIndex;
        nextView = GetNextViewIndex(currView);
        prevView = GetPrevViewIndex(currView);

        initialOffset = new Vector3(0, views[currView].transform.position.y, 0);
        nextOffset = new Vector3(screenWidthDistance, 0, 0);
        prevOffset = new Vector3(-screenWidthDistance, 0, 0);

        // Hide all panels
        for (int i = 0; i < views.Count; i++) {
            views[i].SetActive(false);
        }
        views[currView].SetActive(true);
        views[nextView].SetActive(true);
        views[prevView].SetActive(true);

        views[currView].transform.position = initialOffset;
        views[nextView].transform.position = initialOffset + nextOffset;
        views[prevView].transform.position = initialOffset + prevOffset;
    }

    public void OnMouseDown() {
        swipeStartTime = Time.time;
        swipeStartPos = Input.mousePosition;
    }

    public void OnMouseDrag() {
        float diff = (Input.mousePosition.x - swipeStartPos.x) / Screen.width * screenWidthDistance;
        Vector3 diffVec = new Vector3(Mathf.Clamp(diff, -screenWidthDistance, screenWidthDistance), 0, 0);
        views[currView].transform.position = initialOffset + diffVec;
        views[nextView].transform.position = initialOffset + diffVec + nextOffset;
        views[prevView].transform.position = initialOffset + diffVec + prevOffset;
    }

    public void OnMouseUp() {
        float diffY = Input.mousePosition.y - swipeStartPos.y;
        float swipePercentY = diffY / Screen.height;
        if (swipePercentY > swipePercentThreshold) {
            currentOrderHandler.SendCurrentOrder();
            StartCoroutine(SmoothMove(views[currView].transform.position, initialOffset, easingTime));
            return;
        }

        float diffX = Input.mousePosition.x - swipeStartPos.x;
        float swipePercent = diffX / Screen.width;
        float swipeTime = Time.time - swipeStartTime;
        float swipeSpeed = Mathf.Abs(diffX) / swipeTime;
        if (Mathf.Abs(swipePercent) >= swipePercentThreshold || swipeSpeed >= swipeSpeedThreshold) {
            Vector3 newLocation = initialOffset;
            if (swipePercent < 0) {
                // Swipe to the left, move screen right
                views[prevView].SetActive(false);
                prevView = currView;
                currView = nextView;
                nextView = GetNextViewIndex(currView);
                views[nextView].SetActive(true);
                newLocation -= new Vector3(Mathf.Min(swipeSpeed / 2000f, screenWidthDistance), 0, 0);
            } else if (swipePercent > 0) {
                // Swipe to the right, move screen left
                views[nextView].SetActive(false);
                nextView = currView;
                currView = prevView;
                prevView = GetPrevViewIndex(currView);
                views[prevView].SetActive(true);
                newLocation += new Vector3(Mathf.Min(swipeSpeed / 2000f, screenWidthDistance), 0, 0);
            }
            StartCoroutine(OvershootMove(views[currView].transform.position, newLocation, initialOffset, easingTime, easingTime));
        } else {
            StartCoroutine(SmoothMove(views[currView].transform.position, initialOffset, easingTime));
        }
    }

    IEnumerator OvershootMove(Vector3 startPos, Vector3 overshootPos, Vector3 endPos, float overshootTime, float returnTime) {
        yield return SmoothMove(startPos, overshootPos, overshootTime);
        yield return SmoothMove(overshootPos, endPos, returnTime);
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
