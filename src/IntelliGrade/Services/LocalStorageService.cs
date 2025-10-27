using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using IntelliGrade.App.Interfaces;

namespace IntelliGrade.App.Services;

/// <summary>
/// Provides local storage capabilities for application settings and state
/// </summary>
public class LocalStorageService : ILocalStorageService
{
    private readonly string _storageDirectory;
    private readonly string _settingsFile;

    public LocalStorageService()
    {
        _storageDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IntelliGrade"
        );

        _settingsFile = Path.Combine(_storageDirectory, "settings.json");

        // Ensure directory exists
        Directory.CreateDirectory(_storageDirectory);
    }

    /// <summary>
    /// Saves a value to local storage
    /// </summary>
    public async Task SetAsync<T>(string key, T value)
    {
        try
        {
            var settings = await LoadSettingsAsync();
            settings[key] = JsonSerializer.Serialize(value);
            await SaveSettingsAsync(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to local storage: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a value from local storage
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, T? defaultValue = default)
    {
        try
        {
            var settings = await LoadSettingsAsync();

            if (settings.TryGetValue(key, out var json) && !string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<T>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from local storage: {ex.Message}");
        }

        return defaultValue;
    }

    /// <summary>
    /// Removes a value from local storage
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        try
        {
            var settings = await LoadSettingsAsync();
            settings.Remove(key);
            await SaveSettingsAsync(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing from local storage: {ex.Message}");
        }
    }

    /// <summary>
    /// Clears all local storage
    /// </summary>
    public async Task ClearAsync()
    {
        try
        {
            if (File.Exists(_settingsFile))
            {
                File.Delete(_settingsFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing local storage: {ex.Message}");
        }

        await Task.CompletedTask;
    }

    private async Task<Dictionary<string, string>> LoadSettingsAsync()
    {
        if (!File.Exists(_settingsFile))
        {
            return new Dictionary<string, string>();
        }

        try
        {
            var json = await File.ReadAllTextAsync(_settingsFile);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                   ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    private async Task SaveSettingsAsync(Dictionary<string, string> settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(_settingsFile, json);
    }
}
