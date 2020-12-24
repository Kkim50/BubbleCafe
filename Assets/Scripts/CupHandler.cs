using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupHandler : MonoBehaviour
{
    public CurrentOrderHandler currentOrderHandler;
    public Size size;
    public GameObject cupPrefab;

    private GameObject draggedCup;

    void OnMouseDown() {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        draggedCup = Instantiate(cupPrefab, pos, Quaternion.identity);
    }

    void OnMouseDrag() {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        draggedCup.transform.position = pos;
    }

    void OnMouseUp() {
        Destroy(draggedCup);
        if (currentOrderHandler.currentOrder.size == Size.none) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null) {
                if (hit.transform.name == "SwipeArea") {
                    currentOrderHandler.SetSize(size);
                }
            }
        }
    }
}
