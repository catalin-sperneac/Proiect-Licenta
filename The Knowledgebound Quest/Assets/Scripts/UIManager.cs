using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI dangerText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI interactText;
    public bool isGameOver = false;
    private PlayerControls playerControls;
    private Health playerHealth;
    private CoinManager coinManager;

    void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!playerHealth)
        {
            playerHealth = GameObject.Find("Player").GetComponent<Health>();
        }
        if (!playerControls)
        {
            playerControls = GameObject.Find("Player").GetComponent<PlayerControls>();
        }
        if(!coinManager)
        {
            coinManager = GameObject.Find("Player").GetComponent<CoinManager>();
        }
        gameOverText.gameObject.SetActive(false);
        dangerText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        if(PlayerDataManager.instance.stats.isEndlessMode)
        {
            levelText.gameObject.SetActive(true);
        }
        else
        {
            levelText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = $"Health: {playerHealth.currentHP} / {playerHealth.maxHP}";
        staminaText.text = $"Stamina: {(int)playerControls.currentStamina} / {playerControls.maxStamina}";
        coinText.text = $"Coins: {coinManager.coins}";
        levelText.text = $"Level: {PlayerDataManager.instance.stats.currentLevel}";
        if(playerHealth.currentHP == 0 || (playerHealth.gameObject.transform.position.y <= -30f && PlayerDataManager.instance.stats.isEndlessMode))
        {
            isGameOver = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            healthText.gameObject.SetActive(false);
            staminaText.gameObject.SetActive(false);
            levelText.gameObject.SetActive(false);
            coinText.gameObject.SetActive(false);
            gameOverText.gameObject.SetActive(true);
            if(!PlayerDataManager.instance.stats.isEndlessMode)
            {
                restartButton.gameObject.SetActive(true);
            }
            else
            {
                mainMenuButton.gameObject.SetActive(true);
                PlayerDataManager.instance.ResetPlayerStats();
                PlayerDataManager.instance.ResetSavedInventory();
            }
        }
        if(SpawnTrigger.playerInsideCount > 0 && !BattleManager.instance.isBattleActive)
        {
            dangerText.gameObject.SetActive(true);
        }
        else
        {
            dangerText.gameObject.SetActive(false);
        }
        if(BattleManager.instance.isBattleActive || InventoryManager.instance.isOpen || MenuManager.instance.isMapActive || MenuManager.instance.isMenuActive || isGameOver)
        {
            interactText.gameObject.SetActive(false);
            dangerText.gameObject.SetActive(false);
            messageText.gameObject.SetActive(false);
        }
    }


    public void ShowDangerText(bool show)
    {
        dangerText.gameObject.SetActive(show);
    }

    public void ShowMessageText(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);
    }

    public void DeactivateMessageText()
    {
        messageText.gameObject.SetActive(false);
    }

    public void ShowInteractText(string text)
    {
        interactText.text = text;
        interactText.gameObject.SetActive(true);
    }

    public void DeactivateInteractText()
    {
        interactText.gameObject.SetActive(false);
    }

    public void ShowTimer(bool show)
    {
        timerText.gameObject.SetActive(show);
    }

    public void ModifyTimerText(string text)
    {
        timerText.text = text;
    }

    public void ShowStatsText(bool show)
    {
        healthText.gameObject.SetActive(show);
        staminaText.gameObject.SetActive(show);
        coinText.gameObject.SetActive(show);
        if (PlayerDataManager.instance.stats.isEndlessMode)
        {
            levelText.gameObject.SetActive(show);
        }
    }
}
