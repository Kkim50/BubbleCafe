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
        if (!currentOrder.teaAmounts.ContainsKey(tea)) {
            currentOrder.teaAmounts.Add(tea, 0);
        }
        currentOrder.teaAmounts[tea] += amount;
        currentOrder.totalAmount += amount;
        if (currentOrder.totalAmount > (int) currentOrder.size) {
            Debug.Log("Went over total amount!");
            ResetCurrentOrder();
        }
    }

    public void ResetCurrentOrder() {
        currentOrder = new Order();
        currentOrder.size = Size.medium;  // TODO: Remove once sizes are implemented
    }
}
