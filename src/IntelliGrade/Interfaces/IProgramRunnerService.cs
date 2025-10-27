using System.Threading.Tasks;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.Interfaces;

/// <summary>
/// Service for safely executing student code with timeout protection.
/// </summary>
public interface IProgramRunnerService
{
    /// <summary>
    /// Executes a student's program with timeout protection.
    /// </summary>
    /// <param name="sourceFile">Path to source file</param>
    /// <param name="language">Programming language information</param>
    /// <param name="workingDirectory">Working directory for execution</param>
    /// <returns>Tuple containing success status, output, and error messages</returns>
    Task<(bool success, string output, string error)> RunProgramAsync(
        string sourceFile,
        LanguageInfo language,
        string workingDirectory);
}
