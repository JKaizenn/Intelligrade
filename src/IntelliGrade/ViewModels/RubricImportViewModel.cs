using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliGrade.App.Services;

namespace IntelliGrade.App.ViewModels;

/// <summary>
/// ViewModel for importing and converting rubrics from text or file sources.
/// Provides functionality to import rubrics, automatically convert them to JSON format
/// using AI or pattern-based parsing, and save them to the appropriate course and language directory.
/// Includes comprehensive validation to prevent duplicates and invalid input.
/// </summary>
public partial class RubricImportViewModel : ViewModelBase
{
    private readonly string _rubricDirectory;
    private readonly RubricService _rubricService = new();
    private readonly RubricConverterService _converterService;

    [ObservableProperty] private ObservableCollection<string> _courses = new();
    [ObservableProperty] private ObservableCollection<string> _languages = new() { "cpp", "csharp", "python", "java", "rust", "c", "javascript" };

    [ObservableProperty] private string? _selectedCourse;
    [ObservableProperty] private string? _selectedLanguage;
    [ObservableProperty] private string _assignmentName = string.Empty;
    [ObservableProperty] private string _rubricText = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError = false;
    [ObservableProperty] private bool _isProcessing = false;
    [ObservableProperty] private bool _convertToJson = true;
    [ObservableProperty] private bool _useAiConversion = true;

    public IStorageProvider? StorageProvider { get; set; }
    public event EventHandler? RubricImported;

    public RubricImportViewModel(string rubricDirectory, string model = "llama3.2:1b")
    {
        _rubricDirectory = rubricDirectory;
        _converterService = new RubricConverterService(model);
        LoadCourses();
    }

    private void LoadCourses()
    {
        Courses.Clear();

        if (!Directory.Exists(_rubricDirectory))
            Directory.CreateDirectory(_rubricDirectory);

        var courses = Directory.GetDirectories(_rubricDirectory)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrEmpty(name))
            .Cast<string>()
            .OrderBy(name => name);

        foreach (var course in courses)
        {
            Courses.Add(course);
        }
    }

    [RelayCommand]
    private async Task ImportFromFileAsync()
    {
        if (StorageProvider == null)
        {
            ShowError("File picker not available");
            return;
        }

        try
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Rubric File",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Rubric Files") { Patterns = new[] { "*.json", "*.txt", "*.md" } },
                    new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } },
                    new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt", "*.md", "*.rtf" } },
                    new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
                }
            });

            if (files.Count > 0)
            {
                var file = files[0];
                using var stream = await file.OpenReadAsync();
                using var reader = new StreamReader(stream);
                RubricText = await reader.ReadToEndAsync();

                // Try to extract assignment name from filename if not set
                if (string.IsNullOrWhiteSpace(AssignmentName))
                {
                    AssignmentName = Path.GetFileNameWithoutExtension(file.Name);
                }

                StatusMessage = "File loaded successfully";
                ClearError();
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load file: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SaveRubricAsync()
    {
        ClearError();

        // Validation
        if (!ValidateInputs())
            return;

        IsProcessing = true;
        StatusMessage = "Saving rubric...";

        try
        {
            var coursePath = Path.Combine(_rubricDirectory, SelectedCourse!);
            var languagePath = Path.Combine(coursePath, SelectedLanguage!);

            Directory.CreateDirectory(languagePath);

            if (ConvertToJson)
            {
                await SaveAsJsonAsync(languagePath);
            }
            else
            {
                await SaveAsTextAsync(languagePath);
            }

            RubricImported?.Invoke(this, EventArgs.Empty);
            StatusMessage = "Rubric saved successfully!";

            // Clear form
            AssignmentName = string.Empty;
            RubricText = string.Empty;
        }
        catch (Exception ex)
        {
            ShowError($"Failed to save rubric: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task SaveAsJsonAsync(string languagePath)
    {
        StatusMessage = "Converting to JSON format...";

        string? jsonContent = null;

        if (UseAiConversion)
        {
            // Try AI conversion first
            StatusMessage = "Using AI to parse rubric structure...";
            jsonContent = await _converterService.ConvertToJsonAsync(
                RubricText,
                SelectedCourse!,
                AssignmentName);
        }

        // Fallback to simple conversion if AI fails
        if (string.IsNullOrEmpty(jsonContent))
        {
            StatusMessage = "Using pattern-based conversion...";
            jsonContent = _converterService.ConvertSimpleRubric(
                RubricText,
                SelectedCourse!,
                AssignmentName);
        }

        if (string.IsNullOrEmpty(jsonContent))
        {
            ShowError("Failed to convert rubric to JSON. Try saving as plain text instead.");
            return;
        }

        var jsonPath = Path.Combine(languagePath, $"{SanitizeFileName(AssignmentName)}.json");
        await File.WriteAllTextAsync(jsonPath, jsonContent);

        // Also save plain text version as backup
        var txtPath = Path.Combine(languagePath, $"{SanitizeFileName(AssignmentName)}.txt");
        await File.WriteAllTextAsync(txtPath, RubricText);
    }

    private async Task SaveAsTextAsync(string languagePath)
    {
        var txtPath = Path.Combine(languagePath, $"{SanitizeFileName(AssignmentName)}.txt");
        await File.WriteAllTextAsync(txtPath, RubricText);
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrEmpty(SelectedCourse))
        {
            ShowError("Please select a course");
            return false;
        }

        if (string.IsNullOrEmpty(SelectedLanguage))
        {
            ShowError("Please select a programming language");
            return false;
        }

        if (string.IsNullOrWhiteSpace(AssignmentName))
        {
            ShowError("Assignment name cannot be empty");
            return false;
        }

        if (!IsValidAssignmentName(AssignmentName))
        {
            ShowError("Assignment name contains invalid characters. Use only letters, numbers, spaces, and hyphens.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(RubricText))
        {
            ShowError("Rubric content cannot be empty");
            return false;
        }

        // Check for duplicate
        var languagePath = Path.Combine(_rubricDirectory, SelectedCourse, SelectedLanguage);
        var jsonPath = Path.Combine(languagePath, $"{SanitizeFileName(AssignmentName)}.json");
        var txtPath = Path.Combine(languagePath, $"{SanitizeFileName(AssignmentName)}.txt");

        if (File.Exists(jsonPath) || File.Exists(txtPath))
        {
            ShowError("A rubric with this name already exists for this course and language");
            return false;
        }

        return true;
    }

    private bool IsValidAssignmentName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return !name.Any(c => invalidChars.Contains(c)) &&
               Regex.IsMatch(name.Trim(), @"^[a-zA-Z0-9\s\-_]+$");
    }

    private string SanitizeFileName(string fileName)
    {
        return Regex.Replace(fileName.Trim(), @"[^\w\s\-]", "").Replace(" ", "_");
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    partial void OnSelectedCourseChanged(string? value)
    {
        ClearError();
    }

    partial void OnSelectedLanguageChanged(string? value)
    {
        ClearError();
    }

    partial void OnAssignmentNameChanged(string value)
    {
        ClearError();
    }
}
