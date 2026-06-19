using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using TriviaConfiguration.Models;
using TriviaConfiguration.Services;

namespace TriviaConfiguration.Views;

public partial class ModifyQuestionWindow : Window
{
    private readonly MainWindow _mainWindow;
    private readonly TriviaQuestionBase _question;

    public ModifyQuestionWindow(MainWindow mainWindow, TriviaQuestionBase question)
    {
        _mainWindow = mainWindow;
        _question = question;

        InitializeComponent();
        LoadQuestion();
    }

    private void LoadQuestion()
    {
        QuestionTextBox.Text = _question.QuestionText;

        if (_question is InputQuestion iq)
        {
            InputPanel.IsVisible = true;
            MultipleChoicePanel.IsVisible = false;

            CorrectAnswerTextBox.Text = iq.CorrectAnswer;
        }
        else if (_question is MultipleChoiceQuestion mcq)
        {
            InputPanel.IsVisible = false;
            MultipleChoicePanel.IsVisible = true;

            while (mcq.Answers.Count < 4)
                mcq.Answers.Add("");

            Answer1TextBox.Text = mcq.Answers[0];
            Answer2TextBox.Text = mcq.Answers[1];
            Answer3TextBox.Text = mcq.Answers[2];
            Answer4TextBox.Text = mcq.Answers[3];
        }
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void ApplyButton_Click(object? sender, RoutedEventArgs e)
    {
        var questionText = QuestionTextBox.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(questionText))
        {
            await ShowMessage("Please enter the question text.");
            return;
        }

        if (_question is InputQuestion iq)
        {
            var correct = CorrectAnswerTextBox.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(correct))
            {
                await ShowMessage("Please enter the correct answer.");
                return;
            }

            if (!IsPositiveInteger(correct))
            {
                await ShowMessage("Correct answer must be a non-negative integer (0 allowed).");
                return;
            }

            iq.QuestionText = questionText;
            iq.CorrectAnswer = correct;
        }
        else if (_question is MultipleChoiceQuestion mcq)
        {
            var a1 = Answer1TextBox.Text?.Trim() ?? "";
            var a2 = Answer2TextBox.Text?.Trim() ?? "";
            var a3 = Answer3TextBox.Text?.Trim() ?? "";
            var a4 = Answer4TextBox.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(a1) ||
                string.IsNullOrWhiteSpace(a2) ||
                string.IsNullOrWhiteSpace(a3) ||
                string.IsNullOrWhiteSpace(a4))
            {
                await ShowMessage("Please fill all answers.");
                return;
            }

            if (a2 == a1 || a3 == a1 || a4 == a1)
            {
                await ShowMessage("Answer 2, Answer 3, and Answer 4 must be different from the correct answer.");
                return;
            }

            if (a2 == a3 || a2 == a4 || a3 == a4)
            {
                await ShowMessage("Answer 2, Answer 3, and Answer 4 must all be different from each other.");
                return;
            }

            mcq.QuestionText = questionText;
            mcq.Answers = new List<string> { a1, a2, a3, a4 };
            mcq.CorrectAnswer = a1;
        }

        TriviaFileService.SaveQuestions(_mainWindow.GetQuestions());
        _mainWindow.RefreshQuestions();

        Close();
    }

    private bool IsPositiveInteger(string value)
    {
        return int.TryParse(value, out int number) && number >= 0;
    }

    private async Task ShowMessage(string text)
    {
        var dialog = new Window
        {
            Width = 400,
            Height = 180,
            Title = "Message"
        };

        var okButton = new Button
        {
            Content = "OK",
            Width = 80,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
        };
        okButton.Click += (_, _) => dialog.Close();

        dialog.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 20,
            Children =
            {
                new TextBlock
                {
                    Text = text,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                },
                okButton
            }
        };

        await dialog.ShowDialog(this);
    }
}