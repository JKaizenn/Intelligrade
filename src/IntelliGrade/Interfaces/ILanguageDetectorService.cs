using System.Collections.Generic;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.Interfaces;

/// <summary>
/// Service for detecting programming languages in source code directories.
/// </summary>
public interface ILanguageDetectorService
{
    /// <summary>
    /// Detects all programming languages present in the specified directory.
    /// </summary>
    /// <param name="directory">Directory path to scan for source files</param>
    /// <returns>List of detected languages</returns>
    List<LanguageInfo> DetectLanguages(string directory);

    /// <summary>
    /// Gets all source files for a specific language in a directory.
    /// </summary>
    /// <param name="directory">Directory path to scan</param>
    /// <param name="language">Language to filter by</param>
    /// <returns>Array of relative file paths</returns>
    string[] GetSourceFiles(string directory, LanguageInfo language);
}
