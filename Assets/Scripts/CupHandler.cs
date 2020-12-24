using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupHandler : MonoBehaviour
{
    public CurrentOrderHandler currentOrderHandler;
    public Size size;

    void OnMouseDown() {

    }

    void OnMouseUp() {
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
