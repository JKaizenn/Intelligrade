using Avalonia;
using System;

namespace IntelliGrade.App;

/// <summary>
/// Application entry point for IntelliGrade.
/// Configures and launches the Avalonia desktop application.
/// </summary>
sealed class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// Initializes Avalonia framework and starts the classic desktop lifetime.
    /// </summary>
    /// <param name="args">Command-line arguments</param>
    /// <remarks>
    /// Do not use Avalonia, third-party APIs, or SynchronizationContext-reliant code
    /// before this method is called - framework initialization is not yet complete.
    /// </remarks>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Configures the Avalonia application builder with platform detection and logging.
    /// Used by both the application and the Avalonia visual designer.
    /// </summary>
    /// <returns>Configured AppBuilder instance</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
