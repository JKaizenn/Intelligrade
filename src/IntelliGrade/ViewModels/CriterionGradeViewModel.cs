using CommunityToolkit.Mvvm.ComponentModel;
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
    }
}
