using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IntelliGrade.App.Configuration;
using IntelliGrade.App.Interfaces;
using IntelliGrade.App.Models;

namespace IntelliGrade.App.Services;

/// <summary>
/// Executes student programs safely with timeout.
/// </summary>
public class ProgramRunnerService : IProgramRunnerService
{
    private readonly ExecutionConfiguration _config;

    /// <summary>
    /// Initializes the program runner with execution configuration.
    /// </summary>
    /// <param name="config">Configuration for timeouts and execution limits</param>
    public ProgramRunnerService(ExecutionConfiguration? config = null)
    {
        _config = config ?? new ExecutionConfiguration();
    }

    /// <summary>
    /// Executes a student program with timeout protection.
    /// Automatically compiles if needed (C++, C, Rust) or runs directly (Python, Java, etc.).
    /// </summary>
    /// <param name="sourceFile">Path to source code file</param>
    /// <param name="language">Programming language metadata</param>
    /// <param name="workingDirectory">Directory to execute from</param>
    /// <returns>Tuple of success status, stdout, and stderr</returns>
    public async Task<(bool success, string output, string error)> RunProgramAsync(
        string sourceFile, LanguageInfo language, string workingDirectory)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(sourceFile))
            throw new ArgumentException("Source file cannot be null or whitespace", nameof(sourceFile));
        if (language == null)
            throw new ArgumentNullException(nameof(language));
        if (string.IsNullOrWhiteSpace(workingDirectory))
            throw new ArgumentException("Working directory cannot be null or whitespace", nameof(workingDirectory));

        // For compiled languages, compile first then run
        if (NeedsCompilation(language.Name))
        {
            return await CompileAndRunAsync(sourceFile, language, workingDirectory);
        }

        // For interpreted languages, run directly
        return await RunScriptAsync(sourceFile, language, workingDirectory);
    }

    /// <summary>
    /// Determines if language requires compilation before execution.
    /// </summary>
    private bool NeedsCompilation(string languageName)
    {
        return languageName is "cpp" or "c" or "rust" or "csharp";
    }

    /// <summary>
    /// Compiles source code, then executes the resulting binary.
    /// </summary>
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
        var (compiler, arguments) = GetCompileCommand(sourceFile, language, workingDirectory);
        return await ExecuteProgramAsync(compiler, arguments, workingDirectory, "");
    }

    private async Task<(bool success, string output, string error)> RunScriptAsync(
        string sourceFile, LanguageInfo language, string workingDirectory)
    {
        // Java requires compilation of all .java files before execution
        if (language.Name == "java")
        {
            return await CompileAndRunJavaAsync(sourceFile, workingDirectory);
        }

        var (interpreter, arguments) = GetExecutionCommand(sourceFile, language);
        return await ExecuteProgramAsync(interpreter, arguments, workingDirectory, "");
    }

    /// <summary>
    /// Compiles all Java files in the directory and runs the main class.
    /// </summary>
    private async Task<(bool success, string output, string error)> CompileAndRunJavaAsync(
        string mainFile, string workingDirectory)
    {
        try
        {
            // Find all .java files in the directory
            var javaFiles = Directory.GetFiles(workingDirectory, "*.java", SearchOption.TopDirectoryOnly).ToList();

            if (javaFiles.Count == 0)
            {
                return (false, "", "No Java source files found");
            }

            // Compile all Java files
            var quotedFiles = string.Join(" ", javaFiles.Select(f => $"\"{Path.GetFileName(f)}\""));
            var compileResult = await ExecuteProgramAsync("javac", quotedFiles, workingDirectory, "");

            if (!compileResult.success)
            {
                return (false, compileResult.output, $"Java compilation failed:\n{compileResult.error}");
            }

            // Run the main class (assume file name matches class name)
            var className = Path.GetFileNameWithoutExtension(mainFile);
            return await ExecuteProgramAsync("java", className, workingDirectory,
                $"Compiled successfully:\n{compileResult.output}\n\n--- Program Output ---\n");
        }
        catch (Exception ex)
        {
            return (false, "", $"Java compile and run error: {ex.Message}");
        }
    }

    /// <summary>
    /// Core execution method with timeout protection.
    /// Captures stdout/stderr and enforces configured timeout limit.
    /// </summary>
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

            var completed = await Task.Run(() => process.WaitForExit(_config.TimeoutSeconds * 1000));

            if (!completed)
            {
                process.Kill(true);
                return (false, "", "Program execution timeout");
            }

            // Ensure all async output/error reading completes
            process.WaitForExit();

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            // If no error message but non-zero exit code, add generic error
            if (process.ExitCode != 0 && string.IsNullOrWhiteSpace(error))
            {
                error = $"Program exited with code {process.ExitCode}";
            }

            return (process.ExitCode == 0, output, error);
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
            "php" => ("php", sourceFile),
            "ruby" => ("ruby", sourceFile),
            "go" => ("go", $"run {sourceFile}"),
            _ => throw new NotSupportedException($"Language {language.Name} not supported for direct execution")
        };
    }

    /// <summary>
    /// Gets the compilation command for a source file.
    /// For C++ and C, automatically includes all source files in the directory for linking.
    /// </summary>
    private (string fileName, string arguments) GetCompileCommand(string sourceFile, LanguageInfo language, string workingDirectory)
    {
        var outputName = Path.GetFileNameWithoutExtension(sourceFile);

        return language.Name switch
        {
            "cpp" => GetMultiFileCppCommand(sourceFile, workingDirectory, outputName),
            "c" => GetMultiFileCCommand(sourceFile, workingDirectory, outputName),
            "csharp" => ("dotnet", "build"),
            "rust" => GetMultiFileRustCommand(sourceFile, workingDirectory, outputName),
            _ => throw new NotSupportedException($"Language {language.Name} not supported for compilation")
        };
    }

    /// <summary>
    /// Builds C++ compilation command including all .cpp files in the directory.
    /// Header files (.h, .hpp) are not included as they're handled via #include directives.
    /// </summary>
    private (string fileName, string arguments) GetMultiFileCppCommand(string mainFile, string workingDirectory, string outputName)
    {
        // Find all .cpp, .cc, .cxx files in the directory
        var cppExtensions = new[] { "*.cpp", "*.cc", "*.cxx" };
        var sourceFiles = new List<string>();

        foreach (var pattern in cppExtensions)
        {
            sourceFiles.AddRange(Directory.GetFiles(workingDirectory, pattern, SearchOption.TopDirectoryOnly));
        }

        // Build the compile command with all source files
        if (sourceFiles.Count == 0)
        {
            // Fallback to just the main file if no files found
            return ("g++", $"\"{mainFile}\" -o \"{outputName}\"");
        }

        // Quote each file path and join them
        var quotedFiles = string.Join(" ", sourceFiles.Select(f => $"\"{Path.GetFileName(f)}\""));
        return ("g++", $"{quotedFiles} -o \"{outputName}\"");
    }

    /// <summary>
    /// Builds C compilation command including all .c files in the directory.
    /// Header files (.h) are not included as they're handled via #include directives.
    /// </summary>
    private (string fileName, string arguments) GetMultiFileCCommand(string mainFile, string workingDirectory, string outputName)
    {
        // Find all .c files in the directory (exclude .h header files)
        var sourceFiles = Directory.GetFiles(workingDirectory, "*.c", SearchOption.TopDirectoryOnly).ToList();

        // Build the compile command with all source files
        if (sourceFiles.Count == 0)
        {
            // Fallback to just the main file if no files found
            return ("gcc", $"\"{mainFile}\" -o \"{outputName}\"");
        }

        // Quote each file path and join them
        var quotedFiles = string.Join(" ", sourceFiles.Select(f => $"\"{Path.GetFileName(f)}\""));
        return ("gcc", $"{quotedFiles} -o \"{outputName}\"");
    }

    /// <summary>
    /// Builds Rust compilation command.
    /// Rust's cargo handles multi-file projects, but for simple rustc compilation,
    /// we compile the main file which should include modules.
    /// </summary>
    private (string fileName, string arguments) GetMultiFileRustCommand(string mainFile, string workingDirectory, string outputName)
    {
        // Check if there's a Cargo.toml file (proper Rust project)
        var cargoToml = Path.Combine(workingDirectory, "Cargo.toml");
        if (File.Exists(cargoToml))
        {
            // Use cargo build for proper projects
            return ("cargo", "build --release");
        }

        // For simple single-file or module-based Rust, rustc should handle it
        // Rust uses mod declarations to include other files, not command-line arguments
        return ("rustc", $"\"{Path.GetFileName(mainFile)}\" -o \"{outputName}\"");
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
}