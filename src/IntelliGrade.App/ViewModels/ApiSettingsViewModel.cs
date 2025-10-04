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

    [ObservableProperty] private string _openAiApiKey = string.Empty;
    [ObservableProperty] private string _openAiModel = "gpt-4";
    [ObservableProperty] private bool _useOpenAi;

    [ObservableProperty] private string _anthropicApiKey = string.Empty;
    [ObservableProperty] private string _anthropicModel = "claude-3-5-sonnet-20241022";
    [ObservableProperty] private bool _useAnthropic;

    [ObservableProperty] private string _selectedProvider = "Ollama";
    [ObservableProperty] private ObservableCollection<string> _providers = new() { "Ollama", "OpenAI", "Anthropic" };

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

                OpenAiApiKey = settings.OpenAiApiKey;
                OpenAiModel = settings.OpenAiModel;
                UseOpenAi = settings.UseOpenAi;

                AnthropicApiKey = settings.AnthropicApiKey;
                AnthropicModel = settings.AnthropicModel;
                UseAnthropic = settings.UseAnthropic;

                SelectedProvider = settings.SelectedProvider;
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        ClearMessages();

        // Validate API keys if they are enabled
        if (SelectedProvider == "OpenAI" && UseOpenAi)
        {
            if (string.IsNullOrWhiteSpace(OpenAiApiKey))
            {
                ShowError("OpenAI API key is required when OpenAI is enabled");
                return;
            }
            if (!IsValidOpenAiKey(OpenAiApiKey))
            {
                ShowError("Invalid OpenAI API key format. Should start with 'sk-' or 'sk-proj-'");
                return;
            }
        }

        if (SelectedProvider == "Anthropic" && UseAnthropic)
        {
            if (string.IsNullOrWhiteSpace(AnthropicApiKey))
            {
                ShowError("Anthropic API key is required when Anthropic is enabled");
                return;
            }
            if (!IsValidAnthropicKey(AnthropicApiKey))
            {
                ShowError("Invalid Anthropic API key format. Should start with 'sk-ant-'");
                return;
            }
        }

        try
        {
            var settings = new ApiSettings
            {
                OllamaEndpoint = OllamaEndpoint,
                OllamaModel = OllamaModel,
                UseCustomEndpoint = UseCustomEndpoint,

                OpenAiApiKey = OpenAiApiKey,
                OpenAiModel = OpenAiModel,
                UseOpenAi = UseOpenAi,

                AnthropicApiKey = AnthropicApiKey,
                AnthropicModel = AnthropicModel,
                UseAnthropic = UseAnthropic,

                SelectedProvider = SelectedProvider
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

    private static bool IsValidOpenAiKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        return key.StartsWith("sk-", StringComparison.Ordinal) ||
               key.StartsWith("sk-proj-", StringComparison.Ordinal);
    }

    private static bool IsValidAnthropicKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        return key.StartsWith("sk-ant-", StringComparison.Ordinal);
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        OllamaEndpoint = "http://localhost:11434";
        OllamaModel = "llama3.2:1b";
        UseCustomEndpoint = false;

        OpenAiApiKey = string.Empty;
        OpenAiModel = "gpt-4";
        UseOpenAi = false;

        AnthropicApiKey = string.Empty;
        AnthropicModel = "claude-3-5-sonnet-20241022";
        UseAnthropic = false;

        SelectedProvider = "Ollama";

        ShowSuccess("Settings reset to defaults");
    }

    partial void OnSelectedProviderChanged(string value)
    {
        ClearMessages();
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
