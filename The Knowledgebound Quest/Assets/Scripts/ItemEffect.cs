using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract void UseItem(BattleManager battleManager);
}
