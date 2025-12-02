using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public delegate void OnEquipmentChanged(WeaponItem newItem, WeaponItem oldItem);
    public event OnEquipmentChanged onEquipmentChanged;

    public WeaponItem equippedWeapon { get; private set; }

    public void Equip(WeaponItem newItem)
    {
        if (newItem == equippedWeapon) return;

        WeaponItem oldItem = equippedWeapon;
        equippedWeapon = newItem;

        onEquipmentChanged?.Invoke(newItem, oldItem);

        // Here you would typically also handle stat changes, visual updates, etc.
        Debug.Log("Equipped: " + (newItem != null ? newItem.ItemName : "Nothing"));
    }

    public WeaponItem Unequip()
    {
        if (equippedWeapon == null) return null;

        WeaponItem unequippedItem = equippedWeapon;
        equippedWeapon = null;

        onEquipmentChanged?.Invoke(null, unequippedItem);
        
        Debug.Log("Unequipped: " + unequippedItem.ItemName);
        return unequippedItem;
    }
}