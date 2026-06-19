using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;
using MyGame.Trivia;

public class TriviaLoader : MonoBehaviour
{
    public static TriviaLoader instance;
    [SerializeField] private GameObject questionTextBox;
    [SerializeField] private TextMeshProUGUI questionType2Hint;
    [SerializeField] private Button answer1;
    [SerializeField] private Button answer2;
    [SerializeField] private Button answer3;
    [SerializeField] private Button answer4;
    [SerializeField] private Button okButton;
    [SerializeField] private TMP_InputField playerInput;

    private QuestionBase currentQuestion;
    private TextMeshProUGUI questionText;
    public int questionStatus = 0;
    public bool isStalemate = false;
    public bool hintMode = false;
    public Action<int> OnQuestionAnswered;
    public MonoBehaviour currentRequester;
    public List<QuestionBase> questions = new List<QuestionBase>();

    void Awake()
    {
        instance = this;
        questionText = questionTextBox.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        if (TriviaQuestionRepository.instance != null)
        {
            questions = TriviaQuestionRepository.instance.questions;
        }
        questionTextBox.gameObject.SetActive(false);
        answer1.gameObject.SetActive(false);
        answer2.gameObject.SetActive(false);
        answer3.gameObject.SetActive(false);
        answer4.gameObject.SetActive(false);
        okButton.gameObject.SetActive(false);
        playerInput.gameObject.SetActive(false);
        questionType2Hint.gameObject.SetActive(false);
    }

    void Update()
    {
        if(okButton.gameObject.activeSelf && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            okButton.onClick.Invoke();
        }
    }

