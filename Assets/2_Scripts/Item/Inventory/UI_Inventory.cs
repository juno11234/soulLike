using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI_Inventory : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public GameObject inventorySlotPrefab; // Assign in Inspector
    public Transform inventorySlotParent; // Parent transform for inventory slots
    public Transform equipmentSlotParent; // Parent transform for equipment slots

    public List<UI_InventorySlot> inventorySlots = new List<UI_InventorySlot>();
    public List<UI_InventorySlot> equipmentSlots = new List<UI_InventorySlot>(); // For future equipment UI

    public Button allFilterButton;
    public Button weaponFilterButton;
    public Button potionFilterButton;
    public Button materialFilterButton;
    public Button etcFilterButton;

    private ItemType currentFilter = ItemType.Etc; // Default to a non-displaying filter, or All

    void Start()
    {
        if (playerInventory != null)
        {
            playerInventory.inventory.onInventoryChangedCallback += UpdateInventoryUI;
            InitInventoryUI();
            SetupFilterButtons();
        }
        else
        {
            Debug.LogError("PlayerInventory not assigned to UI_Inventory!");
        }
    }

    void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.inventory.onInventoryChangedCallback -= UpdateInventoryUI;
        }
    }

    void InitInventoryUI()
    {
        // Clear existing slots if any
        foreach (Transform child in inventorySlotParent)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();

        // Create UI slots for the inventory
        for (int i = 0; i < playerInventory.inventory.Slots.Count; i++)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, inventorySlotParent);
            UI_InventorySlot uiSlot = slotGO.GetComponent<UI_InventorySlot>();
            if (uiSlot != null)
            {
                uiSlot.Init(playerInventory.inventory.Slots[i], this);
                inventorySlots.Add(uiSlot);
            }
        }
        UpdateInventoryUI();
    }

    void SetupFilterButtons()
    {
        allFilterButton?.onClick.AddListener(() => SetFilter(ItemType.Etc)); // "Etc" can be used as "All" if no specific Etc type
        weaponFilterButton?.onClick.AddListener(() => SetFilter(ItemType.Weapon));
        potionFilterButton?.onClick.AddListener(() => SetFilter(ItemType.Potion));
        materialFilterButton?.onClick.AddListener(() => SetFilter(ItemType.Material));
        etcFilterButton?.onClick.AddListener(() => SetFilter(ItemType.Etc)); // Assuming "Etc" is a catch-all for now
        
        SetFilter(ItemType.Etc); // Set initial filter to All/Etc
    }

    public void SetFilter(ItemType filterType)
    {
        currentFilter = filterType;
        UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slotData = playerInventory.inventory.Slots[i];
            UI_InventorySlot uiSlot = inventorySlots[i];

            if (slotData.Item != null)
            {
                // Only display if it matches the current filter, or if the filter is "All" (represented by Etc for now)
                bool shouldDisplay = (currentFilter == ItemType.Etc) || (slotData.Item.Type == currentFilter);
                uiSlot.gameObject.SetActive(shouldDisplay);
                if (shouldDisplay)
                {
                    uiSlot.UpdateSlotUI();
                }
            }
            else
            {
                uiSlot.gameObject.SetActive(true); // Always show empty slots
                uiSlot.UpdateSlotUI(); // To clear icon/text if slot becomes empty
            }
        }
    }

    public void HandleDrop(GameObject fromGO, GameObject toGO)
    {
        UI_InventorySlot fromInvSlot = fromGO.GetComponent<UI_InventorySlot>();
        UI_EquipmentSlot fromEquipSlot = fromGO.GetComponent<UI_EquipmentSlot>();

        UI_InventorySlot toInvSlot = toGO.GetComponent<UI_InventorySlot>();
        UI_EquipmentSlot toEquipSlot = toGO.GetComponent<UI_EquipmentSlot>();

        if (fromInvSlot != null && toInvSlot != null)
        {
            HandleInventoryToInventory(fromInvSlot, toInvSlot);
        }
        else if (fromInvSlot != null && toEquipSlot != null)
        {
            HandleInventoryToEquipment(fromInvSlot, toEquipSlot);
        }
        else if (fromEquipSlot != null && toInvSlot != null)
        {
            HandleEquipmentToInventory(fromEquipSlot, toInvSlot);
        }
        
        // Final UI update after any operation
        playerInventory.inventory.TriggerInventoryChanged();
    }

    private void HandleInventoryToInventory(UI_InventorySlot fromSlotUI, UI_InventorySlot toSlotUI)
    {
        InventorySlot fromSlot = fromSlotUI.inventorySlot;
        InventorySlot toSlot = toSlotUI.inventorySlot;

        if (fromSlot == toSlot) return;

        // If 'toSlot' is empty
        if (toSlot.Item == null)
        {
            toSlot.Item = fromSlot.Item;
            toSlot.Quantity = fromSlot.Quantity;
            fromSlot.Item = null;
            fromSlot.Quantity = 0;
        }
        // If 'toSlot' has the same item type and is stackable
        else if (toSlot.Item == fromSlot.Item && toSlot.Item.MaxAmount > 1)
        {
            int amountToStack = Mathf.Min(fromSlot.Quantity, toSlot.Item.MaxAmount - toSlot.Quantity);
            toSlot.AddQuantity(amountToStack);
            fromSlot.RemoveQuantity(amountToStack);
            if (fromSlot.Quantity == 0)
            {
                fromSlot.Item = null;
            }
        }
        // Different items or non-stackable, swap them
        else
        {
            Item tempItem = fromSlot.Item;
            int tempQuantity = fromSlot.Quantity;

            fromSlot.Item = toSlot.Item;
            fromSlot.Quantity = toSlot.Quantity;

            toSlot.Item = tempItem;
            toSlot.Quantity = tempQuantity;
        }
    }

    private void HandleInventoryToEquipment(UI_InventorySlot fromSlotUI, UI_EquipmentSlot toSlotUI)
    {
        if (fromSlotUI.inventorySlot.Item?.Type != ItemType.Weapon)
        {
            Debug.Log("Only weapons can be equipped here.");
            return;
        }

        WeaponItem weaponToEquip = fromSlotUI.inventorySlot.Item as WeaponItem;
        WeaponItem currentlyEquipped = toSlotUI.playerEquipment.equippedWeapon;
        
        // Equip the new item
        toSlotUI.playerEquipment.Equip(weaponToEquip);

        // Move the previously equipped item (if any) to the inventory slot
        fromSlotUI.inventorySlot.Item = currentlyEquipped;
        fromSlotUI.inventorySlot.Quantity = currentlyEquipped != null ? 1 : 0;
    }

    private void HandleEquipmentToInventory(UI_EquipmentSlot fromSlotUI, UI_InventorySlot toSlotUI)
    {
        WeaponItem weaponToUnequip = fromSlotUI.playerEquipment.equippedWeapon;
        if (weaponToUnequip == null) return;

        // Case 1: Target slot is empty. Unequip and move.
        if (toSlotUI.inventorySlot.Item == null)
        {
            toSlotUI.inventorySlot.Item = fromSlotUI.playerEquipment.Unequip();
            toSlotUI.inventorySlot.Quantity = 1;
        }
        // Case 2: Target slot has a weapon. Swap.
        else if (toSlotUI.inventorySlot.Item.Type == ItemType.Weapon)
        {
            WeaponItem weaponToEquip = toSlotUI.inventorySlot.Item as WeaponItem;
            // Unequip current and put it in the inventory slot
            toSlotUI.inventorySlot.Item = fromSlotUI.playerEquipment.Unequip();
            toSlotUI.inventorySlot.Quantity = 1;
            
            // Equip the item from the inventory
            fromSlotUI.playerEquipment.Equip(weaponToEquip);
        }
        // Case 3: Target slot is occupied by a non-weapon. Do nothing.
        else
        {
            Debug.Log("Cannot swap a weapon with a non-weapon item.");
            // The drag will just reset, no data changes.
        }
    }
}