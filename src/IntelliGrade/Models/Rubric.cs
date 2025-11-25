using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IntelliGrade.App.Models;

/// <summary>
/// A complete grading rubric with metadata and criteria.
/// </summary>
public class Rubric
{
    public string Name { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public List<Criterion> Criteria { get; set; } = new();

    [JsonIgnore]
    public string? FilePath { get; set; }
}
