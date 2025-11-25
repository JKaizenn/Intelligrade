using Avalonia.Controls;
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
        Close(repoUrl);
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
