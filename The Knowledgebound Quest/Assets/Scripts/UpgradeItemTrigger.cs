using UnityEngine;
using System.Collections;

public class UpgradeItemTrigger : MonoBehaviour
{
    [SerializeField] private int upgradeType;
    [SerializeField] private int triesBeforeDamage = 3;
    private Health playerHealth;
    private float spinSpeed = 20.0f;
    private bool playerInRange = false;
    private bool isQuestionActive = false;
    public static bool isUpgradeQuestionActive = false;
    private int noOfQuestions = 3;
    private int questionsAnswered;
    private int noOfTries = 0;
    private Animator playerAnim;
    
    void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;
        playerAnim = GameObject.Find("Player").GetComponentInChildren<Animator>();
        if (!playerHealth)
        {
            playerHealth = GameObject.Find("Player").GetComponent<Health>();
        }
        TriviaLoader.instance.OnQuestionAnswered += HandleQuestionResult;
        questionsAnswered = 0;
    }

    void OnDestroy()
    {
        TriviaLoader.instance.OnQuestionAnswered -= HandleQuestionResult;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isQuestionActive)
        {
            StartNextQuestion();
        }
    }

    private void StartNextQuestion()
    {
        UIManager.instance.DeactivateInteractText();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        isQuestionActive = true;
        isUpgradeQuestionActive = true;
        TriviaLoader.instance.StartQuestion(this);
    }

    private void HandleQuestionResult(int result)
    {
        if (TriviaLoader.instance.currentRequester != this)
        {
            return;
        }
        if (result == 1)
        {
            questionsAnswered++;
            if (questionsAnswered == noOfQuestions)
            {
                isQuestionActive = false;
                isUpgradeQuestionActive = false;
                Time.timeScale = 1f;
                if (upgradeType == 1)
                {
                    HealthUpgrade();
                    Debug.Log("Health Upgrade!");
                }
                else if (upgradeType == 2)
                {
                    DamageUpgrade();
                    Debug.Log("Damage Upgrade!");
                }
                Destroy(gameObject);
                TriviaLoader.instance.currentRequester = null;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerAnim.SetTrigger("GotBuff");
                return;
            }
            TriviaLoader.instance.questionStatus = 0;
            StartNextQuestion();
        }
        else if (result == 2)
        {
            questionsAnswered = 0;
            isQuestionActive = false;
            isUpgradeQuestionActive = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            noOfTries++;
            if(noOfTries == triesBeforeDamage)
            {
                playerHealth.TakeDamage(1);
                noOfTries = 0;
            }
            Time.timeScale = 1f;
            TriviaLoader.instance.currentRequester = null;
            return;
        }
        else if (result == 0 && TriviaLoader.instance.isStalemate)
        {
            questionsAnswered = 0;
            isQuestionActive = false;
            isUpgradeQuestionActive = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            noOfTries++;
            if (noOfTries == triesBeforeDamage)
            {
                playerHealth.TakeDamage(1);
                noOfTries = 0;
            }
            Time.timeScale = 1f;
            TriviaLoader.instance.currentRequester = null;
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.ShowInteractText("Press E to obtain upgrade");
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.instance.DeactivateInteractText();
            playerInRange = false;
        }
    }

    private void HealthUpgrade()
    {
        playerHealth.maxHP++;
    }

    private void DamageUpgrade()
    {
        BattleManager.instance.playerDamage++;
    }
}
