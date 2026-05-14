using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupUIManager : MonoBehaviour
{
    public static ItemPickupUIManager Instance { get; private set; }

    public GameObject popupPrefab;
    public int maxPopups = 5;
    public float popupDuration = 2f;

    private readonly Queue<GameObject> activePopups = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Method to show the pickup popup with item name and icon
    public void ShowItemPickupPopup(string itemName, Sprite itemIcon)
    {
        GameObject popup = Instantiate(popupPrefab, transform);
        popup.GetComponentInChildren<TMP_Text>().text = itemName;

        // Set the item icon in the popup
        Image itemImage = popup.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (itemImage)
        {
            itemImage.sprite = itemIcon;
        }

        // Enqueue the new popup and ensure we don't exceed the maximum number of popups
        activePopups.Enqueue(popup);
        if(activePopups.Count > maxPopups)
        {
            Destroy(activePopups.Dequeue());
        }
        
        // Start the coroutine to destroy the popup after a delay
        StartCoroutine(DestroyPopupAfterDelay(popup));
    }

    // Coroutine to fade out and destroy the popup after a delay
    private IEnumerator DestroyPopupAfterDelay(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);
        if (popup == null) yield break; 
        
        // Fade out the popup over 1 second
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        // If the popup doesn't have a CanvasGroup, add one for fading
        for (float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if (popup == null) yield break;
            canvasGroup.alpha = 1f - timePassed;
            yield return null;
        }

        Destroy(popup);
    }
}
