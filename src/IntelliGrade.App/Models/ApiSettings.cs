namespace IntelliGrade.App.Models;

/// <summary>
/// Configuration settings for AI service API access.
/// Supports both local Ollama and cloud-based API providers.
/// </summary>
public class ApiSettings
{
    public string OllamaEndpoint { get; set; } = "http://localhost:11434";
    public string OllamaModel { get; set; } = "llama3.2:1b";
    public bool UseCustomEndpoint { get; set; } = false;

    public string OpenAiApiKey { get; set; } = string.Empty;
    public string OpenAiModel { get; set; } = "gpt-4";
    public bool UseOpenAi { get; set; } = false;

    public string AnthropicApiKey { get; set; } = string.Empty;
    public string AnthropicModel { get; set; } = "claude-3-5-sonnet-20241022";
    public bool UseAnthropic { get; set; } = false;

    public string SelectedProvider { get; set; } = "Ollama";
}
