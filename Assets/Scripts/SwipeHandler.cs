using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 viewLocation;
    public float swipeThreshold = 0.2f;
    public float easingTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        viewLocation = transform.position;
    }

    public void OnBeginDrag(PointerEventData data) {
        
    }

    public void OnDrag(PointerEventData data) {
        
    }

    public void OnEndDrag(PointerEventData data) {
        float swipePercent = (data.pressPosition.x - data.position.x) / Screen.width;
        if (Mathf.Abs(swipePercent) >= swipeThreshold) {
            Vector3 newLocation = viewLocation;
            if (swipePercent > 0) {
                newLocation += new Vector3(-Screen.width, 0, 0);
            } else if (swipePercent < 0) {
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
