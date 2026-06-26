using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    [SerializeField] private Button AttackButton;
    [SerializeField] private Button ItemButton;
    [SerializeField] private Button RunButton;
    [SerializeField] private TextMeshProUGUI enemyHealthText;
    [SerializeField] private Button GoBackButton;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private PlayerInventoryManager pim;
    private Health enemyHealth;
    private Transform playerPos;
    private Transform enemyPos;
    public int playerDamage;
    private Animator playerAnim;
    private Animator enemyAnim;
    private float spawnRate;
    private bool hardDifficulty = false;
    public bool isBattleActive = false;
    public bool battleCooldown = false;
    private bool isQuestionActive = false;
    [SerializeField] private Camera battleCamera;
    [SerializeField] private Camera mainCamera;
    private bool hasDamageBuff = false;
    private float damageMultiplier = 1f;
    private bool hasIntelligenceBuff = false;
    private Vector3 previousBossPosition;
    private Quaternion previousBossRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        if (!playerHealth)
        {
            playerHealth = GameObject.Find("Player").GetComponent<Health>();
        }
        if (!pim)
        {
            pim = FindFirstObjectByType<PlayerInventoryManager>();
        }
        playerDamage = PlayerDataManager.instance.stats.playerDamage;
        hardDifficulty = PlayerDataManager.instance.stats.isHardMode;
        playerAnim = GameObject.Find("Battle Arena/Player Model").GetComponent<Animator>();
        playerPos = GameObject.Find("Battle Arena/Player Model").transform;
    }

    void Start()
    {
        TriviaLoader.instance.OnQuestionAnswered += HandleQuestionResult;
        mainCamera.gameObject.SetActive(true);
        battleCamera.gameObject.SetActive(false);
        if (!hardDifficulty)
        {
            spawnRate = 0.1f;
        }
        else
        {
            spawnRate = 0.2f;
        }
        AttackButton.gameObject.SetActive(false);
        ItemButton.gameObject.SetActive(false);
        RunButton.gameObject.SetActive(false);
        enemyHealthText.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isBattleActive && isQuestionActive)
        {
            AttackButton.gameObject.SetActive(false);
            ItemButton.gameObject.SetActive(false);
            RunButton.gameObject.SetActive(false);
        }
        if(enemyHealth!=null)
        {
            enemyHealthText.text = $"Enemy: {enemyHealth.currentHP}";
        }
    }

    public void ActivateEnemySpawn()
    {
        if (isBattleActive)
        {
            return;
        }
        if (Random.value <= spawnRate)
        {
            StartBattle();
        }
    }

    private void StartBattle()
    {
        isBattleActive = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(true);
        AttackButton.gameObject.SetActive(true);
        ItemButton.gameObject.SetActive(true);
        RunButton.gameObject.SetActive(true);
        enemyHealthText.gameObject.SetActive(true);
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];
        GameObject spawnedEnemy = Instantiate(enemyPrefab, enemySpawnPoint.position + new Vector3(0,1,3), enemySpawnPoint.rotation * Quaternion.Euler(0f,180f,0f));
        if(spawnedEnemy.layer == LayerMask.NameToLayer("Special Enemy"))
        {
            spawnedEnemy.transform.rotation = spawnedEnemy.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }
        enemyHealth = spawnedEnemy.GetComponent<Health>();
        enemyAnim = spawnedEnemy.GetComponent<Animator>();
        enemyPos = spawnedEnemy.transform;
        playerAnim.SetBool("InCombat", true);
    }

    public void StartBossBattle(GameObject boss)
    {
        isBattleActive = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(true);
        AttackButton.gameObject.SetActive(true);
        ItemButton.gameObject.SetActive(true);
        RunButton.gameObject.SetActive(true);
        enemyHealthText.gameObject.SetActive(true);
        enemyHealth = boss.GetComponent<Health>();
        enemyAnim = boss.GetComponentInChildren<Animator>();
        if(enemyAnim == null)
        {
            enemyAnim = boss.GetComponent<Animator>();
        }
        previousBossPosition = boss.transform.position;
        previousBossRotation = boss.transform.rotation;
        boss.transform.position = enemySpawnPoint.position + new Vector3(0, 1, 3);
        boss.transform.rotation = enemySpawnPoint.rotation * Quaternion.Euler(0f,180f,0f);
        enemyPos = boss.transform;
        playerAnim.SetBool("InCombat", true);
    }

    public void RunFromBattle()
    {
        if (Random.value <= 0.3)
        {
            isBattleActive = false;
            battleCooldown = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            mainCamera.gameObject.SetActive(true);
            battleCamera.gameObject.SetActive(false);
            AttackButton.gameObject.SetActive(false);
            ItemButton.gameObject.SetActive(false);
            RunButton.gameObject.SetActive(false);
            GoBackButton.gameObject.SetActive(false);
            enemyHealthText.gameObject.SetActive(false);
            if (enemyHealth.CompareTag("Enemy"))
            {
                Destroy(enemyHealth.gameObject);
            }
            else if (enemyHealth.CompareTag("Boss"))
            {
                enemyHealth.transform.position = previousBossPosition;
                enemyHealth.transform.rotation = previousBossRotation;
            }
            enemyHealth = null;
            playerAnim.SetBool("InCombat", false);
			StartCoroutine(BattleCooldown(2f));
        }
        else
        {
            playerHealth.TakeDamage(1);
            playerAnim.SetTrigger("GotHit");
            StartCoroutine(HideBattleUI(1f));
        }
    }

    public void UseItem()
    {
        AttackButton.gameObject.SetActive(false);
        ItemButton.gameObject.SetActive(false);
        RunButton.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(true);
        InventoryManager.instance.OpenInventory(pim.playerInventory, null, true);
    }

    public void AttackEnemy()
    {
        GoBackButton.gameObject.SetActive(false);
        isQuestionActive = true;
        TriviaLoader.instance.StartQuestion(this);
    }

    private void HandleQuestionResult(int result)
    {
        if (TriviaLoader.instance.currentRequester != this)
        {
            return;
        }
        isQuestionActive = false;
        if (result == 1)
        {
            playerAnim.SetTrigger("IsAttacking");
            playerPos.position = playerPos.position + new Vector3(0f, 0f, 4f);
            if (enemyAnim.gameObject.CompareTag("Enemy"))
            {
                enemyAnim.SetTrigger("Enemy_GotHit");
            }
            else if(enemyAnim.gameObject.CompareTag("Boss"))
            {
                enemyAnim.SetTrigger("Boss_GotHit");
            }
            int damage = Mathf.RoundToInt(playerDamage * damageMultiplier);
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            TriviaLoader.instance.currentRequester = null;
            StartCoroutine(MovePlayer(1f));
        }
        else if (result == 2)
        {
            if (playerHealth != null)
            {
                enemyPos.position = enemyPos.position + new Vector3(0f, 0f, -4f);
                playerAnim.SetTrigger("GotHit");
                if (enemyAnim.gameObject.CompareTag("Enemy"))
                {
                    enemyAnim.SetTrigger("Enemy_IsAttacking");
                }
                else if (enemyAnim.gameObject.CompareTag("Boss"))
                {
                    enemyAnim.SetTrigger("Boss_IsAttacking");
                }
                if (!hardDifficulty)
                {
                    
                    playerHealth.TakeDamage(1);
                }
                else
                {
                    playerHealth.TakeDamage(2);
                }
            }
            TriviaLoader.instance.currentRequester = null;
            StartCoroutine(MoveEnemy(1f));
        }
		ResetTemporaryEffects();
        StartCoroutine(HideBattleUI(1f));
    }

    public void BattleFinished()
    {
        isBattleActive = false;
        battleCooldown = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerHealth.currentHP == 0)
        {
            playerAnim.SetTrigger("IsDead");
        }
        else
        {
            if (enemyAnim.gameObject.CompareTag("Enemy"))
            {
                enemyAnim.SetTrigger("Enemy_IsDead");
            }
            else if (enemyAnim.gameObject.CompareTag("Boss"))
            {
                enemyAnim.SetTrigger("Boss_IsDead");
            }
        }
        playerAnim.SetBool("InCombat", false);
        StartCoroutine(HideBattleUI(3f));
        enemyHealth = null;
        enemyAnim = null;
    }

    public void GoBackFromInventory()
    {
        AttackButton.gameObject.SetActive(true);
        ItemButton.gameObject.SetActive(true);
        RunButton.gameObject.SetActive(true);
        GoBackButton.gameObject.SetActive(false);
        InventoryManager.instance.CloseInventory();
    }

    private IEnumerator HideBattleUI(float seconds)
    {
        AttackButton.gameObject.SetActive(false);
        ItemButton.gameObject.SetActive(false);
        RunButton.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(seconds);
        if (isBattleActive)
        {
            AttackButton.gameObject.SetActive(true);
            ItemButton.gameObject.SetActive(true);
            RunButton.gameObject.SetActive(true);
        }
        else
        {
            mainCamera.gameObject.SetActive(true);
            battleCamera.gameObject.SetActive(false);
            enemyHealthText.gameObject.SetActive(false);
            StartCoroutine(BattleCooldown(2f));
        }
    }

    private IEnumerator BattleCooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        battleCooldown = false;
    }

    private IEnumerator MovePlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        playerPos.position = playerPos.position + new Vector3(0f, 0f, -4f);
    }

    private IEnumerator MoveEnemy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enemyPos.position = enemyPos.position + new Vector3(0f, 0f, 4f);
    }

    public void PlayerHeal(int amount)
    {
        playerHealth.Heal(amount);
        playerAnim.SetTrigger("GotBuff");
        Debug.Log($"Player healed for {amount} HP!");
    }

    public void ApplyDamageBuff(float multiplier)
    {
        hasDamageBuff = true;
        damageMultiplier = multiplier;
        playerAnim.SetTrigger("GotBuff");
        Debug.Log($"Damage buff applied! Next attack deals {multiplier}x damage.");
    }

    public void ApplyIntelligenceBuff()
    {
        hasIntelligenceBuff = true;
        TriviaLoader.instance.hintMode = true;
        playerAnim.SetTrigger("GotBuff");
        Debug.Log("Intelligence potion used! Hints enabled for next question.");
    }

    private void ResetTemporaryEffects()
    {
        if (hasDamageBuff)
        {
            hasDamageBuff = false;
            damageMultiplier = 1f;
        }

        if (hasIntelligenceBuff)
        {
            hasIntelligenceBuff = false;
            TriviaLoader.instance.hintMode = false;
        }
    }
}
