using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using TriviaConfiguration.Models;
using TriviaConfiguration.Services;

namespace TriviaConfiguration.Views;

public partial class AddQuestionWindow : Window
{
    private readonly MainWindow _mainWindow;
    private string _selectedType = "";

    public AddQuestionWindow(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        InitializeComponent();
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void QuestionTypeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (QuestionTypeComboBox.SelectedItem is ComboBoxItem item)
        {
            _selectedType = item.Content?.ToString() ?? "";

            MultipleChoicePanel.IsVisible = _selectedType == "multiple choice";
        }
    }

    private void CorrectAnswerTextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        // Answer1 is hidden, automatically equal to correct answer
    }

    private void NumberTextBox_OnTextInput(object? sender, TextInputEventArgs e)
    {
        foreach (char c in e.Text)
        {
            if (!char.IsDigit(c))
            {
                e.Handled = true;
                break;
            }
        }
    }

    private async void ApplyButton_Click(object? sender, RoutedEventArgs e)
    {
        var questionText = QuestionTextBox.Text?.Trim() ?? "";
        var correctAnswer = CorrectAnswerTextBox.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(_selectedType))
        {
            await ShowMessage("Please select a question type.");
            return;
        }

        if (string.IsNullOrWhiteSpace(questionText))
        {
            await ShowMessage("Please enter the question text.");
            return;
        }

        if (string.IsNullOrWhiteSpace(correctAnswer))
        {
            await ShowMessage("Please enter the correct answer.");
            return;
        }

        var questions = TriviaFileService.LoadQuestions();

        if (_selectedType == "input")
        {
            if (!IsPositiveInteger(correctAnswer))
            {
                await ShowMessage("Correct answer must be a non-negative integer (0 allowed).");
                return;
            }

            questions.Add(new InputQuestion
            {
                QuestionText = questionText,
                CorrectAnswer = correctAnswer
            });
        }
        else if (_selectedType == "multiple choice")
        {
            var a1 = correctAnswer;
            var a2 = Answer2TextBox.Text?.Trim() ?? "";
            var a3 = Answer3TextBox.Text?.Trim() ?? "";
            var a4 = Answer4TextBox.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(a2) ||
                string.IsNullOrWhiteSpace(a3) ||
                string.IsNullOrWhiteSpace(a4))
            {
                await ShowMessage("Please fill Answer 2, Answer 3, and Answer 4.");
                return;
            }

            if (a2 == correctAnswer || a3 == correctAnswer || a4 == correctAnswer)
            {
                await ShowMessage("Answer 2, Answer 3, and Answer 4 must be different from the correct answer.");
                return;
            }

            if (a2 == a3 || a2 == a4 || a3 == a4)
            {
                await ShowMessage("Answer 2, Answer 3, and Answer 4 must all be different from each other.");
                return;
            }

            questions.Add(new MultipleChoiceQuestion
            {
                QuestionText = questionText,
                Answers = new List<string> { a1, a2, a3, a4 },
                CorrectAnswer = correctAnswer
            });
        }

        TriviaFileService.SaveQuestions(questions);
        _mainWindow.RefreshQuestions();
        Close();
    }

    private bool IsPositiveInteger(string value)
    {
        return int.TryParse(value, out int number) && number >= 0; // 0 allowed
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