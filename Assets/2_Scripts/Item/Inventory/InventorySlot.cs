using System;

[Serializable]
public class InventorySlot
{
    public Item Item;
    public int Quantity;

    public InventorySlot(Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public void AddQuantity(int amount)
    {
        Quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        Quantity -= amount;
    }
}