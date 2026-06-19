using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemType type;
    public ItemEffect effect;
    public int initialCost = 5;
    public int cost;
}

public enum ItemType
{
    Key,
    HealthPotion,
    DamagePotion,
    IntelligencePotion
}