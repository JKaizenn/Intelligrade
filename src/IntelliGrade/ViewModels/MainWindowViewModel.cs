using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliGrade.App.Models;
using IntelliGrade.App.Services;

namespace IntelliGrade.App.ViewModels;

/// <summary>
/// Main application view model coordinating the grading workflow.
/// Manages course selection, code execution, AI analysis, and grade recording.
/// </summary>
// In MainWindowViewModel.cs - Replace your current theme handling with this:

using Avalonia;
using Avalonia.Styling;

// ... other usings

public partial class MainWindowViewModel : ViewModelBase
{
    // Keep this property for binding to toggle button
    [ObservableProperty] 
    private bool _isDarkMode;

    // No longer need WindowClass property!
    // DELETE: public string WindowClass => IsDarkMode ? "dark" : "";

    // Simplified theme toggle
    [RelayCommand]
    private void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        ApplyTheme();
    }

    // Called when IsDarkMode changes
    partial void OnIsDarkModeChanged(bool value)
    {
        ApplyTheme();
        SaveThemePreference();
    }

    // Single method to apply theme - no class manipulation needed!
    private void ApplyTheme()
    {
        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeVariant = IsDarkMode 
                ? ThemeVariant.Dark 
                : ThemeVariant.Light;
        }
    }

    private async void LoadSettings()
    {
        try
        {
            var savedTheme = await _localStorage.GetAsync<bool?>("IsDarkMode", null);

            if (savedTheme.HasValue)
            {
                IsDarkMode = savedTheme.Value;
            }
            else
            {
                // Detect system theme
                IsDarkMode = DetectSystemTheme();
            }

            // Apply on startup
            ApplyTheme();

            var dir = await _localStorage.GetAsync("LastDirectory", Directory.GetCurrentDirectory());
            if (dir != null) CurrentDirectory = dir;
        }
        catch { }
    }

    private bool DetectSystemTheme()
    {
        try
        {
            var app = Application.Current;
            if (app?.PlatformSettings != null)
            {
                var colorValues = app.PlatformSettings.GetColorValues();
                return colorValues.ThemeVariant == Avalonia.Platform.PlatformThemeVariant.Dark;
            }
        }
        catch { }
        return false;
    }

    private async void SaveThemePreference()
    {
        try
        {
            await _localStorage.SetAsync("IsDarkMode", IsDarkMode);
        }
        catch { }
    }
}