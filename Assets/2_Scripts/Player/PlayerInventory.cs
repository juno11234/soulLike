using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory;
    public int inventorySize = 20; // Customizable size from Inspector

    void Awake()
    {
        inventory = new Inventory(inventorySize);
    }

    // Example method to add an item to the player's inventory
    public bool AddItemToInventory(Item item, int quantity = 1)
    {
        return inventory.AddItem(item, quantity);
    }

    // Example method to remove an item from the player's inventory
    public bool RemoveItemFromInventory(Item item, int quantity = 1)
    {
        return inventory.RemoveItem(item, quantity);
    }

    // You can add more methods here to expose inventory functionality
    public InventorySlot GetInventorySlot(int index)
    {
        return inventory.GetSlot(index);
    }
}