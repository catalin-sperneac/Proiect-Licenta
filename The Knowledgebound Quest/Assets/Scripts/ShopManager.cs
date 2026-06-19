using UnityEngine;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopInventoryManager shopInventoryManager;
    private bool playerInRange = false;

    void Awake()
    {
        shopInventoryManager = gameObject.GetComponent<ShopInventoryManager>();
    }

    private void Update()
    {
        if(!playerInRange)
        {
            return;
        }
        if(InventoryManager.instance.isOpen)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && !InventoryManager.instance.blockInteraction && !UIManager.instance.isGameOver)
        {
            UIManager.instance.DeactivateInteractText();
            InventoryManager.instance.isShop = true;
            InventoryManager.instance.OpenInventory(
                PlayerInventoryManager.Instance.playerInventory,
                shopInventoryManager.shopInventory
            );
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.ShowInteractText("Press E to open shop");
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.DeactivateInteractText();
            playerInRange = false;
            InventoryManager.instance.isShop = false;
            InventoryManager.instance.CloseInventory();
        }
    }
}

