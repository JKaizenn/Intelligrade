using System;
using System.IO;

namespace IntelliGrade.App.Models;

/// <summary>
/// Legacy grading configuration for backward compatibility.
/// Use Configuration.AppConfiguration for new code.
/// </summary>
[Obsolete("Use Configuration.AppConfiguration instead")]
public class GradingConfig
{
    public string RubricDirectory { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bin", "rubrics");

    public string OllamaModel { get; } = "llama3.2:1b";
}