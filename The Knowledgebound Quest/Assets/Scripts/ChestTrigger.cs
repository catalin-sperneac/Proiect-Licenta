using UnityEngine;
using System.Collections;

public class ChestTrigger : MonoBehaviour
{
    public ChestInventoryManager chestInventory;
    private CoinManager coinManager;
    [SerializeField] private int coinMinLimit;
    [SerializeField] private int coinMaxLimit;
    [SerializeField] private int triesBeforeDamage = 3;
    private bool playerInRange = false;
    private bool isChestUnlocked = false;
    private bool isQuestionActive = false;
    public static bool isChestQuestionActive = false;
    private Health playerHealth;
    private int noOfTries = 0;

    void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;

        coinManager = GameObject.Find("Player").GetComponent<CoinManager>();
        playerHealth = GameObject.Find("Player").GetComponent<Health>();
        TriviaLoader.instance.OnQuestionAnswered += HandleQuestionResult;
    }

    void OnDestroy()
    {
        TriviaLoader.instance.OnQuestionAnswered -= HandleQuestionResult;
    }

    void Update()
    {
        if(!playerInRange)
        {
            return;
        }
        if(InventoryManager.instance.isOpen)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && !isQuestionActive && !InventoryManager.instance.blockInteraction && !UIManager.instance.isGameOver)
        {
            if (!isChestUnlocked)
            {
                UIManager.instance.DeactivateInteractText();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                isQuestionActive = true;
                isChestQuestionActive = true;
                TriviaLoader.instance.StartQuestion(this);
            }
            else
            {
                InventoryManager.instance.OpenInventory(PlayerInventoryManager.Instance.playerInventory, chestInventory.chestInventory);
            }
        }
    }

    private void HandleQuestionResult(int result)
    {
        if(TriviaLoader.instance.currentRequester != this)
        {
            return;
        }
        isQuestionActive = false;
        isChestQuestionActive = false;
        Time.timeScale = 1f;
        if (result == 1)
        {
            isChestUnlocked = true;
            int randomNoOfCoins = Random.Range(coinMinLimit, coinMaxLimit);
            coinManager.coins += randomNoOfCoins;
            InventoryManager.instance.OpenInventory(PlayerInventoryManager.Instance.playerInventory, chestInventory.chestInventory);
            TriviaLoader.instance.currentRequester = null;
        }
        else if (result == 2)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            noOfTries++;
            if( noOfTries == triesBeforeDamage)
            {
                playerHealth.TakeDamage(1);
                noOfTries = 0;
            }
            TriviaLoader.instance.currentRequester = null;
            return;
        }
        else if (result == 0 && TriviaLoader.instance.isStalemate)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            noOfTries++;
            if (noOfTries == triesBeforeDamage)
            {
                playerHealth.TakeDamage(1);
                noOfTries = 0;
            }
            TriviaLoader.instance.currentRequester = null;
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isChestUnlocked)
            {
                UIManager.instance.ShowInteractText("press E to open chest");
            }
            else
            {
                UIManager.instance.ShowInteractText("press E to unlock chest");
            }
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.DeactivateInteractText();
            playerInRange = false;
            InventoryManager.instance.CloseInventory();
        }
    }
}
