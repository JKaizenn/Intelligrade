using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliGrade.App.DTOs;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.ViewModels;

/// <summary>
/// ViewModel for managing a complete grading session.
/// Coordinates grading across multiple criteria with AI assistance.
/// </summary>
public partial class GradingSessionViewModel : ObservableObject
{
    /// <summary>
    /// The rubric being used for grading.
    /// </summary>
    public Rubric Rubric { get; }

    /// <summary>
    /// View models for each criterion in the rubric.
    /// </summary>
    public ObservableCollection<CriterionGradeViewModel> CriteriaGrades { get; }

    /// <summary>
    /// Student's name.
    /// </summary>
    [ObservableProperty]
    private string _studentName = string.Empty;

    /// <summary>
    /// Student's ID (optional).
    /// </summary>
    [ObservableProperty]
    private string _studentId = string.Empty;

    /// <summary>
    /// Assignment name for this grading session.
    /// </summary>
    [ObservableProperty]
    private string _assignmentName = string.Empty;

    /// <summary>
    /// Source file being graded (optional).
    /// </summary>
    [ObservableProperty]
    private string _sourceFile = string.Empty;

    /// <summary>
    /// Overall instructor feedback for the submission.
    /// </summary>
    [ObservableProperty]
    private string _overallFeedback = string.Empty;

    /// <summary>
    /// Total score across all graded criteria.
    /// </summary>
    public int TotalScore => CriteriaGrades
        .Where(c => c.FinalScore.HasValue)
        .Sum(c => c.FinalScore!.Value);

    /// <summary>
    /// Maximum possible score from the rubric.
    /// </summary>
    public int MaxPossible => Rubric.TotalPoints;

    /// <summary>
    /// Percentage score (0-100).
    /// </summary>
    public double Percentage => MaxPossible > 0
        ? (TotalScore / (double)MaxPossible) * 100.0
        : 0.0;

    /// <summary>
    /// Letter grade based on percentage.
    /// </summary>
    public string LetterGrade => CalculateLetterGrade(Percentage);

    /// <summary>
    /// Whether all criteria have been scored.
    /// </summary>
    public bool IsComplete => CriteriaGrades.All(c => c.IsScored);

    /// <summary>
    /// Number of criteria that have been scored.
    /// </summary>
    public int ScoredCount => CriteriaGrades.Count(c => c.IsScored);

    /// <summary>
    /// Total number of criteria.
    /// </summary>
    public int TotalCriteria => CriteriaGrades.Count;

    public GradingSessionViewModel(Rubric rubric, AiGradingResponse? aiResponse = null)
    {
        Rubric = rubric;
        AssignmentName = rubric.Name;

        // Create CriterionGradeViewModel for each criterion
        CriteriaGrades = new ObservableCollection<CriterionGradeViewModel>();

        foreach (var criterion in rubric.Criteria)
        {
            // Find matching AI suggestion if available
            var aiSuggestion = aiResponse?.Suggestions
                .FirstOrDefault(s => s.CriterionName.Equals(criterion.Name, StringComparison.OrdinalIgnoreCase));

            var gradeVM = new CriterionGradeViewModel(criterion, aiSuggestion);

            // Subscribe to property changes to update computed properties
            gradeVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CriterionGradeViewModel.InstructorScore))
                {
                    OnPropertyChanged(nameof(TotalScore));
                    OnPropertyChanged(nameof(Percentage));
                    OnPropertyChanged(nameof(LetterGrade));
                    OnPropertyChanged(nameof(IsComplete));
                    OnPropertyChanged(nameof(ScoredCount));
                }
            };

            CriteriaGrades.Add(gradeVM);
        }
    }

    /// <summary>
    /// Applies all AI suggestions as instructor scores.
    /// </summary>
    [RelayCommand]
    private void ApplyAllAiSuggestions()
    {
        foreach (var criterionGrade in CriteriaGrades)
        {
            criterionGrade.ApplyAiSuggestion();
        }
    }

    /// <summary>
    /// Clears all scores and feedback.
    /// </summary>
    [RelayCommand]
    private void ClearScores()
    {
        foreach (var criterionGrade in CriteriaGrades)
        {
            criterionGrade.ClearScore();
        }
        OverallFeedback = string.Empty;
    }

    /// <summary>
    /// Creates a GradingRecord from the current session for export.
    /// </summary>
    public GradingRecord ToGradingRecord()
    {
        var scores = CriteriaGrades
            .Where(c => c.IsScored)
            .Select(c => new CriterionScore
            {
                CriterionName = c.Criterion.Name,
                Score = c.FinalScore!.Value,
                MaxPoints = c.Criterion.MaxPoints,
                Feedback = c.Feedback
            })
            .ToList();

        return new GradingRecord
        {
            StudentName = StudentName,
            StudentId = StudentId,
            Assignment = AssignmentName,
            SourceFile = SourceFile,
            GradedAt = DateTime.Now,
            Scores = scores,
            TotalScore = TotalScore,
            MaxPossible = MaxPossible,
            Percentage = Percentage,
            LetterGrade = LetterGrade,
            InstructorFeedback = OverallFeedback
        };
    }

    /// <summary>
    /// Calculates letter grade from percentage.
    /// Uses standard grading scale.
    /// </summary>
    private static string CalculateLetterGrade(double percentage)
    {
        return percentage switch
        {
            >= 93.0 => "A",
            >= 90.0 => "A-",
            >= 87.0 => "B+",
            >= 83.0 => "B",
            >= 80.0 => "B-",
            >= 77.0 => "C+",
            >= 73.0 => "C",
            >= 70.0 => "C-",
            >= 67.0 => "D+",
            >= 63.0 => "D",
            >= 60.0 => "D-",
            _ => "F"
        };
    }
}
