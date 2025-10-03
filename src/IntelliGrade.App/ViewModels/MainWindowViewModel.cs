using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliGrade.App.Models;
using IntelliGrade.App.Services;

namespace IntelliGrade.App.ViewModels;

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
    [ObservableProperty] private decimal _grade;
    [ObservableProperty] private string _instructorFeedback = string.Empty;

    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private bool _ollamaAvailable;
    [ObservableProperty] private bool _isDarkMode;
    [ObservableProperty] private bool _showWelcomeScreen = true;

    public string LetterGrade => CalculateLetterGrade(Grade);
    public string WindowClass => IsDarkMode ? "dark" : "";

    public MainWindowViewModel()
    {
        LoadCourses();
        InitializeOllama();
        LoadSettings();
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        OnPropertyChanged(nameof(WindowClass));
        SaveThemePreference();
    }

    [RelayCommand]
    private void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
    }

    private async void LoadSettings()
    {
        try
        {
            IsDarkMode = await _localStorage.GetAsync("IsDarkMode", false);
            var dir = await _localStorage.GetAsync("LastDirectory", Directory.GetCurrentDirectory());
            if (dir != null) CurrentDirectory = dir;
        }
        catch { }
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

    partial void OnGradeChanged(decimal value)
    {
        OnPropertyChanged(nameof(LetterGrade));
    }

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

            ProgramOutput = result.success 
                ? $"✓ Success\n\nOutput:\n{result.output}" 
                : $"✗ Failed\n\nError:\n{result.error}";
            
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

            // Try JSON first, then .txt
            var jsonPath = Path.Combine(rubricDir, $"{SelectedAssignment ?? "default"}.json");
            var txtPath = Path.Combine(rubricDir, $"{SelectedAssignment ?? "default"}.txt");

            string rubricPath = File.Exists(jsonPath) ? jsonPath : txtPath;
            string formattedRubric;

            if (File.Exists(rubricPath))
            {
                // Load and format rubric for AI (uses special formatting for JSON)
                formattedRubric = await _rubricService.LoadAndFormatRubricAsync(rubricPath);
            }
            else
            {
                formattedRubric = "General code quality assessment";
            }

            var analysis = await _ollamaService.AnalyzeCodeAsync(
                SourceCode, formattedRubric,
                SelectedCourse ?? "Unknown",
                SelectedAssignment ?? "Unknown",
                new List<string>());

            AiAnalysis = analysis;
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
        try
        {
            _ollamaService = new OllamaGradingService(_config.OllamaModel);
            OllamaAvailable = await _ollamaService.IsAvailableAsync();
            StatusMessage = OllamaAvailable
                ? "Ollama ready"
                : "Ollama not available - AI grading disabled";

            // Start periodic check for Ollama availability (every 30 seconds)
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(30000); // Check every 30 seconds
                    await CheckOllamaAvailability();
                }
            });
        }
        catch
        {
            OllamaAvailable = false;
            StatusMessage = "Ollama connection failed";
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

    private static string CalculateLetterGrade(decimal grade)
    {
        // BYU-Idaho Grading Scale
        return grade switch
        {
            >= 97 => "A+",
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