    public void StartQuestion(MonoBehaviour requester)
    {
        currentRequester = requester;
        currentQuestion = questions[UnityEngine.Random.Range(0, questions.Count)];
        questionStatus = 0;
        isStalemate = false;
        questionTextBox.gameObject.SetActive(true);
        questionText.text = currentQuestion.questionText;
        bool autoAnswer = !hintMode && currentQuestion.AutoAnswer();
        if (currentQuestion is MultipleChoiceQuestion mcq)
        {
            List<string> shuffledOptions = new List<string>(mcq.options);

            for (int i = 0; i < shuffledOptions.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, shuffledOptions.Count);
                string temp = shuffledOptions[i];
                shuffledOptions[i] = shuffledOptions[randomIndex];
                shuffledOptions[randomIndex] = temp;
            }

            answer1.gameObject.SetActive(true);
            answer1.GetComponentInChildren<TextMeshProUGUI>().text = shuffledOptions[0];

            answer2.gameObject.SetActive(true);
            answer2.GetComponentInChildren<TextMeshProUGUI>().text = shuffledOptions[1];

            answer3.gameObject.SetActive(true);
            answer3.GetComponentInChildren<TextMeshProUGUI>().text = shuffledOptions[2];

            answer4.gameObject.SetActive(true);
            answer4.GetComponentInChildren<TextMeshProUGUI>().text = shuffledOptions[3];

            if (autoAnswer)
            {
                if (shuffledOptions[0] == mcq.correctAnswer)
                {
                    answer1.gameObject.SetActive(true);
                    answer2.gameObject.SetActive(false);
                    answer3.gameObject.SetActive(false);
                    answer4.gameObject.SetActive(false);
                }
                if (shuffledOptions[1] == mcq.correctAnswer)
                {
                    answer1.gameObject.SetActive(false);
                    answer2.gameObject.SetActive(true);
                    answer3.gameObject.SetActive(false);
                    answer4.gameObject.SetActive(false);
                }
                if (shuffledOptions[2] == mcq.correctAnswer)
                {
                    answer1.gameObject.SetActive(false);
                    answer2.gameObject.SetActive(false);
                    answer3.gameObject.SetActive(true);
                    answer4.gameObject.SetActive(false);
                }
                if (shuffledOptions[3] == mcq.correctAnswer)
                {
                    answer1.gameObject.SetActive(false);
                    answer2.gameObject.SetActive(false);
                    answer3.gameObject.SetActive(false);
                    answer4.gameObject.SetActive(true);
                }
                mcq.ResetProgress();
            }
        }
        else if (currentQuestion is InputQuestion iq)
        {
            iq.secondIntervalPercentage = 0.1f;
            playerInput.gameObject.SetActive(true);
            okButton.gameObject.SetActive(true);
            playerInput.text = "";
            playerInput.ActivateInputField();
            playerInput.Select();
            int correctNum = int.Parse(iq.correctAnswer);
            if (!PlayerDataManager.instance.stats.isEndlessMode)
            {
                iq.secondIntervalPercentage = iq.secondIntervalPercentage - ((PlayerDataManager.instance.stats.currentLevel - 1) * 0.01f);
            }
            questionType2Hint.text = $"Hint: {(int)(correctNum - (correctNum * iq.secondIntervalPercentage))} - {(int)(correctNum + (correctNum * iq.secondIntervalPercentage))}";
            if (autoAnswer)
            {
                questionType2Hint.gameObject.SetActive(true);
                questionType2Hint.text = $"Correct answer: {iq.correctAnswer}";
                iq.ResetProgress();
            }
        }
        if (hintMode)
        {
            if (!autoAnswer)
            {
                EnableHintMode();
            }
        }
        StartCoroutine(QuestionTimer());
    }

    private IEnumerator QuestionTimer()
    {
        float timer = 0f;
        while (timer < 30f)
        {
            if (BattleManager.instance.isBattleActive)
            {
                UIManager.instance.ShowTimer(true);
            }
            string timerText = $"Timer: {(int)(30 - timer)}";
            UIManager.instance.ModifyTimerText(timerText);
            if (questionStatus != 0 || (questionStatus == 0 && isStalemate))
            {
                UIManager.instance.ShowTimer(false);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        if (questionStatus == 0 && !isStalemate)
        {
            UIManager.instance.ShowTimer(false);
            questionStatus = 2;
            CheckAnswer("99999");
        }
    }

    public void AnswerButtonClick(Button clicked)
    {
        string selected = clicked.GetComponentInChildren<TextMeshProUGUI>().text;
        CheckAnswer(selected);
    }

    public void SubmitInput()
    {
        CheckAnswer(playerInput.text);
    }

    private void CheckAnswer(string answer)
    {
        int status;
        currentQuestion.CheckAnswer(answer, out status);
        questionStatus = status;
        if (questionStatus == 0)
        {
            Debug.Log("Stalemate");
            isStalemate = true;
        }
        else if (questionStatus == 1)
        {
            Debug.Log("Correct Answer");
        }
        else if (questionStatus == 2)
        {
            Debug.Log("Wrong Answer");
        }
        HideQuestionUI();
        OnQuestionAnswered?.Invoke(questionStatus);
    }

    private void HideQuestionUI()
    {
        questionTextBox.gameObject.SetActive(false);
        answer1.gameObject.SetActive(false);
        answer2.gameObject.SetActive(false);
        answer3.gameObject.SetActive(false);
        answer4.gameObject.SetActive(false);
        okButton.gameObject.SetActive(false);
        playerInput.gameObject.SetActive(false);
        playerInput.text = "";
        questionType2Hint.gameObject.SetActive(false);
    }

    public void EnableHintMode()
    {
        if (currentQuestion is MultipleChoiceQuestion mcq)
        {
            string correct = mcq.correctAnswer;
            string t1 = answer1.GetComponentInChildren<TextMeshProUGUI>().text;
            string t2 = answer2.GetComponentInChildren<TextMeshProUGUI>().text;
            string t3 = answer3.GetComponentInChildren<TextMeshProUGUI>().text;
            string t4 = answer4.GetComponentInChildren<TextMeshProUGUI>().text;
            if (t1 == correct || t2 == correct)
            {
                answer3.gameObject.SetActive(false);
                answer4.gameObject.SetActive(false);
            }
            else
            {
                answer1.gameObject.SetActive(false);
                answer2.gameObject.SetActive(false);
            }
        }
        else if (currentQuestion is InputQuestion)
        {
            questionType2Hint.gameObject.SetActive(true);
        }
        hintMode = false;
    }
}