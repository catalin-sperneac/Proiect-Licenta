using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/HealthPotion")]
public class HealthPotionEffect : ItemEffect
{
    private Health playerHealth;
    private int healAmount;

    public override void UseItem(BattleManager battleManager)
    {
        if (!playerHealth)
        {
            playerHealth = GameObject.Find("Player").GetComponent<Health>();
        }
        healAmount = playerHealth.maxHP;
        battleManager.PlayerHeal(healAmount);
    }
}
