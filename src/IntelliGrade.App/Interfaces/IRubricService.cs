using System.Threading.Tasks;
using IntelliGrade.App.Services;

namespace IntelliGrade.App.Interfaces;

/// <summary>
/// Interface for rubric parsing and formatting service.
/// Provides abstraction for loading and formatting rubrics in multiple formats.
/// </summary>
public interface IRubricService
{
    /// <summary>
    /// Loads a JSON rubric from file.
    /// </summary>
    /// <param name="filePath">Path to JSON rubric file</param>
    /// <returns>Parsed rubric or null if parsing fails</returns>
    Task<RubricService.Rubric?> LoadRubricAsync(string filePath);

    /// <summary>
    /// Loads a plain text rubric from file.
    /// </summary>
    /// <param name="filePath">Path to text rubric file</param>
    /// <returns>Rubric content as string</returns>
    Task<string> LoadPlainTextRubricAsync(string filePath);

    /// <summary>
    /// Loads and formats rubric for AI consumption.
    /// Automatically detects format (JSON or plain text).
    /// </summary>
    /// <param name="filePath">Path to rubric file</param>
    /// <returns>Formatted rubric string</returns>
    Task<string> LoadAndFormatRubricAsync(string filePath);

    /// <summary>
    /// Formats a rubric object for AI analysis prompts.
    /// </summary>
    /// <param name="rubric">Rubric object to format</param>
    /// <returns>Formatted string optimized for AI</returns>
    string FormatRubricForAI(RubricService.Rubric rubric);

    /// <summary>
    /// Formats a rubric object for UI display.
    /// </summary>
    /// <param name="rubric">Rubric object to format</param>
    /// <returns>Formatted string optimized for human reading</returns>
    string FormatRubricForDisplay(RubricService.Rubric rubric);
}
