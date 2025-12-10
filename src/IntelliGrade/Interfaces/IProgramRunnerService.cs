using System;
using System.Diagnostics;
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
    /// <param name="standardInput">Optional standard input to provide to the program</param>
    /// <returns>Tuple containing success status, output, and error messages</returns>
    Task<(bool success, string output, string error)> RunProgramAsync(
        string sourceFile,
        LanguageInfo language,
        string workingDirectory,
        string? standardInput = null);

    /// <summary>
    /// Starts a program in interactive mode where input can be sent in real-time.
    /// </summary>
    /// <param name="sourceFile">Path to source file</param>
    /// <param name="language">Programming language information</param>
    /// <param name="workingDirectory">Working directory for execution</param>
    /// <param name="onOutput">Callback invoked when program produces output</param>
    /// <param name="onError">Callback invoked when program produces error output</param>
    /// <param name="onExit">Callback invoked when program exits</param>
    /// <returns>InteractiveProcess wrapper for sending input and controlling the process</returns>
    Task<InteractiveProcess?> StartInteractiveAsync(
        string sourceFile,
        LanguageInfo language,
        string workingDirectory,
        Action<string> onOutput,
        Action<string> onError,
        Action<int> onExit);
}

/// <summary>
/// Wraps a running process for interactive execution.
/// </summary>
public class InteractiveProcess : IDisposable
{
    private readonly Process _process;
    private readonly object _lock = new();
    private bool _disposed;

    public InteractiveProcess(Process process)
    {
        _process = process;
    }

    ~InteractiveProcess()
    {
        Dispose();
    }

    public bool HasExited => _process.HasExited;

    /// <summary>
    /// Sends a line of input to the running program.
    /// Thread-safe with proper exception handling for race conditions.
    /// </summary>
    public async Task SendInputAsync(string input)
    {
        // Quick check before attempting write (optimization only)
        if (_disposed || _process.HasExited)
            return;

        try
        {
            await _process.StandardInput.WriteLineAsync(input);
            await _process.StandardInput.FlushAsync();
        }
        catch (ObjectDisposedException ex)
        {
            // Process or stream was disposed
            System.Diagnostics.Debug.WriteLine($"Failed to send input: {ex.Message}");
        }
        catch (System.IO.IOException ex)
        {
            // Pipe is broken (process terminated)
            System.Diagnostics.Debug.WriteLine($"Failed to send input: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            // Process has terminated or stdin is not redirected
            System.Diagnostics.Debug.WriteLine($"Failed to send input: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops the running program.
    /// </summary>
    public void Stop()
    {
        lock (_lock)
        {
            if (_disposed || _process.HasExited)
                return;

            try
            {
                _process.Kill(true);
            }
            catch (InvalidOperationException ex)
            {
                // Process already terminated
                System.Diagnostics.Debug.WriteLine($"Failed to stop process: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;
        }

        Stop();
        _process.Dispose();
        GC.SuppressFinalize(this);
    }
}
