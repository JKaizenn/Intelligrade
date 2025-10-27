using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntelliGrade.App.Interfaces;

namespace IntelliGrade.App.Services;

/// <summary>
/// Service for loading, parsing, and formatting rubrics from JSON files.
/// Supports both JSON and plain text rubric formats with automatic detection.
/// Provides formatting optimized for both AI analysis and UI display.
/// </summary>
public class RubricService : IRubricService
{
    public class RubricRating
    {
        public decimal Points { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class RubricCriterion
    {
        public string Name { get; set; } = string.Empty;
        public decimal MaxPoints { get; set; }
        public RubricRating[] Ratings { get; set; } = Array.Empty<RubricRating>();
    }

    public class Rubric
    {
        public string Course { get; set; } = string.Empty;
        public string Assignment { get; set; } = string.Empty;
        public decimal TotalPoints { get; set; }
        public RubricCriterion[] Criteria { get; set; } = Array.Empty<RubricCriterion>();
    }

    public async Task<Rubric?> LoadRubricAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<Rubric>(json, options);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> LoadPlainTextRubricAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return string.Empty;

            return await File.ReadAllTextAsync(filePath);
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string> LoadAndFormatRubricAsync(string filePath)
    {
        // Try JSON format first
        if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            var rubric = await LoadRubricAsync(filePath);
            if (rubric != null)
                return FormatRubricForAI(rubric);
        }

        // Fall back to plain text
        return await LoadPlainTextRubricAsync(filePath);
    }

    public string FormatRubricForAI(Rubric rubric)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"COURSE: {rubric.Course}");
        sb.AppendLine($"ASSIGNMENT: {rubric.Assignment}");
        sb.AppendLine($"TOTAL POINTS: {rubric.TotalPoints}");
        sb.AppendLine();
        sb.AppendLine("GRADING CRITERIA:");
        sb.AppendLine("=".PadRight(80, '='));

        for (int i = 0; i < rubric.Criteria.Length; i++)
        {
            var criterion = rubric.Criteria[i];
            sb.AppendLine();
            sb.AppendLine($"CRITERION {i + 1}: {criterion.Name}");
            sb.AppendLine($"Maximum Points: {criterion.MaxPoints}");
            sb.AppendLine();
            sb.AppendLine("Rating Levels:");

            for (int j = 0; j < criterion.Ratings.Length; j++)
            {
                var rating = criterion.Ratings[j];
                sb.AppendLine($"  [{rating.Points}/{criterion.MaxPoints} points] {rating.Description}");
            }

            if (i < rubric.Criteria.Length - 1)
            {
                sb.AppendLine();
                sb.AppendLine("-".PadRight(80, '-'));
            }
        }

        sb.AppendLine();
        sb.AppendLine("=".PadRight(80, '='));

        return sb.ToString();
    }

    public string FormatRubricForDisplay(Rubric rubric)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{rubric.Course} - {rubric.Assignment}");
        sb.AppendLine($"Total Points: {rubric.TotalPoints}");
        sb.AppendLine();

        foreach (var criterion in rubric.Criteria)
        {
            sb.AppendLine($"• {criterion.Name} ({criterion.MaxPoints} pts)");

            foreach (var rating in criterion.Ratings)
            {
                sb.AppendLine($"    {rating.Points} pts: {rating.Description}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
