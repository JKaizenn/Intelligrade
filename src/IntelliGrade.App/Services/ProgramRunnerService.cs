using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.Services;

/// <summary>
/// Executes student programs safely with timeout.
/// </summary>
public class ProgramRunnerService
{
    private const int TimeoutSeconds = 30;

    public async Task<(bool success, string output, string error)> RunProgramAsync(
        string sourceFile, LanguageInfo language, string workingDirectory)
    {
        // For compiled languages, compile first then run
        if (NeedsCompilation(language.Name))
        {
            return await CompileAndRunAsync(sourceFile, language, workingDirectory);
        }

        // For interpreted languages, run directly
        return await RunScriptAsync(sourceFile, language, workingDirectory);
    }

    private bool NeedsCompilation(string languageName)
    {
        return languageName is "cpp" or "c" or "rust" or "csharp";
    }

    private async Task<(bool success, string output, string error)> CompileAndRunAsync(
        string sourceFile, LanguageInfo language, string workingDirectory)
    {
        try
        {
            // Step 1: Compile
            var (compileSuccess, compileOutput, compileError) = await CompileAsync(sourceFile, language, workingDirectory);

            if (!compileSuccess)
            {
                return (false, compileOutput, $"Compilation failed:\n{compileError}");
            }

            // Step 2: Run the compiled program
            var executablePath = GetExecutablePath(sourceFile, language, workingDirectory);
            return await ExecuteProgramAsync(executablePath, "", workingDirectory,
                $"Compiled successfully:\n{compileOutput}\n\n--- Program Output ---\n");
        }
        catch (Exception ex)
        {
            return (false, "", $"Compile and run error: {ex.Message}");
        }
    }

    private async Task<(bool success, string output, string error)> CompileAsync(
        string sourceFile, LanguageInfo language, string workingDirectory)
    {
        var (compiler, arguments) = GetCompileCommand(sourceFile, language);
        return await ExecuteProgramAsync(compiler, arguments, workingDirectory, "");
    }

    private async Task<(bool success, string output, string error)> RunScriptAsync(
        string sourceFile, LanguageInfo language, string workingDirectory)
    {
        var (interpreter, arguments) = GetExecutionCommand(sourceFile, language);
        return await ExecuteProgramAsync(interpreter, arguments, workingDirectory, "");
    }

    private async Task<(bool success, string output, string error)> ExecuteProgramAsync(
        string fileName, string arguments, string workingDirectory, string outputPrefix)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(outputPrefix))
            {
                outputBuilder.Append(outputPrefix);
            }

            process.OutputDataReceived += (_, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var completed = await Task.Run(() => process.WaitForExit(TimeoutSeconds * 1000));

            if (!completed)
            {
                process.Kill(true);
                return (false, "", "Program execution timeout");
            }

            return (process.ExitCode == 0, outputBuilder.ToString(), errorBuilder.ToString());
        }
        catch (Exception ex)
        {
            return (false, "", $"Execution error: {ex.Message}");
        }
    }

    private (string fileName, string arguments) GetExecutionCommand(string sourceFile, LanguageInfo language)
    {
        return language.Name switch
        {
            "python" => ("python3", sourceFile),
            "javascript" => ("node", sourceFile),
            "java" => GetJavaCommand(sourceFile),
            "php" => ("php", sourceFile),
            "ruby" => ("ruby", sourceFile),
            "go" => ("go", $"run {sourceFile}"),
            _ => throw new NotSupportedException($"Language {language.Name} not supported for direct execution")
        };
    }

    private (string fileName, string arguments) GetCompileCommand(string sourceFile, LanguageInfo language)
    {
        var outputName = Path.GetFileNameWithoutExtension(sourceFile);

        return language.Name switch
        {
            "cpp" => ("g++", $"\"{sourceFile}\" -o \"{outputName}\""),
            "c" => ("gcc", $"\"{sourceFile}\" -o \"{outputName}\""),
            "csharp" => ("dotnet", "build"),
            "rust" => ("rustc", $"\"{sourceFile}\" -o \"{outputName}\""),
            _ => throw new NotSupportedException($"Language {language.Name} not supported for compilation")
        };
    }

    private string GetExecutablePath(string sourceFile, LanguageInfo language, string workingDirectory)
    {
        var baseName = Path.GetFileNameWithoutExtension(sourceFile);

        return language.Name switch
        {
            "cpp" or "c" or "rust" => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(workingDirectory, $"{baseName}.exe")
                : Path.Combine(workingDirectory, baseName),
            "csharp" => "dotnet",
            _ => throw new NotSupportedException($"Language {language.Name} does not produce executables")
        };
    }

    private (string, string) GetJavaCommand(string sourceFile)
    {
        var className = Path.GetFileNameWithoutExtension(sourceFile);
        return ("java", className);
    }
}