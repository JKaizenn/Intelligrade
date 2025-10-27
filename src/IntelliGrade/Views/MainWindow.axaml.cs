using Avalonia.Controls;
using IntelliGrade.App.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace IntelliGrade.App.Views;

/// <summary>
/// Main application window with dynamic sizing based on screen resolution.
/// Defaults to 1920x1080 or 90% of screen size, whichever is smaller.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Set initial window size based on screen
        SetWindowSize();

        // Set StorageProvider on ViewModel when window is loaded
        Opened += (s, e) =>
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.StorageProvider = StorageProvider;

                // Set confirmation dialog handler
                vm.ShowConfirmationDialog = ShowConfirmation;

                // Subscribe to dark mode changes to update window classes
                vm.PropertyChanged += OnViewModelPropertyChanged;
                UpdateWindowClasses(vm.IsDarkMode);
            }
        };
    }

    /// <summary>
    /// Sets window size to 1920x1080 or 90% of screen size, whichever is smaller.
    /// </summary>
    private void SetWindowSize()
    {
        try
        {
            var screen = Screens.Primary;
            if (screen != null)
            {
                var screenWidth = screen.WorkingArea.Width / screen.Scaling;
                var screenHeight = screen.WorkingArea.Height / screen.Scaling;

                // Default to 1920x1080, but use 90% of screen if smaller
                var targetWidth = Math.Min(1920, screenWidth * 0.9);
                var targetHeight = Math.Min(1080, screenHeight * 0.9);

                Width = targetWidth;
                Height = targetHeight;
            }
            else
            {
                // Fallback to 1920x1080
                Width = 1920;
                Height = 1080;
            }
        }
        catch
        {
            // Fallback to 1920x1080 on any error
            Width = 1920;
            Height = 1080;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.IsDarkMode) && sender is MainWindowViewModel vm)
        {
            UpdateWindowClasses(vm.IsDarkMode);
        }
    }

    private void UpdateWindowClasses(bool isDarkMode)
    {
        Classes.Clear();
        if (isDarkMode)
        {
            Classes.Add("dark");
        }
    }

    private async Task<bool> ShowConfirmation(string title, string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            ButtonEnum.YesNo
        );

        var result = await box.ShowWindowDialogAsync(this);
        return result == ButtonResult.Yes;
    }
}