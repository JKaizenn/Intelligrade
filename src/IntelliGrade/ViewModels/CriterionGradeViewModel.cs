using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliGrade.App.DTOs;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.ViewModels;

/// <summary>
/// ViewModel for grading a single criterion.
/// Wraps a criterion from the rubric with AI suggestions and instructor input.
/// </summary>
public partial class CriterionGradeViewModel : ObservableObject
{
    /// <summary>
    /// The criterion being graded (from rubric).
    /// </summary>
    public Criterion Criterion { get; }

    /// <summary>
    /// Levels sorted from highest to lowest points (for Canvas-style rubric grid).
    /// </summary>
    public IEnumerable<CriterionLevel> SortedLevels =>
        Criterion.Levels.OrderByDescending(l => l.Points);

    /// <summary>
    /// AI's suggested score and reasoning for this criterion.
    /// </summary>
    public AiCriterionSuggestion? AiSuggestion { get; }

    /// <summary>
    /// Instructor's assigned score for this criterion.
    /// Null if not yet scored.
    /// </summary>
    [ObservableProperty]
    private int? _instructorScore;

    /// <summary>
    /// Optional instructor feedback for this criterion.
    /// </summary>
    [ObservableProperty]
    private string _feedback = string.Empty;

    /// <summary>
    /// The currently selected rubric level (for visual rubric mode).
    /// </summary>
    [ObservableProperty]
    private CriterionLevel? _selectedLevel;

    /// <summary>
    /// Whether the instructor has assigned a score.
    /// </summary>
    public bool IsScored => InstructorScore.HasValue;

    /// <summary>
    /// Whether the instructor's score matches the AI suggestion.
    /// </summary>
    public bool MatchesAiSuggestion =>
        InstructorScore.HasValue &&
        AiSuggestion != null &&
        InstructorScore.Value == AiSuggestion.SuggestedScore;

    /// <summary>
    /// The final score to use (instructor score if set, otherwise null).
    /// </summary>
    public int? FinalScore => InstructorScore;

    public CriterionGradeViewModel(Criterion criterion, AiCriterionSuggestion? aiSuggestion = null)
    {
        Criterion = criterion;
        AiSuggestion = aiSuggestion;
    }

    /// <summary>
    /// Applies the AI suggestion as the instructor score.
    /// </summary>
    public void ApplyAiSuggestion()
    {
        if (AiSuggestion != null)
        {
            InstructorScore = AiSuggestion.SuggestedScore;
        }
    }

    /// <summary>
    /// Command to apply AI suggestion (wraps ApplyAiSuggestion method).
    /// </summary>
    [RelayCommand]
    private void ApplyAi() => ApplyAiSuggestion();

    /// <summary>
    /// Clears the instructor score and feedback.
    /// </summary>
    public void ClearScore()
    {
        InstructorScore = null;
        Feedback = string.Empty;
    }

    partial void OnInstructorScoreChanged(int? value)
    {
        // Notify computed properties when score changes
        OnPropertyChanged(nameof(IsScored));
        OnPropertyChanged(nameof(MatchesAiSuggestion));
        OnPropertyChanged(nameof(FinalScore));

        // Update selected level if score matches a level
        if (value.HasValue && SelectedLevel?.Points != value.Value)
        {
            SelectedLevel = Criterion.Levels.FirstOrDefault(l => l.Points == value.Value);
        }
    }

    partial void OnSelectedLevelChanged(CriterionLevel? value)
    {
        // Update instructor score when level is selected
        if (value != null && InstructorScore != value.Points)
        {
            InstructorScore = value.Points;
        }
    }

    /// <summary>
    /// Command to select a specific rubric level.
    /// </summary>
    [RelayCommand]
    private void SelectLevel(CriterionLevel level)
    {
        SelectedLevel = level;
    }
}
