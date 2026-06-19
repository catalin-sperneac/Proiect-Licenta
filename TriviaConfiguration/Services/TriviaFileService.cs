using System;
using System.Collections.Generic;
using System.IO;
using TriviaConfiguration.Models;

namespace TriviaConfiguration.Services;

public static class TriviaFileService
{
    public static string FilePath => Path.Combine(AppContext.BaseDirectory, "Trivia.txt");

    public static bool FileExists()
    {
        return File.Exists(FilePath);
    }

    public static List<TriviaQuestionBase> LoadQuestions()
    {
        var questions = new List<TriviaQuestionBase>();

        if (!FileExists())
            return questions;

        var lines = File.ReadAllLines(FilePath);

        foreach (var rawLine in lines)
        {
            if (string.IsNullOrWhiteSpace(rawLine))
                continue;

            var parts = rawLine.Split('/');

            if (parts.Length == 2)
            {
                questions.Add(new InputQuestion
                {
                    QuestionText = parts[0],
                    CorrectAnswer = parts[1]
                });
            }
            else if (parts.Length == 6)
            {
                questions.Add(new MultipleChoiceQuestion
                {
                    QuestionText = parts[0],
                    Answers = new List<string> { parts[1], parts[2], parts[3], parts[4] },
                    CorrectAnswer = parts[5]
                });
            }
        }

        return questions;
    }

    public static void SaveQuestions(List<TriviaQuestionBase> questions)
    {
        var lines = new List<string>();

        foreach (var q in questions)
        {
            if (q is InputQuestion iq)
            {
                lines.Add($"{iq.QuestionText}/{iq.CorrectAnswer}");
            }
            else if (q is MultipleChoiceQuestion mcq && mcq.Answers.Count >= 4)
            {
                lines.Add($"{q.QuestionText}/{mcq.Answers[0]}/{mcq.Answers[1]}/{mcq.Answers[2]}/{mcq.Answers[3]}/{mcq.CorrectAnswer}");
            }
        }

        File.WriteAllLines(FilePath, lines);
    }

    public static void SaveQuestionsFromString(string rawText)
    {
        var path = FilePath;

        var lines = rawText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        File.WriteAllLines(path, lines);
    }

   public static void AppendQuestionsFromString(string rawText)
    {
        var path = FilePath;

        var lines = rawText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        if (!File.Exists(FilePath))
        {
            File.WriteAllLines(FilePath, lines);
            return;
        }

        // Otherwise append
        File.AppendAllLines(path, lines);
    }
}