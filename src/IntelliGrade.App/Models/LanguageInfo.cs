using System;
using System.Linq;

namespace IntelliGrade.App.Models;

/// <summary>
/// Represents metadata about a programming language.
/// Design: Complete fidelity, immutable after creation.
/// </summary>
public class LanguageInfo
{
    private readonly string _name;
    private readonly string _displayName;
    private readonly string[] _extensions;
    private readonly string _executeCommand;

    public string Name => _name;
    public string DisplayName => _displayName;
    public string[] Extensions => _extensions;
    public string ExecuteCommand => _executeCommand;

    public LanguageInfo(string name, string displayName, string[] extensions, string executeCommand)
    {
        _name = ValidateName(name);
        _displayName = ValidateDisplayName(displayName);
        _extensions = ValidateExtensions(extensions);
        _executeCommand = executeCommand ?? string.Empty;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Language name cannot be empty", nameof(name));
        return name.ToLowerInvariant();
    }

    private static string ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));
        return displayName;
    }

    private static string[] ValidateExtensions(string[] extensions)
    {
        if (extensions == null || extensions.Length == 0)
            throw new ArgumentException("Extensions cannot be empty", nameof(extensions));

        var validated = new string[extensions.Length];
        for (int i = 0; i < extensions.Length; i++)
        {
            validated[i] = extensions[i].StartsWith(".") ? extensions[i] : "." + extensions[i];
        }
        return validated;
    }

    public override string ToString() => $"{DisplayName} ({string.Join(", ", Extensions)})";
}