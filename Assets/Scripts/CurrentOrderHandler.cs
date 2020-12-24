using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentOrderHandler : MonoBehaviour
{
    public Order currentOrder;

    // Start is called before the first frame update
    void Start()
    {
        ResetCurrentOrder();
    }

    public void AddTea(Tea tea, int amount) {
        if (currentOrder.size == Size.none) {
            Debug.Log("Size not set!");
            return;
        }
        if (!currentOrder.teaAmounts.ContainsKey(tea)) {
            currentOrder.teaAmounts.Add(tea, 0);
        }
        currentOrder.teaAmounts[tea] += amount;
        currentOrder.totalAmount += amount;
        Debug.Log("Added " + amount);
        Debug.Log("Total: " + currentOrder.totalAmount);
        if (currentOrder.totalAmount > (int) currentOrder.size) {
            Debug.Log("Went over total amount!");
            ResetCurrentOrder();
        }
    }

    public void ResetCurrentOrder() {
        currentOrder = new Order();
    }

    public void SetSize(Size size) {
        if (currentOrder.size != Size.none) {
            Debug.Log("Size already set!");
            return;
        }
        currentOrder.size = size;
        Debug.Log("Size set to: " + size);
    }
}
