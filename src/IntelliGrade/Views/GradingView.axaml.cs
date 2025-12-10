using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using IntelliGrade.App.ViewModels;

namespace IntelliGrade.App.Views;

public partial class GradingView : UserControl
{
    public GradingView()
    {
        InitializeComponent();
    }

    private void InteractiveInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is MainWindowViewModel viewModel)
        {
            viewModel.SendInputCommand.Execute(null);
        }
    }
}
