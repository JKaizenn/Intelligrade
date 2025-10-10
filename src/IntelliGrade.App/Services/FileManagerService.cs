using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntelliGrade.App.Interfaces;

namespace IntelliGrade.App.Services;

/// <summary>
/// Manages file operations for assignments.
/// </summary>
public class FileManagerService : IFileManagerService
{
    public List<string> GetCourses(string rubricDir)
    {
        if (!Directory.Exists(rubricDir))
        {
            Directory.CreateDirectory(rubricDir);
            return new List<string>();
        }

        return Directory.GetDirectories(rubricDir)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrEmpty(name))
            .Cast<string>()
            .OrderBy(name => name)
            .ToList();
    }

    public List<string> GetAssignments(string rubricDir, string course, string language)
    {
        var path = Path.Combine(rubricDir, course, language);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return new List<string>();
        }

        // Support both .txt and .json rubric files
        var txtFiles = Directory.GetFiles(path, "*.txt");
        var jsonFiles = Directory.GetFiles(path, "*.json");

        return txtFiles.Concat(jsonFiles)
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Where(name => !string.IsNullOrEmpty(name))
            .Cast<string>()
            .Distinct()
            .OrderBy(name => name)
            .ToList();
    }

    public string? ReadRubric(string rubricPath)
    {
        return File.Exists(rubricPath) ? File.ReadAllText(rubricPath) : null;
    }

    public List<string> FindOutputFiles(string directory, List<string> copiedFiles)
    {
        var outputFiles = new List<string>();
        var excludeExtensions = new[] { ".cs", ".py", ".java", ".cpp", ".c", ".rs", ".go", ".rb", ".php", ".js" };
        
        var allFiles = Directory.GetFiles(directory)
            .Where(f => !excludeExtensions.Contains(Path.GetExtension(f).ToLower()))
            .Where(f => File.GetLastWriteTime(f) > DateTime.Now.AddMinutes(-5))
            .Where(f => !copiedFiles.Contains(Path.GetFileName(f)));

        return allFiles.Select(Path.GetFileName)
            .Where(name => !string.IsNullOrEmpty(name))
            .Cast<string>()
            .ToList();
    }

    public void CleanupFiles(string directory, List<string> filesToRemove)
    {
        foreach (var file in filesToRemove)
        {
            var fullPath = Path.Combine(directory, file);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }

    public async System.Threading.Tasks.Task<string?> CloneGitRepository(string repoUrl, string targetDirectory)
    {
        try
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone {repoUrl} \"{targetDirectory}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(processInfo);
            if (process == null)
                return "Failed to start git process";

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
                return null;

            var error = await process.StandardError.ReadToEndAsync();
            return string.IsNullOrWhiteSpace(error) ? "Git clone failed" : error;
        }
        catch (Exception ex)
        {
            return $"Error cloning repository: {ex.Message}";
        }
    }
}