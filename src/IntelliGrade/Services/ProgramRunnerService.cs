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
    /// <param name="standardInput">Optional standard input to provide to the program</param>
    /// <returns>Tuple of success status, stdout, and stderr</returns>
    public async Task<(bool success, string output, string error)> RunProgramAsync(
        string sourceFile, LanguageInfo language, string workingDirectory, string? standardInput = null)
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
            return await CompileAndRunAsync(sourceFile, language, workingDirectory, standardInput);
        }

        // For interpreted languages, run directly
        return await RunScriptAsync(sourceFile, language, workingDirectory, standardInput);
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
        string sourceFile, LanguageInfo language, string workingDirectory, string? standardInput = null)
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
            var arguments = executablePath == "dotnet" ? "run" : "";
            return await ExecuteProgramAsync(executablePath, arguments, workingDirectory,
                $"Compiled successfully:\n{compileOutput}\n\n--- Program Output ---\n", standardInput);
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
        string sourceFile, LanguageInfo language, string workingDirectory, string? standardInput = null)
    {
        // Java requires compilation of all .java files before execution
        if (language.Name == "java")
        {
            return await CompileAndRunJavaAsync(sourceFile, workingDirectory, standardInput);
        }

        var (interpreter, arguments) = GetExecutionCommand(sourceFile, language);
        return await ExecuteProgramAsync(interpreter, arguments, workingDirectory, "", standardInput);
    }

    /// <summary>
    /// Compiles all Java files in the directory and runs the main class.
    /// </summary>
    private async Task<(bool success, string output, string error)> CompileAndRunJavaAsync(
        string mainFile, string workingDirectory, string? standardInput = null)
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
                $"Compiled successfully:\n{compileResult.output}\n\n--- Program Output ---\n", standardInput);
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
        string fileName, string arguments, string workingDirectory, string outputPrefix, string? standardInput = null)
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
                RedirectStandardInput = !string.IsNullOrEmpty(standardInput),
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

            // Write standard input if provided
            if (!string.IsNullOrEmpty(standardInput))
            {
                try
                {
                    // Write the input and ensure it ends with a newline
                    await process.StandardInput.WriteAsync(standardInput);
                    if (!standardInput.EndsWith("\n") && !standardInput.EndsWith("\r\n"))
                    {
                        await process.StandardInput.WriteLineAsync();
                    }
                    await process.StandardInput.FlushAsync();
                }
                catch
                {
                    // Ignore errors if process terminates early
                }
                finally
                {
                    process.StandardInput.Close();
                }
            }

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
            "csharp" => GetMultiFileCSharpCommand(sourceFile, workingDirectory, outputName),
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

    /// <summary>
    /// Builds C# compilation command.
    /// For projects with .csproj or .sln files, uses dotnet build.
    /// For standalone .cs files, uses csc compiler to compile all files together.
    /// </summary>
    private (string fileName, string arguments) GetMultiFileCSharpCommand(string mainFile, string workingDirectory, string outputName)
    {
        // Check if there's a .csproj or .sln file (proper .NET project)
        var projectFiles = Directory.GetFiles(workingDirectory, "*.csproj", SearchOption.TopDirectoryOnly);
        var solutionFiles = Directory.GetFiles(workingDirectory, "*.sln", SearchOption.TopDirectoryOnly);

        if (projectFiles.Length > 0 || solutionFiles.Length > 0)
        {
            // Use dotnet build for proper projects
            return ("dotnet", "build");
        }

        // For standalone .cs files, compile all files with csc
        var csFiles = Directory.GetFiles(workingDirectory, "*.cs", SearchOption.TopDirectoryOnly).ToList();

        if (csFiles.Count == 0)
        {
            // Fallback to just the main file if no files found
            return ("csc", $"\"{mainFile}\" /out:\"{outputName}.exe\"");
        }

        // Quote each file path and join them
        var quotedFiles = string.Join(" ", csFiles.Select(f => $"\"{Path.GetFileName(f)}\""));
        return ("csc", $"{quotedFiles} /out:\"{outputName}.exe\"");
    }

    private string GetExecutablePath(string sourceFile, LanguageInfo language, string workingDirectory)
    {
        var baseName = Path.GetFileNameWithoutExtension(sourceFile);

        return language.Name switch
        {
            "cpp" or "c" or "rust" => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(workingDirectory, $"{baseName}.exe")
                : Path.Combine(workingDirectory, baseName),
            "csharp" => GetCSharpExecutablePath(baseName, workingDirectory),
            _ => throw new NotSupportedException($"Language {language.Name} does not produce executables")
        };
    }

    /// <summary>
    /// Gets the executable path for C# projects.
    /// For projects with .csproj or .sln, uses dotnet run.
    /// For standalone .cs files compiled with csc, returns the .exe path.
    /// </summary>
    private string GetCSharpExecutablePath(string baseName, string workingDirectory)
    {
        // Check if there's a .csproj or .sln file (proper .NET project)
        var projectFiles = Directory.GetFiles(workingDirectory, "*.csproj", SearchOption.TopDirectoryOnly);
        var solutionFiles = Directory.GetFiles(workingDirectory, "*.sln", SearchOption.TopDirectoryOnly);

        if (projectFiles.Length > 0 || solutionFiles.Length > 0)
        {
            // Use dotnet run for proper projects
            return "dotnet";
        }

        // For standalone .cs files compiled with csc, return the .exe path
        return Path.Combine(workingDirectory, $"{baseName}.exe");
    }

    /// <summary>
    /// Starts a program in interactive mode for real-time input/output.
    /// </summary>
    public async Task<InteractiveProcess?> StartInteractiveAsync(
        string sourceFile,
        LanguageInfo language,
        string workingDirectory,
        Action<string> onOutput,
        Action<string> onError,
        Action<int> onExit)
    {
        try
        {
            // Track if we've shown the program output header
            var programStarted = false;

            // For compiled languages, compile first
            if (NeedsCompilation(language.Name))
            {
                onOutput("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
                onOutput("  COMPILATION\n");
                onOutput("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n");

                var (compileSuccess, compileOutput, compileError) = await CompileAsync(sourceFile, language, workingDirectory);

                if (!compileSuccess)
                {
                    onError($"Compilation failed:\n{compileError}");
                    onExit(-1);
                    return null;
                }

                onOutput("✓ Compiled successfully\n\n");
            }

            // Determine executable path and arguments
            string fileName, arguments;

            if (language.Name == "java")
            {
                // Compile Java files first
                var javaFiles = Directory.GetFiles(workingDirectory, "*.java", SearchOption.TopDirectoryOnly).ToList();
                if (javaFiles.Count > 0)
                {
                    onOutput("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
                    onOutput("  COMPILATION\n");
                    onOutput("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n");

                    var quotedFiles = string.Join(" ", javaFiles.Select(f => $"\"{Path.GetFileName(f)}\""));
                    var compileResult = await ExecuteProgramAsync("javac", quotedFiles, workingDirectory, "");

                    if (!compileResult.success)
                    {
                        onError($"Java compilation failed:\n{compileResult.error}");
                        onExit(-1);
                        return null;
                    }

                    onOutput("✓ Compiled successfully\n\n");
                }

                var className = Path.GetFileNameWithoutExtension(sourceFile);
                fileName = "java";
                arguments = className;
            }
            else if (NeedsCompilation(language.Name))
            {
                var executablePath = GetExecutablePath(sourceFile, language, workingDirectory);
                fileName = executablePath;
                arguments = executablePath == "dotnet" ? "run" : "";
            }
            else
            {
                (fileName, arguments) = GetExecutionCommand(sourceFile, language);
            }

            // Start the process in interactive mode
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo };

            // Set up output handlers
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    // Show program output header on first actual output
                    if (!programStarted)
                    {
                        programStarted = true;
                        onOutput("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
                        onOutput("  PROGRAM OUTPUT\n");
                        onOutput("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n");
                    }
                    onOutput(e.Data + "\n");
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    // Show program output header on first actual output (including errors)
                    if (!programStarted)
                    {
                        programStarted = true;
                        onOutput("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
                        onOutput("  PROGRAM OUTPUT\n");
                        onOutput("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n");
                    }

                    // Filter out compilation warnings, only show actual errors
                    var line = e.Data;
                    if (!line.Contains("warning NETSDK") &&
                        !line.Contains("warning MSB") &&
                        !line.Contains("The target framework") &&
                        !line.Contains("will not receive security updates") &&
                        !line.Contains("Please refer to https://") &&
                        !line.Trim().StartsWith("All projects are up-to-date") &&
                        !string.IsNullOrWhiteSpace(line))
                    {
                        onError(e.Data + "\n");
                    }
                }
            };

            process.Exited += (sender, e) =>
            {
                onExit(process.ExitCode);
            };

            process.EnableRaisingEvents = true;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return new InteractiveProcess(process);
        }
        catch (Exception ex)
        {
            onError($"Failed to start program: {ex.Message}");
            onExit(-1);
            return null;
        }
    }
}