namespace IntelliGrade.App.Models;

/// <summary>
/// Configuration settings for AI service API access.
/// Supports local Ollama AI provider.
/// </summary>
public class ApiSettings
{
    public string OllamaEndpoint { get; set; } = "http://localhost:11434";
    public string OllamaModel { get; set; } = "llama3.2:1b";
    public bool UseCustomEndpoint { get; set; } = false;
}
