using UnityEngine;

public class ShopInventoryManager : MonoBehaviour
{
    public Inventory shopInventory = new Inventory(4);

    void Start()
    {
        for (int i = shopInventory.items.Count - 1; i >= 0; i--)
        {
            var item = shopInventory.items[i];
            if (!PlayerDataManager.instance.stats.isEndlessMode)
            {
                item.cost = item.initialCost + (LevelManager.instance.level - 1);
            }
            else
            {
                item.cost = item.initialCost + (PlayerDataManager.instance.stats.currentLevel - 1);
            }
        }
    }
}
