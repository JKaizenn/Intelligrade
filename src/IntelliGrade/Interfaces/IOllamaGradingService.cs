using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IntelliGrade.App.DTOs;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.Interfaces;

/// <summary>
/// Service for AI-assisted code grading using Ollama.
/// </summary>
public interface IOllamaGradingService
{
    /// <summary>
    /// Event raised when AI generation progress updates.
    /// Reports the current number of tokens generated.
    /// </summary>
    event Action<int>? OnProgressUpdate;

    /// <summary>
    /// Checks if the AI service is available and operational.
    /// </summary>
    /// <returns>True if service is ready, false otherwise</returns>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Analyzes student code using AI and provides structured grading feedback.
    /// </summary>
    /// <param name="sourceCode">Student's source code</param>
    /// <param name="rubric">Grading rubric with criteria</param>
    /// <param name="courseName">Course name</param>
    /// <param name="assignmentName">Assignment name</param>
    /// <param name="outputContents">Program output contents</param>
    /// <returns>Structured AI grading response with per-criterion suggestions and confidence scores</returns>
    Task<AiGradingResponse> AnalyzeCodeAsync(
        string sourceCode,
        Rubric rubric,
        string courseName,
        string assignmentName,
        List<string> outputContents);

    /// <summary>
    /// Analyzes code using the specified analysis mode configuration.
    /// </summary>
    /// <param name="sourceCode">Student's source code</param>
    /// <param name="rubric">Grading rubric</param>
    /// <param name="courseName">Course name</param>
    /// <param name="assignmentName">Assignment name</param>
    /// <param name="outputContents">Program outputs</param>
    /// <param name="modeConfig">Analysis mode configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Grading response with optional advanced analysis</returns>
    Task<AiGradingResponse> AnalyzeWithModeAsync(
        string sourceCode,
        Rubric rubric,
        string courseName,
        string assignmentName,
        List<string> outputContents,
        AnalysisModeConfig modeConfig,
        CancellationToken cancellationToken = default);
}
