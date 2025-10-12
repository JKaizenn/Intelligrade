using System;
using FluentAssertions;
using IntelliGrade.App.Configuration;
using Xunit;

namespace IntelliGrade.Tests.Configuration;

/// <summary>
/// Unit tests for ExecutionConfiguration.
/// Demonstrates Test-Driven Development (TDD) principles:
/// 1. Test behavior, not implementation
/// 2. One assertion per test (when possible)
/// 3. Descriptive test names
/// 4. Arrange-Act-Assert pattern
/// </summary>
public class ExecutionConfigurationTests
{
    [Fact]
    public void Constructor_WithDefaultValues_SetsExpectedDefaults()
    {
        // Arrange & Act
        var config = new ExecutionConfiguration();

        // Assert
        config.TimeoutSeconds.Should().Be(30, "default timeout should be 30 seconds");
        config.MaxOutputLines.Should().Be(10000, "default max output should be 10000 lines");
        config.EnableSandbox.Should().BeFalse("sandbox should be disabled by default");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(300)]
    public void Constructor_WithValidTimeout_AcceptsValue(int timeoutSeconds)
    {
        // Arrange & Act
        var config = new ExecutionConfiguration(timeoutSeconds: timeoutSeconds);

        // Assert
        config.TimeoutSeconds.Should().Be(timeoutSeconds);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(301)]
    [InlineData(1000)]
    public void Constructor_WithInvalidTimeout_ThrowsArgumentException(int timeoutSeconds)
    {
        // Arrange & Act
        Action act = () => new ExecutionConfiguration(timeoutSeconds: timeoutSeconds);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Timeout must be between 1 and 300 seconds*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void Constructor_WithValidMaxOutputLines_AcceptsValue(int maxLines)
    {
        // Arrange & Act
        var config = new ExecutionConfiguration(maxOutputLines: maxLines);

        // Assert
        config.MaxOutputLines.Should().Be(maxLines);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Constructor_WithInvalidMaxOutputLines_ThrowsArgumentException(int maxLines)
    {
        // Arrange & Act
        Action act = () => new ExecutionConfiguration(maxOutputLines: maxLines);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*MaxOutputLines must be between 1 and 100000*");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_WithEnableSandbox_SetsValue(bool enableSandbox)
    {
        // Arrange & Act
        var config = new ExecutionConfiguration(enableSandbox: enableSandbox);

        // Assert
        config.EnableSandbox.Should().Be(enableSandbox);
    }

    [Fact]
    public void Constructor_WithAllCustomValues_SetsAllProperties()
    {
        // Arrange
        const int timeout = 120;
        const int maxLines = 5000;
        const bool sandbox = true;

        // Act
        var config = new ExecutionConfiguration(
            timeoutSeconds: timeout,
            maxOutputLines: maxLines,
            enableSandbox: sandbox);

        // Assert
        config.TimeoutSeconds.Should().Be(timeout);
        config.MaxOutputLines.Should().Be(maxLines);
        config.EnableSandbox.Should().Be(sandbox);
    }
}
