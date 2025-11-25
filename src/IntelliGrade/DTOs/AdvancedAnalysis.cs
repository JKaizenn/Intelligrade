using System.Collections.Generic;

namespace IntelliGrade.App.DTOs;

/// <summary>
/// Advanced code analysis results beyond rubric grading.
/// Includes complexity metrics, potential bugs, security issues, and code smells.
/// </summary>
public class AdvancedAnalysis
{
    /// <summary>
    /// Estimated cyclomatic complexity score based on decision points.
    /// </summary>
    public int CyclomaticComplexity { get; set; }

    /// <summary>
    /// Complexity rating: Low (1-10), Medium (11-20), High (21+).
    /// </summary>
    public string ComplexityRating { get; set; } = string.Empty;

    /// <summary>
    /// Potential bugs or logic errors detected in the code.
    /// </summary>
    public List<CodeIssue> PotentialBugs { get; set; } = new();

    /// <summary>
    /// Security vulnerabilities or unsafe patterns identified.
    /// </summary>
    public List<CodeIssue> SecurityIssues { get; set; } = new();

    /// <summary>
    /// Code smells and maintainability concerns.
    /// </summary>
    public List<CodeIssue> CodeSmells { get; set; } = new();
}

/// <summary>
/// A single code issue with category, description, severity, and optional fix suggestion.
/// </summary>
public class CodeIssue
{
    /// <summary>
    /// Issue category (e.g., "Null Reference", "SQL Injection", "Long Method").
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the issue.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Severity level: Info, Warning, or Error.
    /// </summary>
    public string Severity { get; set; } = "Info";

    /// <summary>
    /// Optional line number or code location reference.
    /// </summary>
    public string? LineReference { get; set; }

    /// <summary>
    /// Optional suggestion for fixing the issue.
    /// </summary>
    public string? Suggestion { get; set; }
}
