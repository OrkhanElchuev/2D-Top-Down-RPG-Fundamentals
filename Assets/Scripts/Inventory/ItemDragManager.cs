using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 1f; // Minimum distance from the original slot to allow dropping the item
    public float maxDropDistance = 3f; // Maximum distance from the original slot to allow dropping the item


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; // Make the item semi-transparent while dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f; // Restore the item's original appearance

        // Check if the item was dropped on a valid slot
        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
  
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }

        // If the item was dropped on a valid slot, set the item's parent to that slot
        Slot originalSlot = originalParent.GetComponent<Slot>();

        // If the original slot had an item, move it to the drop slot
        if (dropSlot != null)
        {
            if (dropSlot.currentItem != null)
            {
                // Move the item from the drop slot back to the original slot
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;            
            }

            // Set the dragged item as the current item of the drop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            if (!IsWithinInventoryBounds(eventData.position))
            {
                DropItem(originalSlot);
            }
            else
            {
                // If the item was not dropped on a valid slot but is still within the inventory bounds, return it to its original slot
                transform.SetParent(originalParent);
            }
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset the item's position to the center of the slot
    }

    // Helper method to check if the mouse is within the inventory bounds
    bool IsWithinInventoryBounds(Vector2 mousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    // Method to drop the item into the game world if it's dragged outside the inventory bounds
    void DropItem(Slot originalSlot)
    {
        originalSlot.currentItem = null;
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("ItemDragManager: Player not found in the scene.");
            return;
        }

        // Calculate a random drop position around the player within the specified distance range
        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset;

        // Instantiate the item at the drop position and destroy the original item in the inventory
        Instantiate(gameObject, dropPosition, Quaternion.identity);
        Destroy(gameObject);
    }
}