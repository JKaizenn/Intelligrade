using System;
using System.IO;
using FluentAssertions;
using IntelliGrade.App.Configuration;
using Xunit;

namespace IntelliGrade.Tests.Configuration;

/// <summary>
/// Unit tests for AppConfiguration.
/// Tests the main configuration class and validates proper initialization.
/// </summary>
public class AppConfigurationTests
{
    [Fact]
    public void Constructor_WithDefaultValues_CreatesAllSubConfigurations()
    {
        // Arrange & Act
        var config = new AppConfiguration();

        // Assert
        config.Grading.Should().NotBeNull("grading configuration should be initialized");
        config.Ollama.Should().NotBeNull("ollama configuration should be initialized");
        config.Storage.Should().NotBeNull("storage configuration should be initialized");
        config.Execution.Should().NotBeNull("execution configuration should be initialized");
    }

    [Fact]
    public void Constructor_WithCustomGradingConfig_UsesProvidedConfiguration()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), "IntelliGrade_Test_" + Guid.NewGuid().ToString());
        var gradingConfig = new GradingConfiguration(
            rubricDirectory: tempPath,
            defaultModel: "custom-model");

        // Act
        var config = new AppConfiguration(grading: gradingConfig);

        // Assert
        config.Grading.Should().BeSameAs(gradingConfig);

        // Cleanup
        if (Directory.Exists(tempPath))
            Directory.Delete(tempPath, true);
    }

    [Fact]
    public void Constructor_WithCustomOllamaConfig_UsesProvidedConfiguration()
    {
        // Arrange
        var ollamaConfig = new OllamaConfiguration(
            endpoint: "http://custom:1234",
            defaultModel: "custom-model",
            maxRetries: 5);

        // Act
        var config = new AppConfiguration(ollama: ollamaConfig);

        // Assert
        config.Ollama.Should().BeSameAs(ollamaConfig);
    }

    [Fact]
    public void Constructor_WithAllCustomConfigurations_UsesAllProvidedConfigurations()
    {
        // Arrange
        var gradingConfig = new GradingConfiguration();
        var ollamaConfig = new OllamaConfiguration();
        var storageConfig = new StorageConfiguration();
        var executionConfig = new ExecutionConfiguration();

        // Act
        var config = new AppConfiguration(
            grading: gradingConfig,
            ollama: ollamaConfig,
            storage: storageConfig,
            execution: executionConfig);

        // Assert
        config.Grading.Should().BeSameAs(gradingConfig);
        config.Ollama.Should().BeSameAs(ollamaConfig);
        config.Storage.Should().BeSameAs(storageConfig);
        config.Execution.Should().BeSameAs(executionConfig);
    }

    [Fact]
    public void Constructor_WithMixOfCustomAndDefaultConfigs_HandlesCorrectly()
    {
        // Arrange
        var customGrading = new GradingConfiguration(defaultModel: "custom-model");

        // Act
        var config = new AppConfiguration(grading: customGrading);

        // Assert
        config.Grading.Should().BeSameAs(customGrading);
        config.Ollama.Should().NotBeNull("should create default ollama config");
        config.Storage.Should().NotBeNull("should create default storage config");
        config.Execution.Should().NotBeNull("should create default execution config");
    }
}
