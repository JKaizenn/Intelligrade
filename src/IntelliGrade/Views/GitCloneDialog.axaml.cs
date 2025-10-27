using Avalonia.Controls;
using Avalonia.Interactivity;
using IntelliGrade.App.ViewModels;

namespace IntelliGrade.App.Views;

public partial class GitCloneDialog : Window
{
    public GitCloneDialog()
    {
        InitializeComponent();

        // Inherit theme from owner window
        Opened += (s, e) =>
        {
            if (Owner is MainWindow mainWindow && mainWindow.DataContext is MainWindowViewModel mainVm)
            {
                UpdateWindowClasses(mainVm.IsDarkMode);
            }
        };
    }

    private void UpdateWindowClasses(bool isDarkMode)
    {
        Classes.Clear();
        if (isDarkMode)
        {
            Classes.Add("dark");
        }
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
