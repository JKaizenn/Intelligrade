using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntelliGrade.App.Interfaces;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.Services;

/// <summary>
/// Service for loading, parsing, and formatting rubrics from JSON files.
/// Supports both JSON and plain text rubric formats with automatic detection.
/// Provides formatting optimized for both AI analysis and UI display.
/// </summary>
public class RubricService : IRubricService
{

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

    public async Task SaveRubricAsync(Rubric rubric, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(rubric, options);
        await File.WriteAllTextAsync(filePath, json);
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
        sb.AppendLine($"ASSIGNMENT: {rubric.Name}");
        sb.AppendLine($"LANGUAGE: {rubric.Language}");
        sb.AppendLine($"TOTAL POINTS: {rubric.TotalPoints}");
        sb.AppendLine();
        sb.AppendLine("GRADING CRITERIA:");
        sb.AppendLine("=".PadRight(80, '='));

        for (int i = 0; i < rubric.Criteria.Count; i++)
        {
            var criterion = rubric.Criteria[i];
            sb.AppendLine();
            sb.AppendLine($"CRITERION {i + 1}: {criterion.Name}");
            sb.AppendLine($"Maximum Points: {criterion.MaxPoints}");

            if (!string.IsNullOrWhiteSpace(criterion.Description))
            {
                sb.AppendLine($"Description: {criterion.Description}");
            }

            sb.AppendLine();
            sb.AppendLine("Scoring Levels:");

            foreach (var level in criterion.Levels)
            {
                sb.AppendLine($"  [{level.Points}/{criterion.MaxPoints} points] {level.Label}: {level.Description}");
            }

            if (i < rubric.Criteria.Count - 1)
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

        sb.AppendLine($"{rubric.Course} - {rubric.Name}");
        sb.AppendLine($"Language: {rubric.Language}");
        sb.AppendLine($"Total Points: {rubric.TotalPoints}");
        sb.AppendLine();

        foreach (var criterion in rubric.Criteria)
        {
            sb.AppendLine($"• {criterion.Name} ({criterion.MaxPoints} pts)");

            if (!string.IsNullOrWhiteSpace(criterion.Description))
            {
                sb.AppendLine($"  {criterion.Description}");
            }

            foreach (var level in criterion.Levels)
            {
                sb.AppendLine($"    {level.Points} pts - {level.Label}: {level.Description}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
