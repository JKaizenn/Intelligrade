using System;
using System.IO;

namespace IntelliGrade.App.Models;

public class GradingConfig
{
    public string RubricDirectory { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "bin", "rubrics");

    public string OllamaModel { get; } = "llama3.2:1b";
}