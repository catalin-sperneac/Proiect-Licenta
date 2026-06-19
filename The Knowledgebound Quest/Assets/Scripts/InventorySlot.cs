using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData currentItem;
    public Image icon;
    [SerializeField] private Image background;
    public TextMeshProUGUI itemName;
    private Color emptyHighlightColor = Color.yellow;
    private Color swapHighlightColor = Color.orange;
    private Color firstPickedHighlightColor = Color.green;
    private Color defaultColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultColor = background.color;
        defaultColor = Color.grey;
        ClearSlot();
    }
    
    public void AddItem(ItemData item)
    {
        currentItem = item;
        if (item == null)
        {
            ClearSlot();
            return;
        }
        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = item.icon != null;
        }
        if (itemName != null)
        {
            itemName.text = item.itemName ?? "";
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        itemName.text = "";
    }

    public void EmptyHighlight()
    {
        background.color = emptyHighlightColor;
    }

    public void SwapHighlight()
    {
        background.color = swapHighlightColor;
    }

    public void FirstPickedHighlight()
    {
        background.color = firstPickedHighlightColor;
    }

    public void ResetHighlight()
    {
        background.color = defaultColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(InventoryManager.instance.isInBattle && currentItem!=null)
        {
            if (currentItem.itemName == "Key")
            {
                return;
            }
            PlayerInventoryManager.Instance.UseItem(currentItem);
            InventoryManager.instance.RefreshUI();
            InventoryManager.instance.CloseInventory();
        }
        else
        {
            InventoryManager.instance.HandleSlotClick(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null)
        {
            return;
        }
        if (InventoryManager.instance.isShop)
        {
            if (InventoryManager.instance.shopSlots.Contains(this))
            {
                InventoryManager.instance.ShowTooltip(currentItem, isShopSlot: true);
            }
            else if (InventoryManager.instance.playerSlots.Contains(this))
            {
                InventoryManager.instance.ShowTooltip(currentItem, isShopSlot: false);
            }
        }
        else
        {
            InventoryManager.instance.ShowTooltip(currentItem, isShopSlot: false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.instance.HideTooltip();
    }
}
