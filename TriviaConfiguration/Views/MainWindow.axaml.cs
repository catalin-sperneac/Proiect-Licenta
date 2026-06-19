using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.ApplicationLifetimes;
using System.Collections.Generic;
using TriviaConfiguration.Models;
using TriviaConfiguration.Services;

namespace TriviaConfiguration.Views;

public partial class MainWindow : Window
{
    private List<TriviaQuestionBase> _questions = new();
    private int _currentIndex = 0;
    private bool _fileMissing = false;

    public MainWindow()
    {
        InitializeComponent();
        RefreshQuestions();
    }

    public void RefreshQuestions()
    {
        if (!TriviaFileService.FileExists())
        {
            _fileMissing = true;
            _questions.Clear();
            _currentIndex = 0;
        }
        else
        {
            _fileMissing = false;
            _questions = TriviaFileService.LoadQuestions();

            if (_currentIndex >= _questions.Count)
                _currentIndex = _questions.Count - 1;

            if (_currentIndex < 0)
                _currentIndex = 0;
        }

        DisplayQuestion();
    }

    private void DisplayQuestion()
    {
        if (_fileMissing)
        {
            TypeTextBlock.Text = "Type: none";
            QuestionTextBlock.Text = "Trivia.txt not found in the same folder as the program.";
            CorrectAnswerTextBlock.Text = "Correct Answer: -";
            QuestionNumberTextBlock.Text = "0 / 0";
            PrevButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            return;
        }

        if (_questions.Count == 0)
        {
            TypeTextBlock.Text = "Type: none";
            QuestionTextBlock.Text = "Trivia.txt exists, but no valid questions were found.";
            CorrectAnswerTextBlock.Text = "Correct Answer: -";
            QuestionNumberTextBlock.Text = "0 / 0";
            PrevButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            return;
        }

        var question = _questions[_currentIndex];

        if(question is InputQuestion)
        {
            TypeTextBlock.Text = $"Type: Input Question";
        }
        else if(question is MultipleChoiceQuestion)
        {
            TypeTextBlock.Text = $"Type: Multiple Choice Question";
        }
        QuestionTextBlock.Text = question.QuestionText;
        CorrectAnswerTextBlock.Text = $"Correct Answer: {question.CorrectAnswer}";
        QuestionNumberTextBlock.Text = $"{_currentIndex + 1} / {_questions.Count}";

        PrevButton.IsEnabled = _currentIndex > 0;
        NextButton.IsEnabled = _currentIndex < _questions.Count - 1;
        DeleteButton.IsEnabled = true;
    }

    private void PrevButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            DisplayQuestion();
        }
    }

    private void NextButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_currentIndex < _questions.Count - 1)
        {
            _currentIndex++;
            DisplayQuestion();
        }
    }

    private void AddQuestionButton_Click(object? sender, RoutedEventArgs e)
    {
        var addWindow = new AddQuestionWindow(this);
        addWindow.Show();
    }

    private void AIButton_Click(object? sender, RoutedEventArgs e)
    {
        var aiWindow = new AIWindow(this);
        aiWindow.Show();
    }

    private void DeleteButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_questions.Count == 0)
            return;

        _questions.RemoveAt(_currentIndex);
        TriviaFileService.SaveQuestions(_questions);

        if (_currentIndex >= _questions.Count)
            _currentIndex = _questions.Count - 1;

        if (_currentIndex < 0)
            _currentIndex = 0;

        RefreshQuestions();
    }
    
    private void ModifyQuestion_Click(object? sender, RoutedEventArgs e)
    {
        if (_questions.Count == 0)
            return;

        var selected = _questions[_currentIndex];

        var modifyWindow = new ModifyQuestionWindow(this, selected);
        modifyWindow.Show();
    }

    public List<TriviaQuestionBase> GetQuestions()
    {
        return _questions;
    }


    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows)
            {
                if (window != this)
                    window.Close();
            }
        }
    }
}