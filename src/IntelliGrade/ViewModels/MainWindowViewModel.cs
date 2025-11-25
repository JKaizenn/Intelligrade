using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliGrade.App.Models;
using IntelliGrade.App.Services;

namespace IntelliGrade.App.ViewModels;

/// <summary>
/// Main application view model coordinating the grading workflow.
/// Manages course selection, code execution, AI analysis, and grade recording.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly LanguageDetectorService _languageDetector = new();
    private readonly ProgramRunnerService _programRunner = new();
    private readonly FileManagerService _fileManager = new();
    private readonly LocalStorageService _localStorage = new();
    private readonly RubricService _rubricService = new();
    private readonly GradingConfig _config = new();
    private OllamaGradingService? _ollamaService;

    public IStorageProvider? StorageProvider { get; set; }
    public Func<string, string, Task<bool>>? ShowConfirmationDialog { get; set; }

    [ObservableProperty] private string _currentDirectory = Directory.GetCurrentDirectory();
    [ObservableProperty] private ObservableCollection<string> _courses = new();
    [ObservableProperty] private ObservableCollection<LanguageInfo> _detectedLanguages = new();
    [ObservableProperty] private ObservableCollection<string> _sourceFiles = new();
    [ObservableProperty] private ObservableCollection<string> _assignments = new();

    [ObservableProperty] private string? _selectedCourse;
    [ObservableProperty] private LanguageInfo? _selectedLanguage;
    [ObservableProperty] private string? _selectedSourceFile;
    [ObservableProperty] private string? _selectedAssignment;

    [ObservableProperty] private string _sourceCode = string.Empty;
    [ObservableProperty] private string _programOutput = string.Empty;
    [ObservableProperty] private string _aiAnalysis = string.Empty;
    [ObservableProperty] private string _rubricContent = string.Empty;
    [ObservableProperty] private string _statusMessage = "Ready";

    [ObservableProperty] private string _studentName = string.Empty;
    [ObservableProperty] private string _studentId = string.Empty;
    [ObservableProperty] private decimal? _grade = 0;
    [ObservableProperty] private string _instructorFeedback = string.Empty;

    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private bool _ollamaAvailable;
    [ObservableProperty] private bool _isDarkMode;
    [ObservableProperty] private bool _showWelcomeScreen = true;

    public string LetterGrade => CalculateLetterGrade(Grade);

    public MainWindowViewModel()
    {
        LoadCourses();
        InitializeOllama();
        LoadSettings();
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        ApplyTheme();
        SaveThemePreference();
    }

    [RelayCommand]
    private void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
    }

    /// <summary>
    /// Applies the current theme to the application.
    /// </summary>
    private void ApplyTheme()
    {
        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeVariant = IsDarkMode
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        }
    }

    private async void LoadSettings()
    {
        try
        {
            // Check if user has a saved theme preference
            var savedTheme = await _localStorage.GetAsync<bool?>("IsDarkMode", null);

            if (savedTheme.HasValue)
            {
                // Use saved preference
                IsDarkMode = savedTheme.Value;
            }
            else
            {
                // Detect system theme
                IsDarkMode = DetectSystemTheme();
            }

            // Apply theme on startup
            ApplyTheme();

            var dir = await _localStorage.GetAsync("LastDirectory", Directory.GetCurrentDirectory());
            if (dir != null) CurrentDirectory = dir;
        }
        catch { }
    }

    private bool DetectSystemTheme()
    {
        try
        {
            var app = Application.Current;
            if (app?.PlatformSettings != null)
            {
                var colorValues = app.PlatformSettings.GetColorValues();
                return colorValues.ThemeVariant == Avalonia.Platform.PlatformThemeVariant.Dark;
            }
        }
        catch { }

        // Default to light mode if detection fails
        return false;
    }

    private async void SaveThemePreference()
    {
        try
        {
            await _localStorage.SetAsync("IsDarkMode", IsDarkMode);
        }
        catch
        {
            // Ignore errors saving theme
        }
    }

    [RelayCommand]
    private void StartGrading()
    {
        ShowWelcomeScreen = false;
    }

    [RelayCommand]
    private void ReturnHome()
    {
        ShowWelcomeScreen = true;
    }

    [RelayCommand]
    private async Task ManageCoursesAsync()
    {
        var viewModel = new CourseManagementViewModel(_config.RubricDirectory);
        var window = new Views.CourseManagementView
        {
            DataContext = viewModel
        };

        viewModel.CourseAdded += (_, _) => LoadCourses();
        viewModel.CourseDeleted += (_, _) => LoadCourses();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow!);
        }
    }

    [RelayCommand]
    private async Task ImportRubricAsync()
    {
        var viewModel = new RubricImportViewModel(_config.RubricDirectory, "llama3.2:1b");
        var window = new Views.RubricImportView
        {
            DataContext = viewModel
        };

        viewModel.RubricImported += (_, _) =>
        {
            LoadCourses();
            if (SelectedCourse != null && SelectedLanguage != null)
            {
                LoadAssignments();
            }
        };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow!);
        }
    }

    [RelayCommand]
    private async Task OpenApiSettingsAsync()
    {
        var viewModel = new ApiSettingsViewModel(_localStorage);
        var window = new Views.ApiSettingsView
        {
            DataContext = viewModel
        };

        viewModel.SettingsSaved += async (_, _) =>
        {
            await InitializeOllamaAsync();
        };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow!);
        }
    }

    [RelayCommand]
    private async Task SelectDirectoryAsync()
    {
        if (StorageProvider == null)
        {
            StatusMessage = "Folder picker not available";
            return;
        }

        try
        {
            var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Student Code Directory",
                AllowMultiple = false
            });

            if (folders.Count > 0)
            {
                CurrentDirectory = folders[0].Path.LocalPath;
                StatusMessage = $"Selected: {CurrentDirectory}";

                // Save to local storage
                await _localStorage.SetAsync("LastDirectory", CurrentDirectory);

                DetectLanguages();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error selecting folder: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CloneGitRepositoryAsync()
    {
        if (StorageProvider == null)
        {
            StatusMessage = "Folder picker not available";
            return;
        }

        try
        {
            var result = await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var dialog = new Views.GitCloneDialog();
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    return await dialog.ShowDialog<string?>(desktop.MainWindow!);
                }
                return null;
            });

            if (string.IsNullOrWhiteSpace(result))
                return;

            var repoUrl = result;
            var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Directory to Clone Repository Into",
                AllowMultiple = false
            });

            if (folders.Count == 0)
                return;

            var parentDir = folders[0].Path.LocalPath;
            var repoName = Path.GetFileNameWithoutExtension(repoUrl.TrimEnd('/').Split('/').Last());
            var targetDir = Path.Combine(parentDir, repoName);

            IsProcessing = true;
            StatusMessage = $"Cloning repository from {repoUrl}...";

            var error = await _fileManager.CloneGitRepository(repoUrl, targetDir);

            IsProcessing = false;

            if (error == null)
            {
                CurrentDirectory = targetDir;
                StatusMessage = $"Successfully cloned repository to {targetDir}";
                await _localStorage.SetAsync("LastDirectory", CurrentDirectory);
                DetectLanguages();
            }
            else
            {
                StatusMessage = $"Clone failed: {error}";
            }
        }
        catch (Exception ex)
        {
            IsProcessing = false;
            StatusMessage = $"Error cloning repository: {ex.Message}";
        }
    }

    /// <summary>
    /// Scans current directory for programming languages and source files.
    /// Auto-selects if only one language is found.
    /// </summary>
    [RelayCommand]
    private void DetectLanguages()
    {
        try
        {
            var languages = _languageDetector.DetectLanguages(CurrentDirectory);
            DetectedLanguages = new ObservableCollection<LanguageInfo>(languages);
            
            if (languages.Count == 1)
            {
                SelectedLanguage = languages[0];
            }
            
            StatusMessage = $"Detected {languages.Count} language(s)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error detecting languages: {ex.Message}";
        }
    }

    partial void OnSelectedLanguageChanged(LanguageInfo? value)
    {
        if (value != null)
        {
            var files = _languageDetector.GetSourceFiles(CurrentDirectory, value);
            var fileNames = files.Select(f => Path.GetFileName(f) ?? f).ToList();
            SourceFiles = new ObservableCollection<string>(fileNames);
            LoadAssignments();
        }
    }

    partial void OnSelectedCourseChanged(string? value)
    {
        if (value != null && SelectedLanguage != null)
        {
            LoadAssignments();
        }
    }

    partial void OnSelectedSourceFileChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var fullPath = Path.Combine(CurrentDirectory, value);
            SourceCode = File.Exists(fullPath) ? File.ReadAllText(fullPath) : string.Empty;
        }
    }

    partial void OnSelectedAssignmentChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value) && SelectedCourse != null && SelectedLanguage != null)
        {
            LoadRubric();
        }
    }

    partial void OnGradeChanged(decimal? value)
    {
        OnPropertyChanged(nameof(LetterGrade));
    }

    /// <summary>
    /// Executes the selected source file with timeout protection.
    /// Automatically compiles if needed, captures output for grading analysis.
    /// </summary>
    [RelayCommand]
    private async Task RunProgramAsync()
    {
        if (SelectedLanguage == null || string.IsNullOrEmpty(SelectedSourceFile))
        {
            StatusMessage = "Please select language and source file";
            return;
        }

        IsProcessing = true;
        StatusMessage = "Running program...";
        ProgramOutput = string.Empty;

        try
        {
            var result = await _programRunner.RunProgramAsync(
                SelectedSourceFile, SelectedLanguage, CurrentDirectory);

            var output = new System.Text.StringBuilder();

            if (result.success)
            {
                output.AppendLine("✓ Success");
                output.AppendLine();
                output.AppendLine("Output:");
                output.AppendLine(result.output);
            }
            else
            {
                output.AppendLine("✗ Failed");
                output.AppendLine();
                output.AppendLine("Error:");
                output.AppendLine(result.error);
            }

            // Check for .txt output files created by the program
            var outputFiles = _fileManager.FindOutputFiles(CurrentDirectory, new());
            var txtFiles = outputFiles.Where(f => f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).ToList();

            if (txtFiles.Count > 0)
            {
                output.AppendLine();
                output.AppendLine("=".PadRight(60, '='));
                output.AppendLine("Program Output Files:");
                output.AppendLine("=".PadRight(60, '='));

                foreach (var txtFile in txtFiles)
                {
                    try
                    {
                        var filePath = Path.Combine(CurrentDirectory, txtFile);
                        var fileContent = await File.ReadAllTextAsync(filePath);

                        output.AppendLine();
                        output.AppendLine($"File: {txtFile}");
                        output.AppendLine("=".PadRight(60, '='));
                        output.AppendLine(fileContent);
                        output.AppendLine("=".PadRight(60, '='));
                    }
                    catch (Exception fileEx)
                    {
                        output.AppendLine($"Error reading {txtFile}: {fileEx.Message}");
                    }
                }
            }

            ProgramOutput = output.ToString();
            StatusMessage = result.success ? "Program executed successfully" : "Program failed";
        }
        catch (Exception ex)
        {
            ProgramOutput = $"Error: {ex.Message}";
            StatusMessage = "Execution error";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>
    /// Analyzes source code using local Ollama LLM against rubric criteria.
    /// Generates detailed feedback with evidence-based scoring suggestions.
    /// </summary>
    [RelayCommand]
    private async Task AnalyzeCodeAsync()
    {
        if (_ollamaService == null || !OllamaAvailable)
        {
            AiAnalysis = "Ollama not available. Please install Ollama and run: ollama pull llama3.2:1b";
            return;
        }

        if (string.IsNullOrEmpty(SourceCode))
        {
            StatusMessage = "No source code to analyze";
            return;
        }

        IsProcessing = true;
        StatusMessage = "Analyzing with AI...";
        AiAnalysis = "Analyzing...";

        try
        {
            var rubricDir = Path.Combine(_config.RubricDirectory,
                SelectedCourse ?? "default",
                SelectedLanguage?.Name ?? "default");

            // Try to load rubric
            var jsonPath = Path.Combine(rubricDir, $"{SelectedAssignment ?? "default"}.json");
            var rubric = await _rubricService.LoadRubricAsync(jsonPath);

            if (rubric == null)
            {
                // Create a default rubric if none found
                rubric = new Models.Rubric
                {
                    Name = SelectedAssignment ?? "Unknown Assignment",
                    Course = SelectedCourse ?? "Unknown Course",
                    Language = SelectedLanguage?.Name ?? "Unknown",
                    TotalPoints = 100,
                    Criteria = new List<Models.Criterion>
                    {
                        new Models.Criterion
                        {
                            Name = "Overall Quality",
                            MaxPoints = 100,
                            Description = "General code quality assessment",
                            Levels = new List<Models.CriterionLevel>
                            {
                                new Models.CriterionLevel { Label = "Excellent", Points = 100, Description = "Excellent work" },
                                new Models.CriterionLevel { Label = "Good", Points = 80, Description = "Good work with minor issues" },
                                new Models.CriterionLevel { Label = "Fair", Points = 60, Description = "Fair work with several issues" },
                                new Models.CriterionLevel { Label = "Poor", Points = 0, Description = "Does not meet requirements" }
                            }
                        }
                    }
                };
            }

            var response = await _ollamaService.AnalyzeCodeAsync(
                SourceCode,
                rubric,
                SelectedCourse ?? "Unknown",
                SelectedAssignment ?? "Unknown",
                new List<string>());

            // Format response for display
            if (response.Success)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"AI GRADING ANALYSIS");
                sb.AppendLine($"Overall Confidence: {response.OverallConfidence}");
                sb.AppendLine($"Recommended Total: {response.RecommendedTotal}/{response.MaxPossible}");
                sb.AppendLine();
                sb.AppendLine("CRITERION BREAKDOWN:");
                sb.AppendLine("=".PadRight(80, '='));

                foreach (var suggestion in response.Suggestions)
                {
                    sb.AppendLine();
                    sb.AppendLine($"• {suggestion.CriterionName}");
                    sb.AppendLine($"  Score: {suggestion.SuggestedScore}/{suggestion.MaxPoints} ({suggestion.RatingLevel})");
                    sb.AppendLine($"  Confidence: {suggestion.Confidence}");
                    sb.AppendLine($"  Reasoning: {suggestion.Reasoning}");

                    if (suggestion.Evidence.Count > 0)
                    {
                        sb.AppendLine("  Evidence:");
                        foreach (var evidence in suggestion.Evidence)
                        {
                            sb.AppendLine($"    - {evidence}");
                        }
                    }
                }

                sb.AppendLine();
                sb.AppendLine("SUMMARY:");
                sb.AppendLine(response.Summary);

                AiAnalysis = sb.ToString();
            }
            else
            {
                AiAnalysis = $"Analysis failed: {response.ErrorMessage}";
            }

            StatusMessage = "Analysis complete";
        }
        catch (Exception ex)
        {
            AiAnalysis = $"Analysis error: {ex.Message}";
            StatusMessage = "Analysis failed";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>
    /// Removes selected source file and associated output files after confirmation.
    /// Useful for moving to the next student submission.
    /// </summary>
    [RelayCommand]
    private async Task CleanupFilesAsync()
    {
        if (string.IsNullOrEmpty(SelectedSourceFile))
        {
            StatusMessage = "No files to cleanup";
            return;
        }

        // Show confirmation dialog
        if (ShowConfirmationDialog != null)
        {
            var confirmed = await ShowConfirmationDialog(
                "Confirm Delete",
                $"Are you sure you want to delete {SelectedSourceFile} and its output files? This action cannot be undone."
            );

            if (!confirmed)
            {
                StatusMessage = "Cleanup cancelled";
                return;
            }
        }

        try
        {
            var filesToRemove = new List<string> { SelectedSourceFile };
            var outputFiles = _fileManager.FindOutputFiles(CurrentDirectory, new());
            filesToRemove.AddRange(outputFiles);

            _fileManager.CleanupFiles(CurrentDirectory, filesToRemove);

            StatusMessage = $"Cleaned up {filesToRemove.Count} file(s)";
            DetectLanguages();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Cleanup error: {ex.Message}";
        }
    }

    private void LoadCourses()
    {
        try
        {
            var courses = _fileManager.GetCourses(_config.RubricDirectory);
            Courses = new ObservableCollection<string>(courses);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading courses: {ex.Message}";
        }
    }

    private void LoadAssignments()
    {
        if (SelectedCourse == null || SelectedLanguage == null) return;

        try
        {
            var assignments = _fileManager.GetAssignments(
                _config.RubricDirectory, SelectedCourse, SelectedLanguage.Name);
            Assignments = new ObservableCollection<string>(assignments);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading assignments: {ex.Message}";
        }
    }

    private async void InitializeOllama()
    {
        await InitializeOllamaAsync();
    }

    private async Task InitializeOllamaAsync()
    {
        try
        {
            var settings = await _localStorage.GetAsync<ApiSettings>("ApiSettings");
            if (settings != null)
            {
                _ollamaService = new OllamaGradingService(settings.OllamaModel, settings.OllamaEndpoint);
            }
            else
            {
                _ollamaService = new OllamaGradingService(_config.OllamaModel);
            }

            OllamaAvailable = await _ollamaService.IsAvailableAsync();
            StatusMessage = OllamaAvailable
                ? "AI ready"
                : "AI not available - grading features disabled";

            // Start periodic check for Ollama availability (every 30 seconds)
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(30000);
                    await CheckOllamaAvailability();
                }
            });
        }
        catch
        {
            OllamaAvailable = false;
            StatusMessage = "AI connection failed";
        }
    }

    private async Task CheckOllamaAvailability()
    {
        try
        {
            if (_ollamaService != null)
            {
                var wasAvailable = OllamaAvailable;
                OllamaAvailable = await _ollamaService.IsAvailableAsync();

                // Update status message only if state changed
                if (wasAvailable != OllamaAvailable && !IsProcessing)
                {
                    StatusMessage = OllamaAvailable
                        ? "Ollama is now available"
                        : "Ollama connection lost";
                }
            }
        }
        catch
        {
            // Silently fail - don't update UI on background check failure
        }
    }

    private async void LoadRubric()
    {
        if (SelectedCourse == null || SelectedLanguage == null || SelectedAssignment == null)
            return;

        try
        {
            var rubricDir = Path.Combine(_config.RubricDirectory,
                SelectedCourse,
                SelectedLanguage.Name);

            // Try JSON format first
            var jsonPath = Path.Combine(rubricDir, $"{SelectedAssignment}.json");
            var txtPath = Path.Combine(rubricDir, $"{SelectedAssignment}.txt");

            string rubricPath = File.Exists(jsonPath) ? jsonPath : txtPath;

            if (File.Exists(rubricPath))
            {
                if (rubricPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    // Load and format JSON rubric
                    var rubric = await _rubricService.LoadRubricAsync(rubricPath);
                    if (rubric != null)
                    {
                        RubricContent = _rubricService.FormatRubricForDisplay(rubric);
                    }
                    else
                    {
                        RubricContent = "Error parsing JSON rubric";
                    }
                }
                else
                {
                    // Load plain text rubric
                    RubricContent = await _rubricService.LoadPlainTextRubricAsync(rubricPath);
                }
            }
            else
            {
                RubricContent = "No rubric available";
            }
        }
        catch (Exception ex)
        {
            RubricContent = $"Error loading rubric: {ex.Message}";
            StatusMessage = "Failed to load rubric";
        }
    }

    [RelayCommand]
    private async Task ExportJsonAsync()
    {
        if (StorageProvider == null)
        {
            StatusMessage = "File picker not available";
            return;
        }

        try
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Grading Results as JSON",
                SuggestedFileName = $"{StudentName?.Replace(" ", "_") ?? "grading_results"}_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                DefaultExtension = "json"
            });

            if (file != null)
            {
                var results = new
                {
                    StudentName,
                    StudentId,
                    Grade,
                    LetterGrade,
                    Course = SelectedCourse,
                    Assignment = SelectedAssignment,
                    Language = SelectedLanguage?.DisplayName,
                    SourceFile = SelectedSourceFile,
                    InstructorFeedback,
                    ProgramOutput,
                    AiAnalysis,
                    Timestamp = DateTime.Now
                };

                var json = System.Text.Json.JsonSerializer.Serialize(results, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await using var stream = await file.OpenWriteAsync();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(json);

                StatusMessage = $"Exported to {file.Name}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        if (StorageProvider == null)
        {
            StatusMessage = "File picker not available";
            return;
        }

        try
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Grading Results as CSV",
                SuggestedFileName = $"grading_results_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                DefaultExtension = "csv"
            });

            if (file != null)
            {
                var csv = new System.Text.StringBuilder();

                // Header
                csv.AppendLine("Student Name,Student ID,Grade,Letter Grade,Course,Assignment,Language,Source File,Instructor Feedback,Timestamp");

                // Data (escape commas and quotes)
                csv.AppendLine($"\"{Escape(StudentName)}\",\"{Escape(StudentId)}\",{Grade},\"{LetterGrade}\",\"{Escape(SelectedCourse ?? "")}\",\"{Escape(SelectedAssignment ?? "")}\",\"{Escape(SelectedLanguage?.DisplayName ?? "")}\",\"{Escape(SelectedSourceFile ?? "")}\",\"{Escape(InstructorFeedback)}\",\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"");

                await using var stream = await file.OpenWriteAsync();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(csv.ToString());

                StatusMessage = $"Exported to {file.Name}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export failed: {ex.Message}";
        }
    }

    private static string Escape(string? value)
    {
        return value?.Replace("\"", "\"\"") ?? "";
    }

    private static string CalculateLetterGrade(decimal? grade)
    {
        if (!grade.HasValue || grade.Value < 0)
            return "-";

        // Standard BYU-Idaho Grading Scale
        return grade.Value switch
        {
            >= 93 => "A",
            >= 90 => "A-",
            >= 87 => "B+",
            >= 83 => "B",
            >= 80 => "B-",
            >= 77 => "C+",
            >= 73 => "C",
            >= 70 => "C-",
            >= 67 => "D+",
            >= 63 => "D",
            >= 60 => "D-",
            _ => "F"
        };
    }
}