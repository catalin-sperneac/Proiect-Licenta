using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    [SerializeField] private int coinMaxLimit;
    [SerializeField] private int coinMinLimit;
    private CoinManager coinManager;
    private Animator playerAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        coinManager = GameObject.Find("Player").GetComponent<CoinManager>();
        playerAnim = GameObject.Find("Player").GetComponentInChildren<Animator>();
        if (gameObject.CompareTag("Player"))
        {
            maxHP = PlayerDataManager.instance.stats.maxHP;
        }
        if (PlayerDataManager.instance.stats.isEndlessMode && (PlayerDataManager.instance.stats.currentLevel % 5 == 0 && PlayerDataManager.instance.stats.currentLevel > 0))
        {
            if (gameObject.CompareTag("Enemy"))
            {
                maxHP += (PlayerDataManager.instance.stats.currentLevel / 5);
            }
            else if (gameObject.CompareTag("Boss"))
            {
                maxHP += ((PlayerDataManager.instance.stats.currentLevel / 5) * 2);
            }
        }
        if (currentHP <= 0)
        {
            currentHP = maxHP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        else if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (CompareTag("Player"))
        {
            playerAnim.SetTrigger("GotHit");
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHP += healAmount;
    }

    void Die()
    {
        if (CompareTag("Enemy"))
        {
            Debug.Log("Enemy Dead");
            int randomNoOfCoins = Random.Range(coinMinLimit, coinMaxLimit);
            coinManager.coins += randomNoOfCoins;
            BattleManager.instance.BattleFinished();
            StartCoroutine(DelayedDestroy(2f));
        }
        else if (CompareTag("Boss"))
        {
            Debug.Log("Boss Dead");
            int randomNoOfCoins = Random.Range(coinMinLimit, coinMaxLimit);
            coinManager.coins += randomNoOfCoins;
            BattleManager.instance.BattleFinished();
            StartCoroutine(DelayedDestroy(2f));
        }
        else if (CompareTag("Player"))
        {
            Debug.Log("Player Dead");
            playerAnim.SetTrigger("IsDead");
            if (BattleManager.instance.isBattleActive)
            {
                BattleManager.instance.BattleFinished();
            }
        }
    }

    private IEnumerator DelayedDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
