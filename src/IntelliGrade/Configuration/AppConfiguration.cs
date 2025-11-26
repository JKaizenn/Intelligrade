using System;
using System.IO;

namespace IntelliGrade.App.Configuration;

/// <summary>
/// Main application configuration with validation.
/// Immutable configuration object following best practices for robustness.
/// </summary>
public class AppConfiguration
{
    public GradingConfiguration Grading { get; }
    public OllamaConfiguration Ollama { get; }
    public StorageConfiguration Storage { get; }
    public ExecutionConfiguration Execution { get; }

    public AppConfiguration(
        GradingConfiguration? grading = null,
        OllamaConfiguration? ollama = null,
        StorageConfiguration? storage = null,
        ExecutionConfiguration? execution = null)
    {
        Grading = grading ?? new GradingConfiguration();
        Ollama = ollama ?? new OllamaConfiguration();
        Storage = storage ?? new StorageConfiguration();
        Execution = execution ?? new ExecutionConfiguration();

        Validate();
    }

    private void Validate()
    {
        if (Grading == null)
            throw new InvalidOperationException("Grading configuration cannot be null");
        if (Ollama == null)
            throw new InvalidOperationException("Ollama configuration cannot be null");
        if (Storage == null)
            throw new InvalidOperationException("Storage configuration cannot be null");
        if (Execution == null)
            throw new InvalidOperationException("Execution configuration cannot be null");
    }
}

/// <summary>
/// Configuration for grading-related settings.
/// </summary>
public class GradingConfiguration
{
    public string RubricDirectory { get; }
    public string DefaultModel { get; }

    public GradingConfiguration(
        string? rubricDirectory = null,
        string? defaultModel = null)
    {
        RubricDirectory = rubricDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "bin",
            "rubrics");

        DefaultModel = defaultModel ?? "qwen3-coder:30b";

        // Ensure rubric directory exists
        Directory.CreateDirectory(RubricDirectory);
    }
}

/// <summary>
/// Configuration for Ollama AI service.
/// </summary>
public class OllamaConfiguration
{
    public string Endpoint { get; }
    public string DefaultModel { get; }
    public int MaxRetries { get; }

    public OllamaConfiguration(
        string? endpoint = null,
        string? defaultModel = null,
        int maxRetries = 3)
    {
        Endpoint = endpoint ?? "http://localhost:11434";
        DefaultModel = defaultModel ?? "qwen3-coder:30b";
        MaxRetries = maxRetries > 0 && maxRetries <= 10
            ? maxRetries
            : throw new ArgumentException("MaxRetries must be between 1 and 10", nameof(maxRetries));
    }
}

/// <summary>
/// Configuration for local storage.
/// </summary>
public class StorageConfiguration
{
    public string StorageDirectory { get; }
    public string SettingsFileName { get; }

    public StorageConfiguration(
        string? storageDirectory = null,
        string? settingsFileName = null)
    {
        StorageDirectory = storageDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IntelliGrade");

        SettingsFileName = settingsFileName ?? "settings.json";

        // Ensure storage directory exists
        Directory.CreateDirectory(StorageDirectory);
    }

    public string SettingsFilePath => Path.Combine(StorageDirectory, SettingsFileName);
}

/// <summary>
/// Configuration for program execution.
/// </summary>
public class ExecutionConfiguration
{
    public int TimeoutSeconds { get; }
    public int MaxOutputLines { get; }
    public bool EnableSandbox { get; }

    public ExecutionConfiguration(
        int timeoutSeconds = 30,
        int maxOutputLines = 10000,
        bool enableSandbox = false)
    {
        TimeoutSeconds = timeoutSeconds > 0 && timeoutSeconds <= 300
            ? timeoutSeconds
            : throw new ArgumentException("Timeout must be between 1 and 300 seconds", nameof(timeoutSeconds));

        MaxOutputLines = maxOutputLines > 0 && maxOutputLines <= 100000
            ? maxOutputLines
            : throw new ArgumentException("MaxOutputLines must be between 1 and 100000", nameof(maxOutputLines));

        EnableSandbox = enableSandbox;
    }
}
