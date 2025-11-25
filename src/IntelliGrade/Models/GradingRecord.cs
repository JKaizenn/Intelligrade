using System;
using System.Collections.Generic;

namespace IntelliGrade.App.Models;

/// <summary>
/// A completed grading session saved for export.
/// </summary>
public class GradingRecord
{
    public string StudentName { get; set; } = string.Empty;
    public string? StudentId { get; set; }
    public string Assignment { get; set; } = string.Empty;
    public string? SourceFile { get; set; }
    public DateTime GradedAt { get; set; } = DateTime.Now;

    public List<CriterionScore> Scores { get; set; } = new();
    public int TotalScore { get; set; }
    public int MaxPossible { get; set; }
    public double Percentage { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public string InstructorFeedback { get; set; } = string.Empty;
}

/// <summary>
/// Instructor's final score for a single criterion.
/// </summary>
public class CriterionScore
{
    public string CriterionName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int MaxPoints { get; set; }
    public string? Feedback { get; set; }
}
