using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelCompleteConditions : MonoBehaviour
{
    [SerializeField] private bool keyCondition;
    [SerializeField] private bool bossCondition;
    [SerializeField] private bool goToPreviousLevel;
    [SerializeField] private bool isCastleGate;
    [SerializeField] private int targetLevel;
    [SerializeField] PlayerInventoryManager pim;
    private Health playerHealth;
    private CoinManager coinManager;
    private bool playerInRange = false;

    void Awake()
    {
        playerHealth = GameObject.Find("Player").GetComponent<Health>();
        coinManager = GameObject.Find("Player").GetComponent<CoinManager>();
        pim = GameObject.Find("Player").GetComponent<PlayerInventoryManager>();
        if (goToPreviousLevel || isCastleGate)
        {
            keyCondition = false;
            bossCondition = false;
        }
        else if (goToPreviousLevel && isCastleGate)
        {
            Debug.Log("Error! Can't be return gate and castle gate at the same time!");
        }
        else
        {
            if (targetLevel <= PlayerDataManager.instance.stats.currentLevel)
            {
                keyCondition = false;
                bossCondition = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if(isCastleGate)
            {
                UIManager.instance.ShowInteractText("Press E to enter the castle");
            }
            else if(goToPreviousLevel)
            {
                UIManager.instance.ShowInteractText("Press E to return to the previous level");
            }
            else if(targetLevel == 8)
            {
                UIManager.instance.ShowInteractText("Press E to end your journey"); 
            }
            else
            {
                UIManager.instance.ShowInteractText("Press E to enter the next level");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !UIManager.instance.isGameOver && !InventoryManager.instance.isOpen && playerInRange)
        {
            if (keyCondition && !pim.HasKey())
            {
                Debug.Log("You need a Key to proceed.");
                StartCoroutine(ShowMessageText("You need to find the key to the gate!", 5f));
                return;
            }
            if (bossCondition && !AllBossesDefeated())
            {
                Debug.Log("A boss is still alive!");
                StartCoroutine(ShowMessageText("You need to defeat the bosses in this area!", 5f));
                return;
            }
            if (isCastleGate && PlayerDataManager.instance.stats.currentLevel < 7)
            {
                Debug.Log("You need to find the key to the castle!");
                StartCoroutine(ShowMessageText("You need to find the key to the castle!", 5f));
                return;
            }
            if (!goToPreviousLevel)
            {
                if ((targetLevel > PlayerDataManager.instance.stats.currentLevel && !PlayerDataManager.instance.stats.isEndlessMode) || PlayerDataManager.instance.stats.isEndlessMode)
                {
                    PlayerDataManager.instance.stats.currentLevel++;
                }
                if (isCastleGate)
                {
                    PlayerDataManager.instance.stats.currentLevel++;
                }
                PlayerDataManager.instance.stats.playerDamage = BattleManager.instance.playerDamage;
                PlayerDataManager.instance.stats.maxHP = playerHealth.maxHP;
                PlayerDataManager.instance.stats.coins = coinManager.coins;
                PlayerDataManager.instance.LevelComplete();
                PlayerDataManager.instance.SavePlayerStats();
                TriviaQuestionRepository.instance.Save();
            }
            if (!isCastleGate)
            {
                GoToLevel(targetLevel);
            }
            else
            {
                SceneManager.LoadScene("Final Level");
            }
            if (gameObject.CompareTag("Castle Key"))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            UIManager.instance.DeactivateInteractText();
        }
    }

    private void GoToLevel(int level)
    {
        if (!PlayerDataManager.instance.stats.isEndlessMode)
        {
            if (!goToPreviousLevel)
            {
                Debug.Log("Level Complete!");
            }
            else
            {
                Debug.Log("Previous Level!");
            }
            switch (level)
            {
                case 1:
                    SceneManager.LoadScene("Tutorial");
                    break;
                case 2:
                    SceneManager.LoadScene("Level 1");
                    break;
                case 3:
                    SceneManager.LoadScene("Level 2");
                    break;
                case 4:
                    SceneManager.LoadScene("Level 3");
                    break;
                case 5:
                    SceneManager.LoadScene("Level 4");
                    break;
                case 6:
                    SceneManager.LoadScene("Castle Interior");
                    break;
                case 7:
                    Debug.Log("You got the key to the castle!");
                    StartCoroutine(ShowMessageText("You have obtained the key to the castle!\nGo back to the castle!", 5f));
                    break;
                case 8:
                    Debug.Log("Game completed!");
                    StartCoroutine(ShowMessageText("Congratulations!\nYour journey is over!", 5f));
                    PlayerDataManager.instance.stats.currentLevel = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    SceneManager.LoadScene("Main Menu");
                    break;
            }
        }
        else
        {
            Debug.Log("Level Complete!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private bool AllBossesDefeated()
    {
        return FindObjectsByType<BossTrigger>(FindObjectsSortMode.None).Length == 0;
    }

    private IEnumerator ShowMessageText(string text,float seconds)
    {
        UIManager.instance.ShowMessageText(text);
        yield return new WaitForSeconds(seconds);
        UIManager.instance.DeactivateMessageText();
    }

    public bool CheckKeyCondition()
    {
        return keyCondition;
    }

    public bool CheckBossCondition()
    {
        return bossCondition;
    }
}
