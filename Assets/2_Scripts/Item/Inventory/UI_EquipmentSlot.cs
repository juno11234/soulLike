using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EquipmentSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    public PlayerEquipment playerEquipment;
    public PlayerInventory playerInventory;
    public UI_Inventory uiInventory; // Assign in inspector

    private WeaponItem equippedWeapon => playerEquipment.equippedWeapon;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Transform originalParent;
    private Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }
    }

    void Start()
    {
        if (playerEquipment != null)
        {
            playerEquipment.onEquipmentChanged += OnEquipmentChanged;
            UpdateSlotUI();
        }
    }

    void OnDestroy()
    {
        if (playerEquipment != null)
        {
            playerEquipment.onEquipmentChanged -= OnEquipmentChanged;
        }
    }

    void OnEquipmentChanged(WeaponItem newItem, WeaponItem oldItem)
    {
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (equippedWeapon != null)
        {
            icon.sprite = equippedWeapon.Icon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        // Pass the drop handling to the central UI_Inventory manager
        uiInventory.HandleDrop(droppedObject, gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (equippedWeapon == null) return;
        
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        transform.SetParent(parentCanvas.transform);
        icon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (equippedWeapon == null) return;
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (equippedWeapon == null) return;
        
        // Reset visual state
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        icon.raycastTarget = true;
        
        // If not dropped on a valid target, the item simply returns to its original position.
        // The OnDrop methods of valid targets will handle the actual logic.
    }
}