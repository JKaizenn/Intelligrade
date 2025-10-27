using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OllamaSharp;
using OllamaSharp.Models;

namespace IntelliGrade.App.Services;

public class RubricConverterService
{
    private readonly OllamaApiClient _ollama;
    private readonly string _model;

    public RubricConverterService(string model = "llama3.2:1b", string endpoint = "http://localhost:11434")
    {
        _model = model;
        _ollama = new OllamaApiClient(endpoint);
    }

    public async Task<string?> ConvertToJsonAsync(string plainTextRubric, string courseName, string assignmentName)
    {
        try
        {
            var response = await GetAIResponseAsync(BuildConversionPrompt(plainTextRubric, courseName, assignmentName));
            if (string.IsNullOrWhiteSpace(response)) return null;

            var json = ExtractJson(response);
            return IsValidJson(json) ? json : null;
        }
        catch { return null; }
    }

    public string? ConvertSimpleRubric(string plainTextRubric, string courseName, string assignmentName)
    {
        try
        {
            var rubric = new RubricService.Rubric
            {
                Course = courseName,
                Assignment = assignmentName,
                TotalPoints = ExtractTotalPoints(plainTextRubric),
                Criteria = ParseCriteria(plainTextRubric)
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(rubric, options);
        }
        catch
        {
            return null;
        }
    }

    private string BuildConversionPrompt(string plainTextRubric, string courseName, string assignmentName)
    {
        return $@"Convert the following grading rubric into a structured JSON format.

COURSE: {courseName}
ASSIGNMENT: {assignmentName}

RUBRIC TEXT:
{plainTextRubric}

OUTPUT REQUIREMENTS:
- Return ONLY valid JSON, no explanations
- Use this exact structure:
{{
  ""course"": ""{courseName}"",
  ""assignment"": ""{assignmentName}"",
  ""totalPoints"": <number>,
  ""criteria"": [
    {{
      ""name"": ""Criterion Name"",
      ""maxPoints"": <number>,
      ""ratings"": [
        {{
          ""points"": <number>,
          ""description"": ""Description of what earns these points""
        }}
      ]
    }}
  ]
}}

RULES:
- Extract all grading criteria from the rubric
- For each criterion, list all point values/ratings from highest to lowest
- Calculate totalPoints by summing maxPoints of all criteria
- If point values aren't clear, estimate based on percentages or context
- Preserve the original criterion names and descriptions
- Return ONLY the JSON object, nothing else";
    }

    private async Task<string> GetAIResponseAsync(string prompt)
    {
        var response = new System.Text.StringBuilder();
        var request = new GenerateRequest
        {
            Model = _model,
            Prompt = prompt
        };

        await foreach (var chunk in _ollama.Generate(request))
        {
            if (chunk?.Response != null)
            {
                response.Append(chunk.Response);
            }
        }

        return response.ToString();
    }

    private string ExtractJson(string text)
    {
        // Try to find JSON object in the response
        var jsonMatch = Regex.Match(text, @"\{[\s\S]*\}", RegexOptions.Multiline);
        return jsonMatch.Success ? jsonMatch.Value : text.Trim();
    }

    private bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            using var doc = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private decimal ExtractTotalPoints(string rubric)
    {
        // Look for patterns like "Total: 100 points" or "100 points total"
        var totalMatch = Regex.Match(rubric, @"(?:total|maximum)[\s:]+(\d+)\s*(?:points?|pts?)", RegexOptions.IgnoreCase);
        if (totalMatch.Success && decimal.TryParse(totalMatch.Groups[1].Value, out var total))
            return total;

        // Look for "/100" or "out of 100"
        var outOfMatch = Regex.Match(rubric, @"(?:out of|/)\s*(\d+)", RegexOptions.IgnoreCase);
        if (outOfMatch.Success && decimal.TryParse(outOfMatch.Groups[1].Value, out var outOf))
            return outOf;

        return 100; // Default
    }

    private RubricService.RubricCriterion[] ParseCriteria(string rubric)
    {
        var criteria = new System.Collections.Generic.List<RubricService.RubricCriterion>();

        // Split by common section delimiters
        var sections = Regex.Split(rubric, @"\n\s*\n|\r\n\s*\r\n");

        foreach (var section in sections)
        {
            if (string.IsNullOrWhiteSpace(section))
                continue;

            var criterion = ParseCriterion(section);
            if (criterion != null)
                criteria.Add(criterion);
        }

        return criteria.Count > 0 ? criteria.ToArray() : CreateDefaultCriteria();
    }

    private RubricService.RubricCriterion? ParseCriterion(string section)
    {
        // Look for criterion name at the start
        var lines = section.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
            return null;

        var name = lines[0].Trim();
        var ratings = new System.Collections.Generic.List<RubricService.RubricRating>();

        // Look for point values
        foreach (var line in lines)
        {
            var pointMatch = Regex.Match(line, @"(\d+)\s*(?:points?|pts?)");
            if (pointMatch.Success && decimal.TryParse(pointMatch.Groups[1].Value, out var points))
            {
                ratings.Add(new RubricService.RubricRating
                {
                    Points = points,
                    Description = line.Trim()
                });
            }
        }

        if (ratings.Count == 0)
            return null;

        return new RubricService.RubricCriterion
        {
            Name = name,
            MaxPoints = ratings.Count > 0 ? ratings[0].Points : 0,
            Ratings = ratings.ToArray()
        };
    }

    private RubricService.RubricCriterion[] CreateDefaultCriteria()
    {
        return new[]
        {
            new RubricService.RubricCriterion
            {
                Name = "Overall Quality",
                MaxPoints = 100,
                Ratings = new[]
                {
                    new RubricService.RubricRating { Points = 100, Description = "Excellent work" },
                    new RubricService.RubricRating { Points = 80, Description = "Good work with minor issues" },
                    new RubricService.RubricRating { Points = 60, Description = "Acceptable work with several issues" },
                    new RubricService.RubricRating { Points = 0, Description = "Incomplete or does not meet requirements" }
                }
            }
        };
    }
}
