using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int coins;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coins = PlayerDataManager.instance.stats.coins;
    }
}
