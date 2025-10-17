using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using IntelliGrade.App.Services;
using Xunit;

namespace IntelliGrade.Tests.Services;

/// <summary>
/// Unit tests for LanguageDetectorService.
/// Tests language detection logic without external dependencies.
/// </summary>
public class LanguageDetectorServiceTests : IDisposable
{
    private readonly LanguageDetectorService _service;
    private readonly string _testDirectory;

    public LanguageDetectorServiceTests()
    {
        _service = new LanguageDetectorService();
        _testDirectory = Path.Combine(Path.GetTempPath(), "IntelliGrade_Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public void DetectLanguages_WithPythonFiles_ReturnsPython()
    {
        // Arrange
        var pyFile = Path.Combine(_testDirectory, "test.py");
        File.WriteAllText(pyFile, "print('hello')");

        // Act
        var detected = _service.DetectLanguages(_testDirectory);

        // Assert
        detected.Should().ContainSingle(lang => lang.Name == "python");

        // Cleanup
        File.Delete(pyFile);
    }

    [Fact]
    public void DetectLanguages_WithCSharpFiles_ReturnsCSharp()
    {
        // Arrange
        var csFile = Path.Combine(_testDirectory, "test.cs");
        File.WriteAllText(csFile, "Console.WriteLine(\"hello\");");

        // Act
        var detected = _service.DetectLanguages(_testDirectory);

        // Assert
        detected.Should().ContainSingle(lang => lang.Name == "csharp");

        // Cleanup
        File.Delete(csFile);
    }

    [Fact]
    public void DetectLanguages_WithMultipleLanguages_ReturnsAll()
    {
        // Arrange
        var pyFile = Path.Combine(_testDirectory, "test.py");
        var jsFile = Path.Combine(_testDirectory, "test.js");
        var cppFile = Path.Combine(_testDirectory, "test.cpp");

        File.WriteAllText(pyFile, "print('hello')");
        File.WriteAllText(jsFile, "console.log('hello');");
        File.WriteAllText(cppFile, "#include <iostream>");

        // Act
        var detected = _service.DetectLanguages(_testDirectory);

        // Assert
        detected.Should().HaveCount(3);
        detected.Should().Contain(lang => lang.Name == "python");
        detected.Should().Contain(lang => lang.Name == "javascript");
        detected.Should().Contain(lang => lang.Name == "cpp");

        // Cleanup
        File.Delete(pyFile);
        File.Delete(jsFile);
        File.Delete(cppFile);
    }

    [Fact]
    public void DetectLanguages_WithNoSourceFiles_ReturnsEmpty()
    {
        // Arrange
        var txtFile = Path.Combine(_testDirectory, "readme.txt");
        File.WriteAllText(txtFile, "Not a source file");

        // Act
        var detected = _service.DetectLanguages(_testDirectory);

        // Assert
        detected.Should().BeEmpty();

        // Cleanup
        File.Delete(txtFile);
    }

    [Fact]
    public void GetSourceFiles_WithPythonFiles_ReturnsAllPyFiles()
    {
        // Arrange
        var file1 = Path.Combine(_testDirectory, "test1.py");
        var file2 = Path.Combine(_testDirectory, "test2.py");
        File.WriteAllText(file1, "print('1')");
        File.WriteAllText(file2, "print('2')");

        var detected = _service.DetectLanguages(_testDirectory);
        var pythonLang = detected.First(lang => lang.Name == "python");

        // Act
        var sourceFiles = _service.GetSourceFiles(_testDirectory, pythonLang);

        // Assert
        sourceFiles.Should().HaveCount(2);
        sourceFiles.Should().Contain(f => f.EndsWith("test1.py"));
        sourceFiles.Should().Contain(f => f.EndsWith("test2.py"));

        // Cleanup
        File.Delete(file1);
        File.Delete(file2);
    }

    [Fact]
    public void GetSourceFiles_WithSubdirectories_FindsFilesRecursively()
    {
        // Arrange
        var subDir = Path.Combine(_testDirectory, "subdir");
        Directory.CreateDirectory(subDir);

        var file1 = Path.Combine(_testDirectory, "test1.py");
        var file2 = Path.Combine(subDir, "test2.py");
        File.WriteAllText(file1, "print('1')");
        File.WriteAllText(file2, "print('2')");

        var detected = _service.DetectLanguages(_testDirectory);
        var pythonLang = detected.First(lang => lang.Name == "python");

        // Act
        var sourceFiles = _service.GetSourceFiles(_testDirectory, pythonLang);

        // Assert
        sourceFiles.Should().HaveCount(2);
        sourceFiles.Should().Contain(f => f.Contains("test1.py"));
        sourceFiles.Should().Contain(f => f.Contains("test2.py"));

        // Cleanup
        File.Delete(file1);
        File.Delete(file2);
        Directory.Delete(subDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
        GC.SuppressFinalize(this);
    }
}
