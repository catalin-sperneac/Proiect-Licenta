using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] private Button MainMenuButton;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button ControlsButton;
    [SerializeField] private Button HelpButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Image GameTitle;
    [SerializeField] private Button GoBackButton;
    [SerializeField] private TextMeshProUGUI ControlsText;
    [SerializeField] private TextMeshProUGUI HelpText;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera mapCamera;
    public bool isMapActive;
    public bool isMenuActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        GoBackButton.gameObject.SetActive(false);
        ControlsText.gameObject.SetActive(false);
        HelpText.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        ControlsButton.gameObject.SetActive(false);
        HelpButton.gameObject.SetActive(false);
        GameTitle.gameObject.SetActive(false);
        isMenuActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !BattleManager.instance.isBattleActive && !InventoryManager.instance.isOpen && !UIManager.instance.isGameOver && !ChestTrigger.isChestQuestionActive && !UpgradeItemTrigger.isUpgradeQuestionActive)
        {
            if(!isMenuActive)
            {
                GoBackButton.gameObject.SetActive(false);
                ControlsText.gameObject.SetActive(false);
                HelpText.gameObject.SetActive(false);
                QuitButton.gameObject.SetActive(true);
                MainMenuButton.gameObject.SetActive(true);
                if (!PlayerDataManager.instance.stats.isEndlessMode)
                {
                    RestartButton.gameObject.SetActive(true);
                }
                else
                {
                    RestartButton.gameObject.SetActive(false);
                }
                ControlsButton.gameObject.SetActive(true);
                HelpButton.gameObject.SetActive(true);
                GameTitle.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isMenuActive = true;
                UIManager.instance.ShowStatsText(false);
                Time.timeScale = 0f;
            }
            else
            {
                GoBackButton.gameObject.SetActive(false);
                ControlsText.gameObject.SetActive(false);
                HelpText.gameObject.SetActive(false);
                QuitButton.gameObject.SetActive(false);
                MainMenuButton.gameObject.SetActive(false);
                RestartButton.gameObject.SetActive(false);
                ControlsButton.gameObject.SetActive(false);
                HelpButton.gameObject.SetActive(false);
                GameTitle.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isMenuActive = false;
                UIManager.instance.ShowStatsText(true);
                Time.timeScale = 1f;
            }
        }
        if(!isMenuActive && !BattleManager.instance.isBattleActive && !InventoryManager.instance.isOpen && !UIManager.instance.isGameOver && !ChestTrigger.isChestQuestionActive && !UpgradeItemTrigger.isUpgradeQuestionActive && LevelManager.instance.level != 6)
        {
            if(Input.GetKey(KeyCode.M))
            {
                mainCamera.gameObject.SetActive(false);
                mapCamera.gameObject.SetActive(true);
                isMapActive = true;
                UIManager.instance.ShowStatsText(false);
                Time.timeScale = 0f;
            }
            else
            {
                mainCamera.gameObject.SetActive(true);
                mapCamera.gameObject.SetActive(false);
                isMapActive = false;
                UIManager.instance.ShowStatsText(true);
                Time.timeScale = 1f;
            }
        }
    }

    public void QuitGame()
    {
        PlayerDataManager.instance.SavePlayerStats();
        PlayerDataManager.instance.ResetTempInventory();
        Application.Quit();
    }

    public void ControlsMenu()
    {
        GameTitle.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        ControlsButton.gameObject.SetActive(false);
        HelpButton.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(true);
        ControlsText.gameObject.SetActive(true);
    }

    public void HelpMenu()
    {
        GameTitle.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        ControlsButton.gameObject.SetActive(false);
        HelpButton.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(true);
        HelpText.gameObject.SetActive(true);
    }

    public void GoBack()
    {
        GoBackButton.gameObject.SetActive(false);
        ControlsText.gameObject.SetActive(false);
        HelpText.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(true);
        MainMenuButton.gameObject.SetActive(true);
        if (!PlayerDataManager.instance.stats.isEndlessMode)
        {
            RestartButton.gameObject.SetActive(true);
        }
        else
        {
            RestartButton.gameObject.SetActive(false);
        }
        ControlsButton.gameObject.SetActive(true);
        HelpButton.gameObject.SetActive(true);
        GameTitle.gameObject.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerDataManager.instance.SavePlayerStats();
        PlayerDataManager.instance.ResetTempInventory();
        SceneManager.LoadScene("Main Menu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        PlayerDataManager.instance.ResetTempInventory();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
