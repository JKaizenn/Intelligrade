namespace IntelliGrade.App.Models;

/// <summary>
/// A scoring level within a grading criterion (e.g., "Complete" = 50pts).
/// </summary>
public class CriterionLevel
{
    public string Label { get; set; } = string.Empty;
    public int Points { get; set; }
    public string Description { get; set; } = string.Empty;
}