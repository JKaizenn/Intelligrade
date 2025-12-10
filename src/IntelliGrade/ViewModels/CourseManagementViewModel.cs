using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace IntelliGrade.App.ViewModels;

/// <summary>
/// ViewModel for managing courses in the IntelliGrade application.
/// Provides functionality to add, delete, and view courses with validation
/// to ensure course names are file-system safe and unique.
/// </summary>
public partial class CourseManagementViewModel : ViewModelBase
{
    private readonly string _rubricDirectory;

    [ObservableProperty] private ObservableCollection<string> _courses = new();
    [ObservableProperty] private string _newCourseName = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string? _selectedCourse;
    [ObservableProperty] private bool _hasError = false;
    [ObservableProperty] private ObservableCollection<RubricInfo> _rubrics = new();
    [ObservableProperty] private RubricInfo? _selectedRubric;

    public event EventHandler? CourseAdded;
    public event EventHandler? CourseDeleted;
    public event EventHandler? RubricDeleted;

    public CourseManagementViewModel(string rubricDirectory)
    {
        _rubricDirectory = rubricDirectory;
        LoadCourses();
    }

    private void LoadCourses()
    {
        Courses.Clear();

        if (!Directory.Exists(_rubricDirectory))
            return;

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
    private void AddCourse()
    {
        ClearError();

        // Validate course name
        if (string.IsNullOrWhiteSpace(NewCourseName))
        {
            ShowError("Course name cannot be empty");
            return;
        }

        // Validate against invalid file system characters
        if (!IsValidCourseName(NewCourseName))
        {
            ShowError("Course name contains invalid characters. Use only letters, numbers, spaces, and hyphens.");
            return;
        }

        // Check for duplicate
        if (Courses.Any(c => c.Equals(NewCourseName.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ShowError("A course with this name already exists");
            return;
        }

        try
        {
            var coursePath = Path.Combine(_rubricDirectory, NewCourseName.Trim());
            Directory.CreateDirectory(coursePath);

            Courses.Add(NewCourseName.Trim());
            CourseAdded?.Invoke(this, EventArgs.Empty);

            NewCourseName = string.Empty;
            ClearError();
        }
        catch (Exception ex)
        {
            ShowError($"Failed to create course: {ex.Message}");
        }
    }

    [RelayCommand]
    private void DeleteCourse()
    {
        if (string.IsNullOrEmpty(SelectedCourse))
        {
            ShowError("Please select a course to delete");
            return;
        }

        ClearError();

        try
        {
            var coursePath = Path.Combine(_rubricDirectory, SelectedCourse);

            if (Directory.Exists(coursePath))
            {
                // Check if course has content
                var hasFiles = Directory.GetFiles(coursePath, "*", SearchOption.AllDirectories).Any();
                if (hasFiles)
                {
                    ShowError("Cannot delete course with existing rubrics. Delete rubrics first.");
                    return;
                }

                Directory.Delete(coursePath, true);
                Courses.Remove(SelectedCourse);
                SelectedCourse = null;
                CourseDeleted?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to delete course: {ex.Message}");
        }
    }

    private bool IsValidCourseName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return !name.Any(c => invalidChars.Contains(c)) &&
               Regex.IsMatch(name.Trim(), @"^[a-zA-Z0-9\s\-_]+$");
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

    partial void OnNewCourseNameChanged(string value)
    {
        ClearError();
    }

    partial void OnSelectedCourseChanged(string? value)
    {
        LoadRubrics();
    }

    /// <summary>
    /// Loads all rubrics for the currently selected course.
    /// </summary>
    private void LoadRubrics()
    {
        Rubrics.Clear();
        SelectedRubric = null;

        if (string.IsNullOrEmpty(SelectedCourse))
            return;

        try
        {
            var coursePath = Path.Combine(_rubricDirectory, SelectedCourse);
            if (!Directory.Exists(coursePath))
                return;

            // Get all language directories
            var languageDirs = Directory.GetDirectories(coursePath);

            foreach (var langDir in languageDirs)
            {
                var language = Path.GetFileName(langDir);

                // Get all rubric files (JSON and TXT)
                var rubricFiles = Directory.GetFiles(langDir, "*.*")
                    .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                               f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var file in rubricFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var extension = Path.GetExtension(file).ToLowerInvariant();

                    // Only add JSON files or TXT files that don't have a corresponding JSON
                    if (extension == ".json" ||
                        (extension == ".txt" && !rubricFiles.Any(f =>
                            Path.GetFileNameWithoutExtension(f) == fileName &&
                            f.EndsWith(".json", StringComparison.OrdinalIgnoreCase))))
                    {
                        Rubrics.Add(new RubricInfo
                        {
                            Name = fileName,
                            Language = language,
                            FilePath = file,
                            IsJson = extension == ".json"
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Silently fail - rubrics list will just be empty
            System.Diagnostics.Debug.WriteLine($"Error loading rubrics: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes the selected rubric file(s).
    /// </summary>
    [RelayCommand]
    private void DeleteRubric()
    {
        if (SelectedRubric == null)
        {
            ShowError("Please select a rubric to delete");
            return;
        }

        ClearError();

        try
        {
            // Delete the main file
            if (File.Exists(SelectedRubric.FilePath))
            {
                File.Delete(SelectedRubric.FilePath);
            }

            // If it's a JSON file, also delete the corresponding TXT file if it exists
            if (SelectedRubric.IsJson)
            {
                var txtPath = Path.ChangeExtension(SelectedRubric.FilePath, ".txt");
                if (File.Exists(txtPath))
                {
                    File.Delete(txtPath);
                }
            }

            Rubrics.Remove(SelectedRubric);
            SelectedRubric = null;
            RubricDeleted?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to delete rubric: {ex.Message}");
        }
    }
}

/// <summary>
/// Holds information about a rubric file.
/// </summary>
public class RubricInfo
{
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public bool IsJson { get; set; }

    public string DisplayText => $"{Name} ({Language}) - {(IsJson ? "JSON" : "Text")}";
}
