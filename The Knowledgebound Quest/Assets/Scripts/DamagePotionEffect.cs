using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/DamagePotion")]
public class DamagePotionEffect : ItemEffect
{
    public float multiplier = 2f;

    public override void UseItem(BattleManager battleManager)
    {
        battleManager.ApplyDamageBuff(multiplier);
    }
}
