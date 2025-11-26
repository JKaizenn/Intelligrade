using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
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
    private readonly int _timeoutSeconds;

    /// <summary>
    /// Determines if the current model is a small model (1b, 3b, or under 1b parameters).
    /// Small models need simplified prompts for better performance.
    /// </summary>
    private bool IsSmallModel =>
        _model.Contains(":1b") ||
        _model.Contains(":3b") ||
        _model.Contains("0.6b") ||
        _model.Contains("0.5b") ||
        _model.Contains(":1B") ||
        _model.Contains(":3B");

    /// <summary>
    /// Event raised when AI generation progress updates.
    /// Reports the current number of tokens generated.
    /// </summary>
    public event Action<int>? OnProgressUpdate;

    /// <summary>
    /// Initializes the grading service with specified Ollama model and endpoint.
    /// </summary>
    /// <param name="model">LLM model name (default: llama3.2:1b)</param>
    /// <param name="endpoint">Ollama API endpoint (default: localhost:11434)</param>
    /// <param name="timeoutSeconds">Request timeout in seconds (default: 90)</param>
    public OllamaGradingService(
        string model = "llama3.2:1b",
        string endpoint = "http://localhost:11434",
        int timeoutSeconds = 90)
    {
        _model = model;
        _ollama = new OllamaApiClient(endpoint);
        _timeoutSeconds = timeoutSeconds;
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
        using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));

        try
        {
            var prompt = IsSmallModel
                ? BuildLiteGradingPrompt(sourceCode, rubric, courseName, assignmentName, outputContents)
                : BuildGradingPrompt(sourceCode, rubric, courseName, assignmentName, outputContents);

            var response = new StringBuilder();
            var request = new GenerateRequest
            {
                Model = _model,
                Prompt = prompt,
                Options = new RequestOptions
                {
                    NumPredict = IsSmallModel ? 800 : 1500,
                    NumCtx = IsSmallModel ? 2048 : 4096,
                    Temperature = IsSmallModel ? 0.2f : 0.3f,
                    TopP = 0.9f,
                    RepeatPenalty = 1.1f
                }
            };

            var tokenCount = 0;
            await foreach (var chunk in _ollama.Generate(request, cts.Token))
            {
                if (chunk?.Response != null)
                {
                    response.Append(chunk.Response);
                    tokenCount++;

                    // Report progress every 10 tokens to avoid UI thrashing
                    if (tokenCount % 10 == 0)
                    {
                        OnProgressUpdate?.Invoke(tokenCount);
                    }
                }
            }

            var aiText = response.ToString();

            // Try JSON parsing first, fallback to text parsing
            var result = IsSmallModel
                ? TryParseLiteJsonResponse(aiText, rubric) ?? TryParseJsonResponse(aiText, rubric)
                : TryParseJsonResponse(aiText, rubric);

            if (result != null)
            {
                result.RawAiResponse = aiText;
                result.ParserUsed = IsSmallModel ? "LiteJSON" : "JSON";
            }
            else
            {
                result = ParseTextResponse(aiText, rubric);
                result.RawAiResponse = aiText;
                result.ParserUsed = "Text";
            }

            return result;
        }
        catch (System.OperationCanceledException)
        {
            return new AiGradingResponse
            {
                Success = false,
                ErrorMessage = $"AI analysis timed out after {_timeoutSeconds} seconds. Try using a smaller model or simpler code.",
                MaxPossible = rubric.TotalPoints
            };
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
    /// Builds a simplified prompt optimized for small language models (1b-3b parameters).
    /// Reduces token count by omitting detailed level descriptions.
    /// </summary>
    private static string BuildLiteGradingPrompt(
        string sourceCode,
        Rubric rubric,
        string courseName,
        string assignmentName,
        List<string> outputContents)
    {
        var outputSection = outputContents.Count > 0
            ? $"\n\nOUTPUT:\n{string.Join("\n", outputContents.Take(500))}"
            : "";

        // Simplified criteria list - just names and max points
        var criteriaList = string.Join("\n", rubric.Criteria.Select(c =>
            $"- {c.Name}: {c.MaxPoints} points"));

        // Truncate source code if too long (keep first 150 lines)
        var codeLines = sourceCode.Split('\n');
        var truncatedCode = codeLines.Length > 150
            ? string.Join("\n", codeLines.Take(150)) + "\n... (truncated)"
            : sourceCode;

        return $@"Grade this {courseName} {assignmentName} code.

CRITERIA (Total: {rubric.TotalPoints} points):
{criteriaList}

CODE:
{truncatedCode}
{outputSection}

Respond with JSON only:
{{
  ""scores"": [
    {{""criterion"": ""[name]"", ""score"": [points], ""reason"": ""[1 sentence]""}}
  ],
  ""total"": [sum],
  ""summary"": ""[1-2 sentences]""
}}";
    }

    /// <summary>
    /// Attempts to parse AI response as JSON.
    /// </summary>
    private AiGradingResponse? TryParseJsonResponse(string aiText, Rubric rubric)
    {
        string? extractedJson = null;
        try
        {
            // Extract JSON from response (AI might wrap it in markdown code blocks)
            // First try to extract from markdown code blocks
            var markdownMatch = Regex.Match(aiText, @"```(?:json)?\s*(\{[\s\S]*?\})\s*```", RegexOptions.Multiline);

            if (markdownMatch.Success && markdownMatch.Groups.Count > 1)
            {
                extractedJson = markdownMatch.Groups[1].Value;
            }
            else
            {
                // Use brace-balanced extraction to find complete JSON object
                extractedJson = ExtractJsonObject(aiText);
                if (extractedJson == null)
                    return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
            };

            var response = JsonSerializer.Deserialize<AiGradingResponse>(extractedJson, options);
            if (response == null)
                return null;

            // Check if suggestions were actually parsed
            if (response.Suggestions == null || response.Suggestions.Count == 0)
            {
                return null;
            }

            // Recalculate total from suggestions to fix any AI miscalculation
            response.RecommendedTotal = response.Suggestions.Sum(s => s.SuggestedScore);
            response.Success = true;
            return response;
        }
        catch (Exception ex)
        {
            // For debugging: create a response with error details
            return new AiGradingResponse
            {
                Success = false,
                ErrorMessage = $"JSON parse error: {ex.Message}",
                RawAiResponse = extractedJson ?? aiText,
                ParserUsed = "JSON (failed)",
                MaxPossible = rubric.TotalPoints
            };
        }
    }

    /// <summary>
    /// Extracts a complete JSON object from text using brace balancing.
    /// Finds the first '{' and matches it with the corresponding '}'.
    /// </summary>
    private static string? ExtractJsonObject(string text)
    {
        int startIndex = text.IndexOf('{');
        if (startIndex == -1)
            return null;

        int braceCount = 0;
        bool inString = false;
        bool escapeNext = false;

        for (int i = startIndex; i < text.Length; i++)
        {
            char c = text[i];

            if (escapeNext)
            {
                escapeNext = false;
                continue;
            }

            if (c == '\\')
            {
                escapeNext = true;
                continue;
            }

            if (c == '"')
            {
                inString = !inString;
                continue;
            }

            if (!inString)
            {
                if (c == '{')
                    braceCount++;
                else if (c == '}')
                {
                    braceCount--;
                    if (braceCount == 0)
                    {
                        // Found matching closing brace
                        return text.Substring(startIndex, i - startIndex + 1);
                    }
                }
            }
        }

        return null;
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
    /// Attempts to parse the simplified JSON response from lite prompts.
    /// </summary>
    private AiGradingResponse? TryParseLiteJsonResponse(string aiText, Rubric rubric)
    {
        try
        {
            var extractedJson = ExtractJsonObject(aiText);
            if (extractedJson == null)
                return null;

            using var doc = JsonDocument.Parse(extractedJson);
            var root = doc.RootElement;

            var suggestions = new List<AiCriterionSuggestion>();

            if (root.TryGetProperty("scores", out var scoresElement))
            {
                foreach (var scoreItem in scoresElement.EnumerateArray())
                {
                    var criterionName = scoreItem.GetProperty("criterion").GetString() ?? "";

                    // Handle score as either number or array
                    int score = 0;
                    if (scoreItem.TryGetProperty("score", out var scoreEl))
                    {
                        if (scoreEl.ValueKind == JsonValueKind.Number)
                        {
                            score = scoreEl.GetInt32();
                        }
                        else if (scoreEl.ValueKind == JsonValueKind.Array && scoreEl.GetArrayLength() > 0)
                        {
                            // AI sometimes returns score as an array - take first element
                            score = scoreEl[0].GetInt32();
                        }
                    }

                    var reason = scoreItem.TryGetProperty("reason", out var reasonEl)
                        ? reasonEl.GetString() ?? ""
                        : "";

                    // Find matching criterion from rubric
                    var criterion = rubric.Criteria.FirstOrDefault(c =>
                        c.Name.Equals(criterionName, StringComparison.OrdinalIgnoreCase));

                    suggestions.Add(new AiCriterionSuggestion
                    {
                        CriterionName = criterionName,
                        SuggestedScore = score,
                        MaxPoints = criterion?.MaxPoints ?? score,
                        Confidence = AiConfidence.Medium,
                        RatingLevel = "Auto",
                        Reasoning = reason,
                        Evidence = new List<string>()
                    });
                }
            }

            var summary = root.TryGetProperty("summary", out var summaryEl)
                ? summaryEl.GetString() ?? ""
                : "";

            // Calculate total from suggestions instead of trusting AI's total
            var calculatedTotal = suggestions.Sum(s => s.SuggestedScore);

            return new AiGradingResponse
            {
                Success = true,
                Suggestions = suggestions,
                RecommendedTotal = calculatedTotal,
                MaxPossible = rubric.TotalPoints,
                Summary = summary,
                OverallConfidence = AiConfidence.Medium
            };
        }
        catch
        {
            return null;
        }
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
        using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));

        try
        {
            var prompt = BuildCodeQualityPrompt(sourceCode, language);

            var response = new StringBuilder();
            var request = new GenerateRequest
            {
                Model = _model,
                Prompt = prompt,
                Options = new RequestOptions
                {
                    NumPredict = 1000,
                    NumCtx = 4096,
                    Temperature = 0.2f,
                    TopP = 0.9f,
                    RepeatPenalty = 1.1f
                }
            };

            var tokenCount = 0;
            await foreach (var chunk in _ollama.Generate(request, cts.Token))
            {
                if (chunk?.Response != null)
                {
                    response.Append(chunk.Response);
                    tokenCount++;

                    // Report progress every 10 tokens
                    if (tokenCount % 10 == 0)
                    {
                        OnProgressUpdate?.Invoke(tokenCount);
                    }
                }
            }

            var aiText = response.ToString();

            // Try to parse JSON response
            var extractedJson = ExtractJsonObject(aiText);
            if (extractedJson == null)
                return null;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var analysis = JsonSerializer.Deserialize<AdvancedAnalysis>(extractedJson, options);
            return analysis;
        }
        catch (System.OperationCanceledException)
        {
            return null;  // Timeout returns null for optional advanced analysis
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Builds the AI prompt for advanced code quality analysis.
    /// Simplified for better performance with smaller models.
    /// </summary>
    private static string BuildCodeQualityPrompt(string sourceCode, string language)
    {
        // Truncate very long code
        var codeLines = sourceCode.Split('\n');
        var truncatedCode = codeLines.Length > 200
            ? string.Join("\n", codeLines.Take(200)) + "\n... (truncated)"
            : sourceCode;

        return $@"Analyze this {language} code for quality issues.

CODE:
{truncatedCode}

Identify:
1. Cyclomatic complexity (count decision points, rate Low/Medium/High)
2. Potential bugs (off-by-one, null risks, logic errors)
3. Security issues (injection, hardcoded secrets, unsafe operations)
4. Code smells (long methods, deep nesting, magic numbers)

Respond with JSON only:
{{
  ""cyclomaticComplexity"": [number],
  ""complexityRating"": ""Low|Medium|High"",
  ""potentialBugs"": [{{""description"": ""..."", ""severity"": ""Warning|Error""}}],
  ""securityIssues"": [{{""description"": ""..."", ""severity"": ""Warning|Error""}}],
  ""codeSmells"": [{{""description"": ""..."", ""severity"": ""Info|Warning""}}]
}}";
    }

    /// <summary>
    /// Analyzes code using the specified analysis mode configuration.
    /// This is the preferred method for grading with mode selection.
    /// </summary>
    /// <param name="sourceCode">Student's source code</param>
    /// <param name="rubric">Grading rubric</param>
    /// <param name="courseName">Course name</param>
    /// <param name="assignmentName">Assignment name</param>
    /// <param name="outputContents">Program outputs (if any)</param>
    /// <param name="modeConfig">Analysis mode configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Grading response with optional advanced analysis</returns>
    public async Task<AiGradingResponse> AnalyzeWithModeAsync(
        string sourceCode,
        Rubric rubric,
        string courseName,
        string assignmentName,
        List<string> outputContents,
        AnalysisModeConfig modeConfig,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(modeConfig.TimeoutSeconds));

        try
        {
            // Build appropriate prompt based on mode
            var prompt = modeConfig.UseLitePrompt
                ? BuildLiteGradingPrompt(sourceCode, rubric, courseName, assignmentName, outputContents)
                : BuildGradingPrompt(sourceCode, rubric, courseName, assignmentName, outputContents);

            var response = new StringBuilder();
            var request = new GenerateRequest
            {
                Model = _model,
                Prompt = prompt,
                Options = new RequestOptions
                {
                    NumPredict = modeConfig.MaxTokens,
                    NumCtx = modeConfig.ContextWindow,
                    Temperature = modeConfig.Temperature,
                    TopP = 0.9f,
                    RepeatPenalty = 1.1f
                }
            };

            // Track progress
            var tokenCount = 0;
            await foreach (var chunk in _ollama.Generate(request, cts.Token))
            {
                if (chunk?.Response != null)
                {
                    response.Append(chunk.Response);
                    tokenCount++;

                    // Report progress every 10 tokens
                    if (tokenCount % 10 == 0)
                    {
                        OnProgressUpdate?.Invoke(tokenCount);
                    }
                }
            }

            var aiText = response.ToString();

            // Parse response based on prompt type
            AiGradingResponse? result;
            string parserUsed;

            if (modeConfig.UseLitePrompt)
            {
                result = TryParseLiteJsonResponse(aiText, rubric);
                parserUsed = result != null ? "LiteJSON" : "Text";
            }
            else
            {
                result = TryParseJsonResponse(aiText, rubric);
                parserUsed = result != null ? "JSON" : "Text";
            }

            // Fallback to text parsing
            if (result == null)
            {
                result = ParseTextResponse(aiText, rubric);
            }

            result.RawAiResponse = aiText;
            result.ParserUsed = parserUsed;

            // Run advanced analysis if mode requires it
            if (modeConfig.IncludeAdvancedAnalysis && result.Success)
            {
                try
                {
                    var advancedAnalysis = await AnalyzeCodeQualityAsync(sourceCode, "code");
                    result.AdvancedAnalysis = advancedAnalysis;
                }
                catch
                {
                    // Advanced analysis is optional - don't fail if it errors
                }
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return new AiGradingResponse
            {
                Success = false,
                ErrorMessage = $"Analysis timed out after {modeConfig.TimeoutSeconds} seconds. Try Fast mode for quicker results.",
                MaxPossible = rubric.TotalPoints
            };
        }
        catch (Exception ex)
        {
            return new AiGradingResponse
            {
                Success = false,
                ErrorMessage = $"AI analysis failed: {ex.Message}",
                MaxPossible = rubric.TotalPoints
            };
        }
    }
}
