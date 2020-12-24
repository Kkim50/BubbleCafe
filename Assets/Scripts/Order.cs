using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Size {
    none = 0,
    small = 70,
    medium = 100,
    large = 130
}

public enum Tea {
    black,
    green,
    oolong
}

public enum Topping {
    boba
}

public class Order
{
    // Size of the current cup
    public Size size;

    // How full the current cup is
    public int totalAmount = 0;

    // How much of each tea is in the cup
    public Dictionary<Tea, int> teaAmounts = new Dictionary<Tea, int>();

    // Toppings in the cup
    public List<Topping> toppings = new List<Topping>();

    public Order() {
        this.size = Size.none;
    }

    public Order(Size size) {
        this.size = size;
    }
}
