using Avalonia.Controls;
using IntelliGrade.App.ViewModels;

namespace IntelliGrade.App.Views;

public partial class WelcomeView : UserControl
{
    public WelcomeView()
    {
        InitializeComponent();

        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(MainWindowViewModel.IsDarkMode))
                    {
                        UpdateTheme(vm.IsDarkMode);
                    }
                };

                // Set initial theme
                UpdateTheme(vm.IsDarkMode);
            }
        };
    }

    private void UpdateTheme(bool isDarkMode)
    {
        Classes.Clear();
        if (isDarkMode)
        {
            Classes.Add("dark");
        }
    }
}
