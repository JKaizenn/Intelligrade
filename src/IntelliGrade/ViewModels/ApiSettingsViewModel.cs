using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliGrade.App.Models;
using IntelliGrade.App.Services;

namespace IntelliGrade.App.ViewModels;

/// <summary>
/// ViewModel for configuring AI API settings.
/// Allows users to choose between local Ollama or cloud-based AI providers.
/// </summary>
public partial class ApiSettingsViewModel : ViewModelBase
{
    private readonly LocalStorageService _localStorage;

    [ObservableProperty] private string _ollamaEndpoint = "http://localhost:11434";
    [ObservableProperty] private string _ollamaModel = "llama3.2:1b";
    [ObservableProperty] private bool _useCustomEndpoint;

    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _hasSuccess;
    [ObservableProperty] private bool _hasError;

    public event EventHandler? SettingsSaved;

    public ApiSettingsViewModel(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
        LoadSettings();
    }

    private async void LoadSettings()
    {
        try
        {
            var settings = await _localStorage.GetAsync<ApiSettings>("ApiSettings");
            if (settings != null)
            {
                OllamaEndpoint = settings.OllamaEndpoint;
                OllamaModel = settings.OllamaModel;
                UseCustomEndpoint = settings.UseCustomEndpoint;
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        ClearMessages();

        try
        {
            var settings = new ApiSettings
            {
                OllamaEndpoint = OllamaEndpoint,
                OllamaModel = OllamaModel,
                UseCustomEndpoint = UseCustomEndpoint
            };

            await _localStorage.SetAsync("ApiSettings", settings);

            ShowSuccess("Settings saved successfully!");
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to save settings: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        OllamaEndpoint = "http://localhost:11434";
        OllamaModel = "llama3.2:1b";
        UseCustomEndpoint = false;

        ShowSuccess("Settings reset to defaults");
    }

    private void ShowSuccess(string message)
    {
        StatusMessage = message;
        HasSuccess = true;
        HasError = false;
    }

    private void ShowError(string message)
    {
        StatusMessage = message;
        HasError = true;
        HasSuccess = false;
    }

    private void ClearMessages()
    {
        StatusMessage = string.Empty;
        HasSuccess = false;
        HasError = false;
    }
}
