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

    /// <summary>
    /// Event raised when a course is successfully added.
    /// </summary>
    public event EventHandler? CourseAdded;

    /// <summary>
    /// Event raised when a course is successfully deleted.
    /// </summary>
    public event EventHandler? CourseDeleted;

    /// <summary>
    /// Initializes a new instance of the CourseManagementViewModel.
    /// </summary>
    /// <param name="rubricDirectory">The directory where course folders are stored.</param>
    public CourseManagementViewModel(string rubricDirectory)
    {
        _rubricDirectory = rubricDirectory;
        LoadCourses();
    }

    /// <summary>
    /// Loads all existing courses from the rubric directory.
    /// </summary>
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

    /// <summary>
    /// Validates that a course name is safe for file system use.
    /// Allows only letters, numbers, spaces, hyphens, and underscores.
    /// </summary>
    /// <param name="name">The course name to validate.</param>
    /// <returns>True if the name is valid; otherwise, false.</returns>
    private bool IsValidCourseName(string name)
    {
        // Allow letters, numbers, spaces, hyphens, underscores
        // Disallow: <>:"/\|?*
        var invalidChars = Path.GetInvalidFileNameChars();
        return !name.Any(c => invalidChars.Contains(c)) &&
               Regex.IsMatch(name.Trim(), @"^[a-zA-Z0-9\s\-_]+$");
    }

    /// <summary>
    /// Displays an error message to the user.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    /// <summary>
    /// Clears any displayed error messages.
    /// </summary>
    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    /// <summary>
    /// Handles changes to the NewCourseName property and clears errors.
    /// </summary>
    partial void OnNewCourseNameChanged(string value)
    {
        ClearError();
    }
}
