using System.Collections.Generic;

namespace IntelliGrade.App.Models;

/// <summary>
/// Defines the available AI analysis modes for grading.
/// Each mode represents a different speed/quality tradeoff.
/// </summary>
public enum AnalysisMode
{
    /// <summary>
    /// Quick analysis using smallest model and simplified prompt.
    /// Best for: Initial pass, low-end hardware, quick checks.
    /// Expected time: 5-15 seconds.
    /// </summary>
    Fast,

    /// <summary>
    /// Standard analysis with full rubric evaluation.
    /// Best for: Regular grading workflow (default).
    /// Expected time: 15-30 seconds.
    /// </summary>
    Balanced,

    /// <summary>
    /// Comprehensive analysis with advanced code quality metrics.
    /// Best for: Final review, complex assignments, borderline cases.
    /// Expected time: 45-90 seconds.
    /// </summary>
    Detailed
}

/// <summary>
/// Configuration settings for a specific analysis mode.
/// Immutable record for thread safety and clarity.
/// </summary>
public record AnalysisModeConfig
{
    /// <summary>
    /// The analysis mode this configuration applies to.
    /// </summary>
    public required AnalysisMode Mode { get; init; }

    /// <summary>
    /// Display name shown in the UI.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Brief description of what this mode does.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Icon/emoji for visual identification.
    /// </summary>
    public required string Icon { get; init; }

    /// <summary>
    /// Recommended Ollama model for this mode.
    /// </summary>
    public required string RecommendedModel { get; init; }

    /// <summary>
    /// Fallback model if recommended isn't available.
    /// </summary>
    public required string FallbackModel { get; init; }

    /// <summary>
    /// Maximum tokens for AI to generate.
    /// </summary>
    public required int MaxTokens { get; init; }

    /// <summary>
    /// Context window size for the model.
    /// </summary>
    public required int ContextWindow { get; init; }

    /// <summary>
    /// Temperature for response generation (0.0-1.0).
    /// </summary>
    public required float Temperature { get; init; }

    /// <summary>
    /// Whether to use the simplified "lite" prompt.
    /// </summary>
    public required bool UseLitePrompt { get; init; }

    /// <summary>
    /// Whether to include advanced analysis (complexity, bugs, security).
    /// </summary>
    public required bool IncludeAdvancedAnalysis { get; init; }

    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    public required int TimeoutSeconds { get; init; }

    /// <summary>
    /// Estimated time range to display to user.
    /// </summary>
    public required string EstimatedTime { get; init; }
}

/// <summary>
/// Provides predefined configurations for each analysis mode.
/// </summary>
public static class AnalysisModeConfigs
{
    /// <summary>
    /// Fast mode: Quick analysis with minimal model.
    /// </summary>
    public static readonly AnalysisModeConfig Fast = new()
    {
        Mode = AnalysisMode.Fast,
        DisplayName = "Fast",
        Description = "Quick scores with brief feedback",
        Icon = "",
        RecommendedModel = "qwen3:4b",
        FallbackModel = "qwen3:1.7b",
        MaxTokens = 800,
        ContextWindow = 4096,
        Temperature = 0.1f,
        UseLitePrompt = true,
        IncludeAdvancedAnalysis = false,
        TimeoutSeconds = 60,
        EstimatedTime = "10-20 sec"
    };

    /// <summary>
    /// Balanced mode: Full feedback with good performance.
    /// </summary>
    public static readonly AnalysisModeConfig Balanced = new()
    {
        Mode = AnalysisMode.Balanced,
        DisplayName = "Balanced",
        Description = "Full feedback with detailed reasoning",
        Icon = "",
        RecommendedModel = "qwen3-coder:30b",
        FallbackModel = "qwen3:8b",
        MaxTokens = 1500,
        ContextWindow = 8192,
        Temperature = 0.2f,
        UseLitePrompt = false,
        IncludeAdvancedAnalysis = false,
        TimeoutSeconds = 120,
        EstimatedTime = "20-40 sec"
    };

    /// <summary>
    /// Detailed mode: Comprehensive analysis with code quality metrics.
    /// </summary>
    public static readonly AnalysisModeConfig Detailed = new()
    {
        Mode = AnalysisMode.Detailed,
        DisplayName = "Detailed",
        Description = "Deep analysis with bugs, security, complexity",
        Icon = "",
        RecommendedModel = "qwen3:14b",
        FallbackModel = "qwen3-coder:30b",
        MaxTokens = 2500,
        ContextWindow = 16384,
        Temperature = 0.3f,
        UseLitePrompt = false,
        IncludeAdvancedAnalysis = true,
        TimeoutSeconds = 180,
        EstimatedTime = "60-120 sec"
    };

    /// <summary>
    /// Gets all available mode configurations.
    /// </summary>
    public static IReadOnlyList<AnalysisModeConfig> All { get; } = new[]
    {
        Fast,
        Balanced,
        Detailed
    };

    /// <summary>
    /// Gets the configuration for a specific mode.
    /// </summary>
    public static AnalysisModeConfig GetConfig(AnalysisMode mode) => mode switch
    {
        AnalysisMode.Fast => Fast,
        AnalysisMode.Balanced => Balanced,
        AnalysisMode.Detailed => Detailed,
        _ => Balanced
    };
}
