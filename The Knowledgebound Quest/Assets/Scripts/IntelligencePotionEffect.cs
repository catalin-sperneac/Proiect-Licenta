using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/IntelligencePotion")]
public class IntelligencePotionEffect : ItemEffect
{
    public override void UseItem(BattleManager battleManager)
    {
        battleManager.ApplyIntelligenceBuff();
    }
}
