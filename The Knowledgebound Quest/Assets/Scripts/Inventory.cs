using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public int capacity;
    public List<ItemData> items = new();

    public Inventory(int capacity)
    {
        this.capacity = capacity;
    }

    public bool AddItem(ItemData item)
    {
        if (items.Count >= capacity)
            return false;

        items.Add(item);
        return true;
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
    }
}

