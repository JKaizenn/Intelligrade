using System;
using System.IO;

namespace IntelliGrade.App.Models;

/// <summary>
/// Grading configuration settings.
/// </summary>
public class GradingConfig
{
    public string RubricDirectory { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bin", "rubrics");

    public string OllamaModel { get; } = "qwen3-coder:30b";
}