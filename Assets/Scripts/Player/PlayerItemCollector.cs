using UnityEngine;
using TMPro;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private Collider2D currentItemCollider;
    [SerializeField] private TextMeshProUGUI pickupPrompt;

    private void Awake()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inventoryManager == null)
            inventoryManager = FindAnyObjectByType<InventoryManager>();

        if (pickupPrompt == null)
        {
            Debug.LogWarning("PlayerItemCollector: Pickup prompt text not assigned in the inspector.");
        }
        else
        {
            pickupPrompt.text = "";
        }
    }

    private void Update()
    {
        if (currentItemCollider != null && Input.GetKeyDown(KeyCode.E))
        {
            PickupItem(currentItemCollider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                currentItemCollider = collision;
                if (pickupPrompt != null)
                    pickupPrompt.text = "(E) Pick up ";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && collision == currentItemCollider)
        {
            currentItemCollider = null;
            if (pickupPrompt != null)
                pickupPrompt.text = "";
        }
    }

    private void PickupItem(Collider2D itemCollider)
    {
        if (inventoryManager == null)
        {
            Debug.LogError("PlayerItemCollector: InventoryManager not found.");
            return;
        }

        Item item = itemCollider.GetComponent<Item>();
        if (item != null)
        {
            bool itemAdded = inventoryManager.AddItem(itemCollider.gameObject);
            if (itemAdded)
            {
                currentItemCollider = null;
                if (pickupPrompt != null)
                    pickupPrompt.text = "";
                Destroy(itemCollider.gameObject);
            }
        }
    }
}
