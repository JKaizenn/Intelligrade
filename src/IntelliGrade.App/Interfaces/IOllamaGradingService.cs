using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntelliGrade.App.Interfaces;

/// <summary>
/// Interface for AI-powered code grading service.
/// Provides abstraction for AI grading, allowing multiple AI provider implementations.
/// </summary>
public interface IOllamaGradingService
{
    /// <summary>
    /// Checks if the AI service is available and operational.
    /// </summary>
    /// <returns>True if service is ready, false otherwise</returns>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Analyzes student code using AI and provides grading feedback.
    /// </summary>
    /// <param name="sourceCode">Student's source code</param>
    /// <param name="rubric">Grading rubric</param>
    /// <param name="courseName">Course name</param>
    /// <param name="assignmentName">Assignment name</param>
    /// <param name="outputContents">Program output contents</param>
    /// <returns>AI-generated analysis and grading suggestions</returns>
    Task<string> AnalyzeCodeAsync(
        string sourceCode,
        string rubric,
        string courseName,
        string assignmentName,
        List<string> outputContents);
}
