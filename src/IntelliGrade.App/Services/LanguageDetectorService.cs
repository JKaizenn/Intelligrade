using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntelliGrade.App.Interfaces;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.Services;

/// <summary>
/// Detects programming languages from source files in a directory.
/// Single Responsibility: Language detection only.
/// </summary>
public class LanguageDetectorService : ILanguageDetectorService
{
    private static readonly Dictionary<string, LanguageInfo> SupportedLanguages = new()
    {
        ["python"] = new("python", "Python", new[] { ".py" }, "python3"),
        ["csharp"] = new("csharp", "C#", new[] { ".cs" }, "dotnet run"),
        ["javascript"] = new("javascript", "JavaScript", new[] { ".js" }, "node"),
        ["html"] = new("html", "HTML", new[] { ".html", ".htm" }, ""),
        ["css"] = new("css", "CSS", new[] { ".css" }, ""),
        ["java"] = new("java", "Java", new[] { ".java" }, "java"),
        ["cpp"] = new("cpp", "C++", new[] { ".cpp", ".cc", ".cxx" }, "g++"),
        ["c"] = new("c", "C", new[] { ".c" }, "gcc"),
        ["php"] = new("php", "PHP", new[] { ".php" }, "php"),
        ["ruby"] = new("ruby", "Ruby", new[] { ".rb" }, "ruby"),
        ["go"] = new("go", "Go", new[] { ".go" }, "go run"),
        ["rust"] = new("rust", "Rust", new[] { ".rs" }, "rustc")
    };

    public List<LanguageInfo> DetectLanguages(string directory)
    {
        var detected = new List<LanguageInfo>();

        foreach (var (_, langInfo) in SupportedLanguages)
        {
            foreach (var ext in langInfo.Extensions)
            {
                if (Directory.GetFiles(directory, $"*{ext}", SearchOption.AllDirectories).Length > 0)
                {
                    detected.Add(langInfo);
                    break;
                }
            }
        }

        return detected;
    }

    public string[] GetSourceFiles(string directory, LanguageInfo language)
    {
        var files = new List<string>();
        foreach (var ext in language.Extensions)
        {
            files.AddRange(Directory.GetFiles(directory, $"*{ext}", SearchOption.AllDirectories));
        }
        return files.Select(f => Path.GetRelativePath(directory, f)).ToArray();
    }
}