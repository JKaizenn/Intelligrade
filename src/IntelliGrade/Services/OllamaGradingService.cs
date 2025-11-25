using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IntelliGrade.App.DTOs;
using IntelliGrade.App.Interfaces;
using IntelliGrade.App.Models;
using OllamaSharp;
using OllamaSharp.Models;

namespace IntelliGrade.App.Services;

/// <summary>
/// Service for AI-assisted code analysis and grading using local Ollama models.
/// Provides detailed, evidence-based feedback on student code based on rubric criteria.
/// Uses structured prompts to ensure consistent, objective grading with specific code references.
/// </summary>
public class OllamaGradingService : IOllamaGradingService
{
    private readonly OllamaApiClient _ollama;
    private readonly string _model;

    /// <summary>
    /// Initializes the grading service with specified Ollama model and endpoint.
    /// </summary>
    /// <param name="model">LLM model name (default: llama3.2:1b)</param>
    /// <param name="endpoint">Ollama API endpoint (default: localhost:11434)</param>
    public OllamaGradingService(string model = "llama3.2:1b", string endpoint = "http://localhost:11434")
    {
        _model = model;
        _ollama = new OllamaApiClient(endpoint);
    }

    /// <summary>
    /// Checks if Ollama is running and the specified model is available locally.
    /// </summary>
    /// <returns>True if Ollama service is accessible and model exists</returns>
    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var models = await _ollama.ListLocalModels();
            return models.Any(m => m.Name.Contains(_model.Split(':')[0]));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Analyzes student code against rubric criteria using AI.
    /// Generates detailed feedback with specific evidence and suggested scores.
    /// </summary>
    /// <param name="sourceCode">Student's source code to evaluate</param>
    /// <param name="rubric">Grading rubric with criteria and point values</param>
    /// <param name="courseName">Course name for context</param>
    /// <param name="assignmentName">Assignment name for context</param>
    /// <param name="outputContents">Program execution outputs (if any)</param>
    /// <returns>Structured grading analysis with scores and reasoning</returns>
    public async Task<AiGradingResponse> AnalyzeCodeAsync(
        string sourceCode,
        Rubric rubric,
        string courseName,
        string assignmentName,
        List<string> outputContents)
    {
        try
        {
            var prompt = BuildGradingPrompt(sourceCode, rubric, courseName, assignmentName, outputContents);

            var response = new StringBuilder();
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

            var aiText = response.ToString();

            // Try JSON parsing first, fallback to text parsing
            var result = TryParseJsonResponse(aiText, rubric) ?? ParseTextResponse(aiText, rubric);

            return result;
        }
        catch (Exception ex)
        {
            return new AiGradingResponse
            {
                Success = false,
                ErrorMessage = $"AI grading failed: {ex.Message}",
                MaxPossible = rubric.TotalPoints
            };
        }
    }

    /// <summary>
    /// Constructs the structured prompt for AI grading requesting JSON output.
    /// </summary>
    private static string BuildGradingPrompt(
        string sourceCode,
        Rubric rubric,
        string courseName,
        string assignmentName,
        List<string> outputContents)
    {
        var outputSection = outputContents.Count > 0
            ? $"\n\nPROGRAM OUTPUT:\n{string.Join("\n\n", outputContents)}"
            : "";

        var criteriaSection = new StringBuilder();
        for (int i = 0; i < rubric.Criteria.Count; i++)
        {
            var criterion = rubric.Criteria[i];
            criteriaSection.AppendLine($"\nCRITERION {i + 1}: {criterion.Name}");
            criteriaSection.AppendLine($"Max Points: {criterion.MaxPoints}");
            if (!string.IsNullOrWhiteSpace(criterion.Description))
                criteriaSection.AppendLine($"Description: {criterion.Description}");

            criteriaSection.AppendLine("Scoring Levels:");
            foreach (var level in criterion.Levels)
            {
                criteriaSection.AppendLine($"  - {level.Label} ({level.Points} pts): {level.Description}");
            }
        }

        return $@"You are a grading assistant for {courseName} - {assignmentName}.

TASK: Analyze student code against rubric criteria and provide structured grading suggestions.

=== RUBRIC CRITERIA ===
{criteriaSection}

=== STUDENT'S CODE ===
{sourceCode}
{outputSection}

=== OUTPUT FORMAT ===

Respond with VALID JSON matching this structure:

{{
  ""suggestions"": [
    {{
      ""criterionName"": ""[exact criterion name from rubric]"",
      ""suggestedScore"": [points as integer],
      ""maxPoints"": [max points as integer],
      ""confidence"": ""High"" | ""Medium"" | ""Low"",
      ""ratingLevel"": ""[which level label applies]"",
      ""reasoning"": ""[2-3 sentences explaining why this score fits]"",
      ""evidence"": [
        ""[specific code element/line/feature]"",
        ""[another piece of evidence]""
      ]
    }}
  ],
  ""recommendedTotal"": [sum of suggested scores],
  ""maxPossible"": {rubric.TotalPoints},
  ""summary"": ""[2-3 sentences on strengths and improvement areas]"",
  ""overallConfidence"": ""High"" | ""Medium"" | ""Low""
}}

=== CONFIDENCE LEVELS ===
- High: Clear evidence matching specific rubric level
- Medium: Evidence present but ambiguous between levels
- Low: Limited evidence, uncertain assessment

=== CRITICAL RULES ===
- Output ONLY valid JSON (no markdown, no explanations)
- Evaluate ONLY what rubric explicitly mentions
- Reference SPECIFIC code elements in evidence
- Match scores to rubric level descriptions
- Be OBJECTIVE - base on observable code facts
- Do NOT invent criteria not in rubric";
    }

