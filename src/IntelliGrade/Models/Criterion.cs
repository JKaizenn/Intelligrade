using System.Collections.Generic;

namespace IntelliGrade.App.Models;

/// <summary>
/// A single grading criterion with scoring levels.
/// </summary>
public class Criterion
{
    public string Name { get; set; } = string.Empty;
    public int MaxPoints { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<CriterionLevel> Levels { get; set; } = new();
}
