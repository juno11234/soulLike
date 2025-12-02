using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    public List<InventorySlot> Slots = new List<InventorySlot>();

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged onInventoryChangedCallback;

    private int inventorySize = 20; // Default inventory size

    public Inventory(int size)
    {
        inventorySize = size;
        for (int i = 0; i < inventorySize; i++)
        {
            Slots.Add(new InventorySlot(null, 0)); // Initialize empty slots
        }
    }

    public void TriggerInventoryChanged()
    {
        onInventoryChangedCallback?.Invoke();
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        // Try to stack if the item is already in inventory and stackable
        if (item.MaxAmount > 1)
        {
            foreach (InventorySlot slot in Slots)
            {
                if (slot.Item == item && slot.Quantity < item.MaxAmount)
                {
                    int spaceLeft = item.MaxAmount - slot.Quantity;
                    int amountToAdd = Mathf.Min(quantity, spaceLeft);
                    slot.AddQuantity(amountToAdd);
                    quantity -= amountToAdd;

                    if (quantity == 0)
                    {
                        TriggerInventoryChanged();
                        return true;
                    }
                }
            }
        }

        // Add to an empty slot if there's still quantity left
        if (quantity > 0)
        {
            foreach (InventorySlot slot in Slots)
            {
                if (slot.Item == null)
                {
                    int amountToAdd = Mathf.Min(quantity, item.MaxAmount);
                    slot.Item = item;
                    slot.Quantity = amountToAdd;
                    quantity -= amountToAdd;

                    if (quantity == 0)
                    {
                        TriggerInventoryChanged();
                        return true;
                    }
                }
            }
        }

        // If we reach here, inventory is full or item is not stackable and no space
        Debug.LogWarning("Inventory full or no space for item: " + item.ItemName);
        TriggerInventoryChanged(); // Even if it fails, UI might need an update
        return false;
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        // Remove from existing stacks
        for (int i = Slots.Count - 1; i >= 0; i--)
        {
            InventorySlot slot = Slots[i];
            if (slot.Item == item)
            {
                int amountToRemove = Mathf.Min(quantity, slot.Quantity);
                slot.RemoveQuantity(amountToRemove);
                quantity -= amountToRemove;

                if (slot.Quantity == 0)
                {
                    slot.Item = null; // Clear the slot if quantity is 0
                }

                if (quantity == 0)
                {
                    TriggerInventoryChanged();
                    return true;
                }
            }
        }

        Debug.LogWarning("Item not found or not enough quantity: " + item.ItemName);
        TriggerInventoryChanged(); // Even if it fails, UI might need an update
        return false;
    }

    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < Slots.Count)
        {
            return Slots[index];
        }
        return null;
    }

    public int GetTotalQuantity(Item item)
    {
        return Slots.Where(slot => slot.Item == item).Sum(slot => slot.Quantity);
    }
}