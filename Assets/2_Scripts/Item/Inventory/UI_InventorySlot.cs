using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // Assuming TextMeshPro for text

public class UI_InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public TextMeshProUGUI quantityText;
    public GameObject highlight; // Visual feedback for selection/hover

    [HideInInspector] public InventorySlot inventorySlot;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public Canvas parentCanvas; // Reference to the canvas to handle drag correctly
    [HideInInspector] public UI_Inventory parentInventoryUI; // Reference to the parent UI_Inventory

    private Vector2 originalPosition;
    private Transform originalParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }
    }

    public void Init(InventorySlot slot, UI_Inventory inventoryUI)
    {
        inventorySlot = slot;
        parentInventoryUI = inventoryUI;
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (inventorySlot != null && inventorySlot.Item != null)
        {
            icon.sprite = inventorySlot.Item.Icon;
            icon.enabled = true;
            quantityText.text = inventorySlot.Quantity.ToString();
            quantityText.enabled = inventorySlot.Item.MaxAmount > 1; // Only show quantity if stackable
        }
        else
        {
            icon.enabled = false;
            quantityText.enabled = false;
        }
    }

    // --- Drag Handlers ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventorySlot.Item == null) return;

        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // Make the dragged item follow the mouse
        transform.SetParent(parentCanvas.transform);
        icon.raycastTarget = false; // Disable raycast so we can drop onto other slots
        quantityText.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventorySlot.Item == null) return;
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventorySlot.Item == null) return;

        // Reset parent and raycast target
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        icon.raycastTarget = true;
        quantityText.raycastTarget = true;

        // If the item was dropped on an invalid area, return to original position
        // This is handled implicitly by resetting position, but more complex logic might go here
    }

    // --- Drop Handler ---
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null || droppedObject == this.gameObject) return;
        
        // Pass the drop handling to the central UI_Inventory manager
        parentInventoryUI.HandleDrop(droppedObject, gameObject);
    }

    // Optional: Add hover feedback
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlight != null) highlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlight != null) highlight.SetActive(false);
    }
}