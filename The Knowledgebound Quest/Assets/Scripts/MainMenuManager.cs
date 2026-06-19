using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button NewGameButton;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private Button ContinueEndlessButton;
    [SerializeField] private Button ControlsButton;
    [SerializeField] private Button HelpButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Image GameTitle;
    [SerializeField] private Button MediumButton;
    [SerializeField] private Button EndlessButton;
    [SerializeField] private Button HardButton;
    [SerializeField] private TextMeshProUGUI HardDifficultyText;
    [SerializeField] private TextMeshProUGUI EndlessModeText;
    [SerializeField] private Button GoBackButton;
    [SerializeField] private TextMeshProUGUI ControlsText;
    [SerializeField] private TextMeshProUGUI HelpText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupMainMenuUI();
    }

    private void SetupMainMenuUI()
    {
        MediumButton.gameObject.SetActive(false);
        HardButton.gameObject.SetActive(false);
        HardDifficultyText.gameObject.SetActive(false);
        EndlessModeText.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(false);
        ControlsText.gameObject.SetActive(false);
        HelpText.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(true);
        NewGameButton.gameObject.SetActive(true);
        ControlsButton.gameObject.SetActive(true);
        HelpButton.gameObject.SetActive(true);
        GameTitle.gameObject.SetActive(true);
        ContinueButton.gameObject.SetActive(PlayerDataManager.instance.stats.currentLevel > 0 && !PlayerDataManager.instance.stats.isEndlessMode);
        ContinueEndlessButton.gameObject.SetActive(PlayerDataManager.instance.stats.currentLevel > 0 && PlayerDataManager.instance.stats.isEndlessMode);
    }

    void Update()
    {
        Camera.main.transform.Rotate(Vector3.up, 5 * Time.deltaTime, Space.World);
    }

    public void QuitGame()
    {
        PlayerDataManager.instance.SavePlayerStats();
        Application.Quit();
    }

    public void SelectNewGame()
    {
        NewGameButton.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
        ContinueEndlessButton.gameObject.SetActive(false);
        ControlsButton.gameObject.SetActive(false);
        HelpButton.gameObject.SetActive(false);
        MediumButton.gameObject.SetActive(true);
        HardButton.gameObject.SetActive(true);
        EndlessButton.gameObject.SetActive(true);
        HardDifficultyText.gameObject.SetActive(true);
        EndlessModeText.gameObject.SetActive(true);
        GoBackButton.gameObject.SetActive(true);
    }

    public void ContinueGame()
    {
        switch(PlayerDataManager.instance.stats.currentLevel)
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
                SceneManager.LoadScene("Castle Interior");
                break;
            case 8:
                SceneManager.LoadScene("Final Level");
                break;
        }
    }

    public void ContinueEndlessGame()
    {
        SceneManager.LoadScene("Endless");
    }

    public void StartNewGameMedium()
    {
        PlayerDataManager.instance.ResetPlayerStats();
        PlayerDataManager.instance.ResetSavedInventory();
        PlayerDataManager.instance.stats.currentLevel = 1;
        PlayerDataManager.instance.stats.isHardMode = false;
        PlayerDataManager.instance.stats.isEndlessMode = false;
        TriviaQuestionRepository.instance.LoadRepository();
        foreach (var q in TriviaQuestionRepository.instance.questions)
        {
            q.ResetProgress();
        }
        TriviaQuestionRepository.instance.Save();
        SceneManager.LoadScene("Tutorial");
    }

    public void StartNewGameHard()
    {
        PlayerDataManager.instance.ResetPlayerStats();
        PlayerDataManager.instance.ResetSavedInventory();
        PlayerDataManager.instance.stats.currentLevel = 1;
        PlayerDataManager.instance.stats.isHardMode = true;
        PlayerDataManager.instance.stats.isEndlessMode = false;
        TriviaQuestionRepository.instance.LoadRepository();
        foreach (var q in TriviaQuestionRepository.instance.questions)
        {
            q.ResetProgress();
        }
        TriviaQuestionRepository.instance.Save();
        SceneManager.LoadScene("Tutorial");
    }

    public void StartNewGameEndless()
    {
        PlayerDataManager.instance.ResetPlayerStats();
        PlayerDataManager.instance.ResetSavedInventory();
        PlayerDataManager.instance.stats.currentLevel = 1;
        PlayerDataManager.instance.stats.isHardMode = false;
        PlayerDataManager.instance.stats.isEndlessMode = true;
        TriviaQuestionRepository.instance.LoadRepository();
        foreach (var q in TriviaQuestionRepository.instance.questions)
        {
            q.ResetProgress();
        }
        TriviaQuestionRepository.instance.Save();
        SceneManager.LoadScene("Endless");
    }

    public void ControlsMenu()
    {
        GameTitle.gameObject.SetActive(false);
        NewGameButton.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
        ContinueEndlessButton.gameObject.SetActive(false);
        ControlsButton.gameObject.SetActive(false);
        HelpButton.gameObject.SetActive(false);
        MediumButton.gameObject.SetActive(false);
        EndlessButton.gameObject.SetActive(false);
        HardButton.gameObject.SetActive(false);
        HardDifficultyText.gameObject.SetActive(false);
        EndlessModeText.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(true);
        ControlsText.gameObject.SetActive(true);
    }

    public void HelpMenu()
    {
        GameTitle.gameObject.SetActive(false);
        NewGameButton.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
        ContinueEndlessButton.gameObject.SetActive(false);
        ControlsButton.gameObject.SetActive(false);
        HelpButton.gameObject.SetActive(false);
        MediumButton.gameObject.SetActive(false);
        EndlessButton.gameObject.SetActive(false);
        HardButton.gameObject.SetActive(false);
        HardDifficultyText.gameObject.SetActive(false);
        EndlessModeText.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(true);
        HelpText.gameObject.SetActive(true);
    }

    public void GoBack()
    {
        MediumButton.gameObject.SetActive(false);
        EndlessButton.gameObject.SetActive(false);
        HardButton.gameObject.SetActive(false);
        HardDifficultyText.gameObject.SetActive(false);
        EndlessModeText.gameObject.SetActive(false);
        GoBackButton.gameObject.SetActive(false);
        ControlsText.gameObject.SetActive(false);
        HelpText.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(true);
        NewGameButton.gameObject.SetActive(true);
        ControlsButton.gameObject.SetActive(true);
        HelpButton.gameObject.SetActive(true);
        GameTitle.gameObject.SetActive(true);
        ContinueButton.gameObject.SetActive(PlayerDataManager.instance.stats.currentLevel > 0 && !PlayerDataManager.instance.stats.isEndlessMode);
        ContinueEndlessButton.gameObject.SetActive(PlayerDataManager.instance.stats.currentLevel > 0 && PlayerDataManager.instance.stats.isEndlessMode);
    }
}