    /// <summary>
    /// Attempts to parse AI response as JSON.
    /// </summary>
    private AiGradingResponse? TryParseJsonResponse(string aiText, Rubric rubric)
    {
        try
        {
            // Extract JSON from response (AI might wrap it in markdown)
            var jsonMatch = Regex.Match(aiText, @"\{[\s\S]*\}", RegexOptions.Multiline);
            if (!jsonMatch.Success)
                return null;

            var json = jsonMatch.Value;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var response = JsonSerializer.Deserialize<AiGradingResponse>(json, options);
            if (response == null)
                return null;

            response.Success = true;
            return response;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Fallback parser for text-formatted AI responses.
    /// </summary>
    private AiGradingResponse ParseTextResponse(string aiText, Rubric rubric)
    {
        var suggestions = new List<AiCriterionSuggestion>();

        // Parse each criterion section
        var criterionPattern = @"CRITERION:\s*(.+?)\s*\n.*?SUGGESTED SCORE:\s*(\d+)/(\d+).*?\n.*?RATING LEVEL:\s*(.+?)\s*\n.*?REASONING:\s*(.+?)(?=\nEVIDENCE:|CRITERION:|RECOMMENDED TOTAL:|$)";
        var matches = Regex.Matches(aiText, criterionPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            var criterionName = match.Groups[1].Value.Trim();
            var score = int.Parse(match.Groups[2].Value);
            var maxPoints = int.Parse(match.Groups[3].Value);
            var ratingLevel = match.Groups[4].Value.Trim();
            var reasoning = match.Groups[5].Value.Trim();

            // Extract evidence
            var evidencePattern = @"-\s*(.+?)(?=\n-|\nCRITERION:|\nRECOMMENDED TOTAL:|$)";
            var evidenceMatches = Regex.Matches(aiText, evidencePattern, RegexOptions.Singleline);
            var evidence = evidenceMatches.Cast<Match>()
                .Select(m => m.Groups[1].Value.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .ToList();

            // Determine confidence based on evidence quality
            var confidence = DetermineConfidence(evidence.Count, reasoning.Length, score, maxPoints);

            suggestions.Add(new AiCriterionSuggestion
            {
                CriterionName = criterionName,
                SuggestedScore = score,
                MaxPoints = maxPoints,
                Confidence = confidence,
                RatingLevel = ratingLevel,
                Reasoning = reasoning,
                Evidence = evidence
            });
        }

        // Extract total and summary
        var totalMatch = Regex.Match(aiText, @"RECOMMENDED TOTAL:\s*(\d+)", RegexOptions.IgnoreCase);
        var recommendedTotal = totalMatch.Success ? int.Parse(totalMatch.Groups[1].Value) : suggestions.Sum(s => s.SuggestedScore);

        var summaryMatch = Regex.Match(aiText, @"SUMMARY:\s*(.+?)(?=\n===|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        var summary = summaryMatch.Success ? summaryMatch.Groups[1].Value.Trim() : "AI grading analysis completed.";

        return new AiGradingResponse
        {
            Suggestions = suggestions,
            RecommendedTotal = recommendedTotal,
            MaxPossible = rubric.TotalPoints,
            Summary = summary,
            OverallConfidence = CalculateOverallConfidence(suggestions),
            Success = true
        };
    }

    /// <summary>
    /// Determines confidence level for a single criterion based on evidence quality.
    /// </summary>
    private AiConfidence DetermineConfidence(int evidenceCount, int reasoningLength, int score, int maxPoints)
    {
        // High confidence: Multiple evidence items, detailed reasoning, clear score match
        if (evidenceCount >= 3 && reasoningLength > 100 && (score == 0 || score == maxPoints))
            return AiConfidence.High;

        // Low confidence: Minimal evidence or very short reasoning
        if (evidenceCount < 2 || reasoningLength < 50)
            return AiConfidence.Low;

        // Medium confidence: Everything else
        return AiConfidence.Medium;
    }

    /// <summary>
    /// Calculates overall confidence from individual criterion confidences.
    /// </summary>
    private AiConfidence CalculateOverallConfidence(List<AiCriterionSuggestion> suggestions)
    {
        if (suggestions.Count == 0)
            return AiConfidence.Low;

        var highCount = suggestions.Count(s => s.Confidence == AiConfidence.High);
        var lowCount = suggestions.Count(s => s.Confidence == AiConfidence.Low);

        // Overall high if majority are high confidence
        if (highCount > suggestions.Count / 2.0)
            return AiConfidence.High;

        // Overall low if any are low confidence
        if (lowCount > 0)
            return AiConfidence.Low;

        return AiConfidence.Medium;
    }

    /// <summary>
    /// Performs advanced code quality analysis for complexity, potential bugs, security issues, and code smells.
    /// This is separate from rubric grading and provides additional insights for instructor consideration.
    /// </summary>
    /// <param name="sourceCode">Source code to analyze</param>
    /// <param name="language">Programming language name</param>
    /// <returns>Advanced analysis results, or null if analysis fails</returns>
    public async Task<AdvancedAnalysis?> AnalyzeCodeQualityAsync(string sourceCode, string language)
    {
        try
        {
            var prompt = BuildCodeQualityPrompt(sourceCode, language);

            var response = new StringBuilder();
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

            var aiText = response.ToString();

            // Try to parse JSON response
            var jsonMatch = Regex.Match(aiText, @"\{[\s\S]*\}", RegexOptions.Multiline);
            if (!jsonMatch.Success)
                return null;

            var json = jsonMatch.Value;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var analysis = JsonSerializer.Deserialize<AdvancedAnalysis>(json, options);
            return analysis;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Builds the AI prompt for advanced code quality analysis.
    /// Focuses on complexity, bugs, security, and maintainability.
    /// </summary>
    private static string BuildCodeQualityPrompt(string sourceCode, string language)
    {
        return $@"You are a code quality analyzer. Analyze the following {language} code and identify issues:

=== CODE TO ANALYZE ===
{sourceCode}

=== ANALYSIS TASKS ===

1. CYCLOMATIC COMPLEXITY:
   - Count decision points: if, while, for, switch, &&, ||, ?:, catch
   - Calculate complexity score
   - Rate as Low (1-10), Medium (11-20), or High (21+)

2. POTENTIAL BUGS:
   - Off-by-one errors in loops
   - Null/undefined reference risks
   - Uninitialized variables
   - Logic errors (wrong operators, missing conditions)
   - Resource leaks (unclosed files, connections)
   - Integer overflow/underflow

3. SECURITY ISSUES:
   - SQL injection vulnerabilities
   - Command injection risks
   - Hardcoded credentials or secrets
   - Unsafe file operations
   - Missing input validation
   - Insecure random number generation
   - Path traversal vulnerabilities

4. CODE SMELLS:
   - Long methods (>50 lines)
   - Deep nesting (>3 levels)
   - Duplicate code blocks
   - Magic numbers or hardcoded strings
   - Poor variable/function naming
   - Dead/unreachable code
   - God classes/functions doing too much

=== OUTPUT FORMAT ===

Respond with VALID JSON only (no markdown, no explanations):

{{
  ""cyclomaticComplexity"": <number>,
  ""complexityRating"": ""Low"" | ""Medium"" | ""High"",
  ""potentialBugs"": [
    {{
      ""category"": ""Brief category name"",
      ""description"": ""What the issue is"",
      ""severity"": ""Info"" | ""Warning"" | ""Error"",
      ""lineReference"": ""line X"" or ""lines X-Y"",
      ""suggestion"": ""How to fix it""
    }}
  ],
  ""securityIssues"": [
    {{
      ""category"": ""Security category"",
      ""description"": ""What the vulnerability is"",
      ""severity"": ""Warning"" | ""Error"",
      ""lineReference"": ""line X"",
      ""suggestion"": ""How to fix it""
    }}
  ],
  ""codeSmells"": [
    {{
      ""category"": ""Smell type"",
      ""description"": ""What the issue is"",
      ""severity"": ""Info"" | ""Warning"",
      ""lineReference"": ""line X or function name"",
      ""suggestion"": ""How to improve""
    }}
  ]
}}

IMPORTANT:
- Output ONLY valid JSON
- Be specific - reference actual code elements
- Severity: Error = must fix, Warning = should fix, Info = consider
- Empty arrays are OK if no issues found
- Focus on objective, verifiable issues";
    }
}
