using System;
using IntelliGrade.App.Configuration;
using IntelliGrade.App.Interfaces;
using IntelliGrade.App.Services;
using IntelliGrade.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace IntelliGrade.App.DependencyInjection;

/// <summary>
/// Extension methods for configuring dependency injection.
/// Centralizes all service registration for clean architecture.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services with the dependency injection container.
    /// </summary>
    public static IServiceCollection AddIntelliGradeServices(this IServiceCollection services)
    {
        // Configuration (Singleton - one instance for entire application)
        services.AddSingleton<AppConfiguration>();
        services.AddSingleton(sp => sp.GetRequiredService<AppConfiguration>().Grading);
        services.AddSingleton(sp => sp.GetRequiredService<AppConfiguration>().Ollama);
        services.AddSingleton(sp => sp.GetRequiredService<AppConfiguration>().Storage);
        services.AddSingleton(sp => sp.GetRequiredService<AppConfiguration>().Execution);

        // Core Services (Scoped - new instance per request/window)
        services.AddScoped<ILanguageDetectorService, LanguageDetectorService>();
        services.AddScoped<IProgramRunnerService, ProgramRunnerService>();
        services.AddScoped<IFileManagerService, FileManagerService>();
        services.AddScoped<IRubricService, RubricService>();

        // Singleton services (shared across application)
        services.AddSingleton<ILocalStorageService, LocalStorageService>();

        // AI Service Factory (creates appropriate AI service based on configuration)
        services.AddScoped<IOllamaGradingService>(sp =>
        {
            var config = sp.GetRequiredService<OllamaConfiguration>();
            return new OllamaGradingService(config.DefaultModel, config.Endpoint);
        });

        // ViewModels (Transient - new instance every time requested)
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<CourseManagementViewModel>();
        services.AddTransient<RubricImportViewModel>();
        services.AddTransient<ApiSettingsViewModel>();

        return services;
    }
}
