using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using IntelliGrade.App.ViewModels;

namespace IntelliGrade.App.Views;

public partial class GitCloneDialog : Window
{
    public GitCloneDialog()
    {
        InitializeComponent();
    }

    private void CloneButton_Click(object? sender, RoutedEventArgs e)
    {
        var repoUrl = this.FindControl<TextBox>("RepoUrlTextBox")?.Text;

        // Validate URL before closing
        if (string.IsNullOrWhiteSpace(repoUrl))
        {
            // Could show error message here if needed
            return;
        }

        Close(repoUrl);
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private void RepoUrlTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CloneButton_Click(sender, e);
        }
    }
}
