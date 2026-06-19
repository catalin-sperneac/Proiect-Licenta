using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using MyGame.Trivia;

public class TriviaQuestionRepository : MonoBehaviour
{
    public static TriviaQuestionRepository instance;
    public List<QuestionBase> questions = new List<QuestionBase>();
    private string savePath;
    private string triviaFilePath;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
#endif

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "triviaQuestions.json");

        string buildFolder = Directory.GetParent(Application.dataPath).FullName;
        triviaFilePath = Path.Combine(buildFolder, "TriviaConfiguration", "Trivia.txt");

        LoadRepository();
    }

    public void LoadRepository()
    {
        List<QuestionBase> jsonQuestions = new List<QuestionBase>();

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            jsonQuestions = JsonUtility.FromJson<QuestionWrapper>(json)?.questions ?? new List<QuestionBase>();
        }

        if (!File.Exists(triviaFilePath))
        {
            ShowMissingTriviaError();
            return;
        }

        string[] lines = File.ReadAllLines(triviaFilePath);

        List<QuestionBase> newQuestions = new List<QuestionBase>();

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            string[] parts = trimmed.Split('/');

            if (parts.Length != 6 && parts.Length != 2)
                continue;

            string questionText = parts[0];

            QuestionBase existing = jsonQuestions.FirstOrDefault(q => q.questionText == questionText);

            if (parts.Length == 6)
            {
                MultipleChoiceQuestion q = new MultipleChoiceQuestion
                {
                    questionText = questionText,
                    options = new string[4] { parts[1], parts[2], parts[3], parts[4] },
                    correctAnswer = parts[5],
                    correctCount = existing?.correctCount ?? 0
                };
                newQuestions.Add(q);
            }
            else if (parts.Length == 2)
            {
                InputQuestion q = new InputQuestion
                {
                    questionText = questionText,
                    correctAnswer = parts[1],
                    correctCount = existing?.correctCount ?? 0
                };
                newQuestions.Add(q);
            }
        }
        if (newQuestions.Count == 0)
        {
            ShowInvalidTriviaError();
            return;
        }
        questions = newQuestions;

        Save();
    }

    private void ShowMissingTriviaError()
    {
        string message =
            "Trivia.txt was not found.\n\n" +
            "Please place Trivia.txt in the TriviaConfiguration folder.\n\n" +
            "Expected path:\n" + triviaFilePath;

        Debug.LogError(message);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        MessageBox(IntPtr.Zero, message, "Missing Trivia File", 0x00000010);
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowInvalidTriviaError()
    {
        string message =
            "No valid trivia questions found in Trivia.txt.\n\n" +
            "Please add questions in the text file via TriviaConfiguration application.\n\n" +
            "Expected path:\n" + triviaFilePath;

        Debug.LogError(message);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        MessageBox(IntPtr.Zero, message, "Missing Trivia File", 0x00000010);
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Save()
    {
        var wrapper = new QuestionWrapper { questions = questions };
        File.WriteAllText(savePath, JsonUtility.ToJson(wrapper, true));
    }
}

[Serializable]
class QuestionWrapper
{
    public List<QuestionBase> questions;
}
