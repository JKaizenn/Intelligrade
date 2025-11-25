using System.Collections.Generic;

namespace IntelliGrade.App.DTOs;

/// <summary>
/// AI's suggested score and reasoning for a single criterion.
/// </summary>
public class AiCriterionSuggestion
{
    public string CriterionName { get; set; } = string.Empty;
    public int SuggestedScore { get; set; }
    public int MaxPoints { get; set; }
    public AiConfidence Confidence { get; set; } = AiConfidence.Medium;
    public string RatingLevel { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public List<string> Evidence { get; set; } = new();
}

public enum AiConfidence
{
    High,
    Medium,
    Low
}
