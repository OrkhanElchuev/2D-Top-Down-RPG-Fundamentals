using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalParent = transform.parent;
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
            // If the item was not dropped on a valid slot, return it to its original position
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset the item's position to the center of the slot
    }
}