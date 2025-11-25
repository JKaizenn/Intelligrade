namespace IntelliGrade.App.DTOs;

/// <summary>
/// Complete AI grading analysis for a submission.
/// </summary>
public class AiGradingResponse
{
    public List<AiCriterionSuggestion> Suggestions { get; set; } = new();
    public int RecommendedTotal { get; set; }
    public int MaxPossible { get; set; }
    public string Summary { get; set; } = string.Empty;
    public AiConfidence OverallConfidence { get; set; } = AiConfidence.Medium;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
