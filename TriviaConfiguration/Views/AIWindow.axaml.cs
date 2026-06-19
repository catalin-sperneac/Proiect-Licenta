using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Threading.Tasks;
using TriviaConfiguration.Services;

namespace TriviaConfiguration.Views;

public partial class AIWindow : Window
{
    private readonly MainWindow _mainWindow;

    private string? _filePath;

    public AIWindow(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        InitializeComponent();

        LanguageComboBox.SelectedIndex = 0;
    }

    //BACK
    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    //FILE PICK
    private async void AddFileButton_Click(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select file",
            AllowMultiple = false
        });

        if (files.Count == 0)
            return;

        _filePath = files[0].Path.LocalPath;
        SelectedFileText.Text = Path.GetFileName(_filePath);
    }

    //CLEAR FILE
    private void ClearFileButton_Click(object? sender, RoutedEventArgs e)
    {
        _filePath = null;
        SelectedFileText.Text = "No file selected";
    }

    //GENERATE
    private async Task Generate(bool append)
    {
        ApplyButton.IsEnabled = false;
        AppendButton.IsEnabled = false;
        BackButton.IsEnabled = false;
        
        try
        {
            ApplyButton.Content = "Generating...";
            AppendButton.Content = "Generating...";

            int mc = int.TryParse(MCQuestionsTextBox.Text, out var m) ? m : 0;
            int inp = int.TryParse(InputQuestionsTextBox.Text, out var i) ? i : 0;

            if (mc == 0 && inp == 0)
            {
                await ShowMessage("Please enter at least one question.");
                return;
            }

            string lang =
                (LanguageComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "English";

            string prompt = AdditionalPromptTextBox.Text ?? "";

            string result = await ApiService.GenerateTriviaAsync(
                mc,
                inp,
                prompt,
                lang,
                _filePath
            );

            if (string.IsNullOrWhiteSpace(result))
            {
                await ShowMessage("AI returned empty response.");
                return;
            }

            if (append)
                TriviaFileService.AppendQuestionsFromString(result);
            else
                TriviaFileService.SaveQuestionsFromString(result);

            _mainWindow.RefreshQuestions();

            await ShowMessage("Generated successfully!");
            Close();
        }
        catch (Exception ex)
        {
            await ShowMessage(ex.Message);
        }
        finally
        {
            ApplyButton.IsEnabled = true;
            AppendButton.IsEnabled = true;
            ApplyButton.Content = "Generate (Replace)";
            AppendButton.Content = "Generate (Append)";
            BackButton.IsEnabled = true;
        }
    }
    
    private async void GenerateReplace_Click(object? sender, RoutedEventArgs e)
    {
        await Generate(false);
    }

    private async void GenerateAppend_Click(object? sender, RoutedEventArgs e)
    {
        await Generate(true);
    }

    //MESSAGE BOX
    private async Task ShowMessage(string text)
    {
        var dialog = new Window
        {
            Width = 400,
            Height = 180,
            Title = "Info"
        };

        var ok = new Button { Content = "OK" };
        ok.Click += (_, _) => dialog.Close();

        dialog.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(15),
            Spacing = 10,
            Children =
            {
                new TextBlock
                {
                    Text = text,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                },
                ok
            }
        };

        await dialog.ShowDialog(this);
    }
}
