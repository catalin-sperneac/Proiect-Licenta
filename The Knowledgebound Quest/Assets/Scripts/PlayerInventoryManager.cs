using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance;
    public Inventory playerInventory = new Inventory(8);

    void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void UseItem(ItemData item)
    {
        if(BattleManager.instance.isBattleActive)
        {
            if (item.effect != null)
            {
                item.effect.UseItem(BattleManager.instance);
                playerInventory.RemoveItem(item);
                BattleManager.instance.AttackEnemy();
            }
        }
    }

    public bool HasKey()
    {
        foreach (var item in playerInventory.items)
        {
            if (item != null && item.itemName == "Key")
            {
                return true;
            }
        }
        return false;
    }
}
