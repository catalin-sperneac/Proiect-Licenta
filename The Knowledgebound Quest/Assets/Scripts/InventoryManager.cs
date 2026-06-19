using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.Cinemachine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject chestPanel;
    [SerializeField] private GameObject shopPanel;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI shopDescriptionText;
    public bool isInBattle = false;

    public InventorySlot[] playerSlots;
    private InventorySlot[] chestSlots;
    public InventorySlot[] shopSlots;

    public bool isShop = false;

    private Inventory currentInventory;
    private Inventory chestInventory;
    private Inventory shopInventory;
    private InventorySlot firstSelectedSlot;

    private InventorySlot selectedSlot;
    public bool isOpen = false;

    private CoinManager coinManager;
    private CinemachineBrain cinemachineBrain;
    private Health playerHealth;
    public bool blockInteraction = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        inventoryPanel.SetActive(false);
        chestPanel.SetActive(false);
        shopPanel.SetActive(false);
        coinManager = GameObject.Find("Player").GetComponent<CoinManager>();
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        playerHealth = GameObject.Find("Player").GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.currentHP != 0)
        {
            HandleInventoryToggle();
        }
        else
        {
            cinemachineBrain.enabled = false;
        }
    }

    void HandleInventoryToggle()
    {
        if (!MenuManager.instance.isMenuActive && !MenuManager.instance.isMapActive && !BattleManager.instance.isBattleActive)
        {
            if(!isOpen && Input.GetKeyDown(KeyCode.I))
            {
                if (currentInventory == null)
                {
                    PlayerInventoryManager pim = FindFirstObjectByType<PlayerInventoryManager>();
                    currentInventory = pim.playerInventory;
                }
                OpenInventory(currentInventory);
            }
            if(isOpen)
            {
                if(!chestPanel.activeSelf && !shopPanel.activeSelf && Input.GetKeyDown(KeyCode.I))
                {
                    CloseInventory();
                }
                if((chestPanel.activeSelf || shopPanel.activeSelf) && Input.GetKeyDown(KeyCode.E))
                {
                    CloseInventory();
                    isShop = false;
                }
            }
        }
    }

    public void OpenInventory(Inventory playerInventory, Inventory secondaryInventory = null, bool inBattle = false)
    {
        cinemachineBrain.enabled = false;
        StartCoroutine(OpenInventoryRoutine(playerInventory, secondaryInventory, inBattle));
    }

    private IEnumerator OpenInventoryRoutine(Inventory playerInventory, Inventory secondaryInventory, bool inBattle)
    {
        inventoryPanel.SetActive(true);
        bool isChestOpen = false;
        bool isShopOpen = false;
        if (!isShop && secondaryInventory != null)
        {
            chestPanel.SetActive(true);
            isChestOpen = true;
        }
        if (isShop && secondaryInventory != null)
        {
            shopPanel.SetActive(true);
            isShopOpen = true;
        }
        yield return null;
        playerSlots = inventoryPanel.GetComponentsInChildren<InventorySlot>(true);
        chestSlots = chestPanel.GetComponentsInChildren<InventorySlot>(true);
        shopSlots = shopPanel.GetComponentsInChildren<InventorySlot>(true);
        currentInventory = playerInventory;
        if (isShop)
        {
            shopInventory = secondaryInventory;
            foreach (var slot in shopSlots)
            {
                slot.ResetHighlight();
            }
        }
        else
        {
            chestInventory = secondaryInventory;
        }
        isInBattle = inBattle;
        RefreshUI();
        isOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!isShopOpen && !isChestOpen && !inBattle)
        {
            Time.timeScale = 0f;
        }
    }

    public void CloseInventory()
    {
        cinemachineBrain.enabled = true;
        if (firstSelectedSlot != null)
        {
            firstSelectedSlot.ResetHighlight();
            firstSelectedSlot = null;
        }
        inventoryPanel.SetActive(false);
        chestPanel.SetActive(false);
        shopPanel.SetActive(false);
        if (!isInBattle)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Time.timeScale = 1f;
        isOpen = false;
        StartCoroutine(BlockInteraction());
    }

    IEnumerator BlockInteraction()
    {
        blockInteraction = true;
        yield return null;
        blockInteraction = false;
    }

    public void RefreshUI()
    {
        if (playerSlots == null || currentInventory == null)
        {
            return;
        }
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (i < currentInventory.items.Count)
            {
                playerSlots[i].AddItem(currentInventory.items[i]);
            }
            else
            {
                playerSlots[i].ClearSlot();
            }
        }
        if (chestInventory != null && chestSlots != null)
        {
            for (int i = 0; i < chestSlots.Length; i++)
            {
                if (i < chestInventory.items.Count)
                {
                    chestSlots[i].AddItem(chestInventory.items[i]);
                }
                else
                {
                    chestSlots[i].ClearSlot();
                }
            }
        }
        if (isShop && shopInventory != null && shopSlots != null)
        {
            for (int i = 0; i < shopSlots.Length; i++)
            {
                if (i < shopInventory.items.Count)
                    shopSlots[i].AddItem(shopInventory.items[i]);
                else
                    shopSlots[i].ClearSlot();
            }
        }
    }

    public void HandleSlotClick(InventorySlot clickedSlot)
    {
        if (isShop)
        {
            HandleShopClick(clickedSlot);
            return;
        }
        HandleInventorySlotClick(clickedSlot);
    }

    private void HandleShopClick(InventorySlot clickedSlot)
    {
        bool clickedIsShop = shopSlots.Contains(clickedSlot);
        bool clickedIsPlayer = playerSlots.Contains(clickedSlot);
        if (firstSelectedSlot == null)
        {
            if (clickedIsPlayer)
            {
                ResetAllHighlights();
                firstSelectedSlot = null;
                return;
            }
            if (!clickedIsShop || clickedSlot.currentItem == null)
            {
                return;
            }
            firstSelectedSlot = clickedSlot;
            firstSelectedSlot.FirstPickedHighlight();
            HighlightSlotsShop(playerSlots);
            return;
        }
        if (!clickedIsPlayer)
        {
            ResetAllHighlights();
            firstSelectedSlot = null;
            return;
        }
        if (clickedSlot.currentItem != null)
        {
            shopDescriptionText.text = "Slot must be empty";
            ResetAllHighlights();
            firstSelectedSlot = null;
            return;
        }
        ItemData itemToBuy = firstSelectedSlot.currentItem;
        if (coinManager.coins < itemToBuy.cost)
        {
            shopDescriptionText.text = "Not enough coins!";
            ResetAllHighlights();
            firstSelectedSlot = null;
            return;
        }
        coinManager.coins -= itemToBuy.cost;
        currentInventory.items.Add(itemToBuy);
        shopInventory.items.Remove(itemToBuy);
        ResetAllHighlights();
        firstSelectedSlot = null;
        RefreshUI();
    }

    public void HandleInventorySlotClick(InventorySlot clickedSlot)
    {
        bool firstIsPlayer = playerSlots.Contains(firstSelectedSlot);
        bool secondIsPlayer = playerSlots.Contains(clickedSlot);
        if (isInBattle || chestInventory == null)
        {
            return;
        }
        if (firstSelectedSlot == null)
        {
            if (clickedSlot.currentItem == null)
            {
                return;
            }
            firstSelectedSlot = clickedSlot;
            firstSelectedSlot.FirstPickedHighlight();
            firstIsPlayer = playerSlots.Contains(firstSelectedSlot);
            secondIsPlayer = playerSlots.Contains(clickedSlot);
            if (firstIsPlayer)
            {
                HighlightSlotsChest(chestSlots);
            }
            else
            {
                HighlightSlotsChest(playerSlots);
            }
            return;
        }
        if (clickedSlot == firstSelectedSlot)
        {
            ResetAllHighlights();
            firstSelectedSlot = null;
            return;
        }
        firstIsPlayer = playerSlots.Contains(firstSelectedSlot);
        secondIsPlayer = playerSlots.Contains(clickedSlot);
        if (firstIsPlayer == secondIsPlayer)
        {
            ResetAllHighlights();
            firstSelectedSlot = null;
            return;
        }
        Inventory source = firstIsPlayer ? currentInventory : chestInventory;
        Inventory destination = secondIsPlayer ? currentInventory : chestInventory;
        ItemData itemA = firstSelectedSlot.currentItem;
        ItemData itemB = clickedSlot.currentItem;
        if (itemB == null)
        {
            if (source.items.Remove(itemA))
            {
                destination.items.Add(itemA);
            }
            else
            {
                Debug.LogWarning("Move failed: item not found in source inventory list.");
            }
        }
        else
        {
            int idxA = source.items.IndexOf(itemA);
            int idxB = destination.items.IndexOf(itemB);

            if (idxA >= 0 && idxB >= 0)
            {
                source.items[idxA] = itemB;
                destination.items[idxB] = itemA;
            }
            else
            {
                source.items.Remove(itemA);
                destination.items.Remove(itemB);
                source.items.Add(itemB);
                destination.items.Add(itemA);
            }
        }
        ResetAllHighlights();
        firstSelectedSlot = null;
        RefreshUI();
    }

    public void ShowTooltip(ItemData item, bool isShopSlot = false)
    {
        if (isShopSlot)
        {
            if (shopDescriptionText != null)
            {
                shopDescriptionText.text = "Cost: " + item.cost + "\n" + item.description;
            }
        }
        else
        {
            if (!isShop)
            {
                if (descriptionText != null)
                {
                    descriptionText.text = item.description;
                }
            }
            else
            {
                if (descriptionText != null)
                {
                    descriptionText.text = item.description;
                }
            }
        }
    }

    public void HideTooltip()
    {
        if (descriptionText != null)
        {
            descriptionText.text = "";
        }
        if (shopDescriptionText != null)
        {
            shopDescriptionText.text = "";
        }
    }

    private void HighlightSlotsChest(InventorySlot[] slots)
    {
        foreach(var slot in slots)
        {
            if(slot.currentItem == null)
            {
                slot.EmptyHighlight();
            }
            else
            {
                slot.SwapHighlight();
            }
        }
    }

    private void HighlightSlotsShop(InventorySlot[] slots)
    {
        foreach(var slot in slots)
        {
            if(slot.currentItem == null)
            {
                slot.EmptyHighlight();
            }
        }
    }

    private void ResetAllHighlights()
    {
        foreach(var slot in playerSlots)
        {
            slot.ResetHighlight();
        }
        foreach(var slot in chestSlots)
        {
            slot.ResetHighlight();
        }
        foreach(var slot in shopSlots)
        {
            slot.ResetHighlight();
        }
    }
}