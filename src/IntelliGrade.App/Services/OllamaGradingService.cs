using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OllamaSharp;
using OllamaSharp.Models;

namespace IntelliGrade.App.Services;

/// <summary>
/// Service for AI-powered code analysis and grading using local Ollama models.
/// Provides detailed, evidence-based feedback on student code based on rubric criteria.
/// Uses structured prompts to ensure consistent, objective grading with specific code references.
/// </summary>
public class OllamaGradingService
{
    private readonly OllamaApiClient _ollama;
    private readonly string _model;

    /// <summary>
    /// Initializes a new instance of the OllamaGradingService.
    /// </summary>
    /// <param name="model">The Ollama model to use for grading (default: llama3.2:1b)</param>
    /// <param name="endpoint">The Ollama API endpoint (default: http://localhost:11434)</param>
    public OllamaGradingService(string model = "llama3.2:1b", string endpoint = "http://localhost:11434")
    {
        _model = model;
        _ollama = new OllamaApiClient(endpoint);
    }

    /// <summary>
    /// Checks if the Ollama service is available and the model is installed.
    /// </summary>
    /// <returns>True if available, false otherwise</returns>
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

    public async Task<string> AnalyzeCodeAsync(
        string sourceCode, 
        string rubric, 
        string courseName, 
        string assignmentName,
        List<string> outputContents)
    {
        var prompt = BuildGradingPrompt(sourceCode, rubric, courseName, assignmentName, outputContents);
        
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

    private string BuildGradingPrompt(
        string sourceCode,
        string rubric,
        string courseName,
        string assignmentName,
        List<string> outputContents)
    {
        var outputSection = outputContents.Count > 0
            ? $"\n\nPROGRAM OUTPUT:\n{string.Join("\n\n", outputContents)}"
            : "";

        return $@"You are a grading assistant for {courseName} - {assignmentName}.

YOUR SINGLE TASK: Analyze the student's code against each rubric criterion and suggest appropriate scores with detailed reasoning.

=== RUBRIC CRITERIA ===
{rubric}

=== STUDENT'S CODE ===
{sourceCode}
{outputSection}

=== YOUR GRADING INSTRUCTIONS ===

For EACH criterion in the rubric above:
1. Carefully examine the student's code for evidence related to that criterion
2. Compare what you find against the rating descriptions in the rubric
3. Select the most appropriate rating level based on the evidence
4. Provide specific reasoning that references actual code elements

GRADING FORMAT (use exactly this format):

CRITERION: [Criterion Name]
SUGGESTED SCORE: [Points]/[Max Points]
RATING LEVEL: [The rating description that matches]
REASONING: [Detailed explanation with specific evidence from the code]
EVIDENCE:
- [Specific code element, line, or feature supporting your assessment]
- [Another piece of evidence]
- [Continue listing concrete evidence]

[Repeat for each criterion]

---
RECOMMENDED TOTAL: [Sum of suggested points]
MAXIMUM POSSIBLE: [Total max points from rubric]

SUMMARY:
[2-3 sentences summarizing the main strengths and areas for improvement]

=== CRITICAL RULES ===
- ONLY evaluate based on what the rubric explicitly mentions
- Provide SPECIFIC evidence from the code (mention actual functions, variables, patterns)
- If code quality is not in the rubric, do NOT comment on it
- Match your score to the rating description that best fits the evidence
- Be OBJECTIVE - base everything on observable facts in the code
- Do NOT invent criteria not in the rubric
- Focus ONLY on grading - no pleasantries or meta-commentary";
    }
}