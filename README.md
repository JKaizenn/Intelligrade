# IntelliGrade

<img width="1353" height="849" alt="Screenshot 2025-10-27 at 3 23 03 PM" src="https://github.com/user-attachments/assets/17eedd81-5a00-4371-8cb7-77fc11f31b7c" />

**Currently in Beta** - Desktop application for grading programming assignments using AI to assist with grading. Looking for testers and feedback!

## Features

- 🤖 **AI Grading** - Automated code analysis with Ollama, OpenAI, or Anthropic (bring your own API keys)
- 🔄 **Code Execution** - Run and test student programs safely
- 📝 **Rubric Support** - JSON and markdown/text-based grading criteria
- 🌐 **Multi-Language** - Python, C++, Java, JavaScript, C#, Rust, Go, and more
- 📊 **Export** - CSV and JSON export for gradebooks
- 🌍 **Cross-Platform** - macOS, Windows, Linux

## Quick Start

### For Beta Testers

**Download the installer for your platform:**
- **macOS**: Download `IntelliGrade-v0.9.0-beta-macOS.dmg` from [Releases](https://github.com/JKaizenn/Intelligrade/releases)
- **Windows**: Download `IntelliGrade-v0.9.0-beta-Windows-Setup.exe` from [Releases](https://github.com/JKaizenn/Intelligrade/releases)

**No .NET installation required** - installers include everything you need!

**Optional (for local AI grading):**
- Install [Ollama](https://ollama.com/) and run `ollama pull llama3.2:1b`
- Or use OpenAI/Anthropic API keys (configure in Settings)

### For Developers

**Requirements:**
- [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)

**Run from source:**
```bash
cd src/IntelliGrade
dotnet run
```

**Build installers:**
```bash
# macOS (creates DMG)
./build-macos.sh
# Output: IntelliGrade-v0.9.0-beta-macOS.dmg

# Windows (creates installer if Inno Setup is installed)
.\build-windows.ps1
# Output: dist\IntelliGrade-v0.9.0-beta-Windows-Setup.exe

# Linux (creates AppImage/binary)
./build-linux.sh
```

## Using IntelliGrade

### Navigation

The app has a simple layout:
- **Top Bar** - Settings (⚙️), Home (🏠), and Dark Mode toggle buttons
- **Main Area** - Grading interface with tabs for different views
- **Status Bar** - Shows current operation status

### Step-by-Step Guide

1. **Select Student Directory**
   - Click "Browse" on the welcome screen
   - Choose the folder containing student submissions

2. **Detect Languages**
   - Click "Detect Languages" to scan for code files
   - The app will find all programming languages in the directory

3. **Configure Assignment**
   - Select the programming language from the dropdown
   - Choose the specific source file to grade
   - Select your course (e.g., CSE 101, CSE 232)
   - Pick the rubric for this assignment

4. **Run the Code** (Optional)
   - Click "Run Program" to execute the student's code
   - View the output to verify functionality
   - Check for errors or unexpected behavior

5. **AI Analysis** (Optional)
   - Click "Analyze with AI" to get automated feedback
   - The AI will review the code against the rubric
   - View suggestions in the AI Analysis panel

6. **Enter Grade**
   - Fill in student name and ID
   - Enter the numeric grade (0-100)
   - Add instructor feedback in the text area
   - The letter grade calculates automatically

7. **Export Results**
   - Click "Export to CSV" or "Export to JSON"
   - Save grades for import into your gradebook system

### Managing Courses and Rubrics

- Click the **Settings (⚙️)** button in the top-right
- Use "Manage Courses" to add/remove courses
- Use "Import Rubric" to add new grading rubrics

## AI Configuration

### Using Ollama (Local AI)

1. Install [Ollama](https://ollama.com/)
2. Pull a model: `ollama pull llama3.2:1b`
3. IntelliGrade will automatically detect Ollama if it's running

### Using OpenAI

1. Get your API key from [OpenAI Platform](https://platform.openai.com/)
2. Click Settings (⚙️) → Enter your OpenAI API key
3. Select your preferred model (e.g., gpt-4, gpt-3.5-turbo)
4. Enable "Use OpenAI"

### Using Anthropic Claude

1. Get your API key from [Anthropic Console](https://console.anthropic.com/)
2. Click Settings (⚙️) → Enter your Anthropic API key
3. Select your preferred model (e.g., claude-3-5-sonnet-20241022)
4. Enable "Use Anthropic"

**Note:** You are responsible for any costs associated with API usage. API keys are stored locally and never shared.

## Rubric Format

Place rubrics in `~/bin/rubrics/{Course}/{Language}/`

### JSON Format (Recommended)

```json
{
  "course": "CSE 101",
  "assignment": "Lab 1",
  "totalPoints": 100,
  "criteria": [
    {
      "name": "Functionality",
      "maxPoints": 40,
      "ratings": [
        { "points": 40, "description": "Works perfectly" },
        { "points": 20, "description": "Partial functionality" },
        { "points": 0, "description": "Doesn't work" }
      ]
    }
  ]
}
```

### Text/Markdown Format (Simple)

```markdown
# Assignment Rubric - CSE 101

## Requirements (100 points total)

### Functionality (40 points)
- Works perfectly (40 points)
- Partial functionality (20 points)
- Doesn't work (0 points)

## Grading Scale
- A: 90-100
- B: 80-89
- C: 70-79
- D: 60-69
- F: Below 60
```

## Development

```bash
# Build from source
dotnet build src/IntelliGrade/IntelliGrade.App.csproj

# Run from source
cd src/IntelliGrade
dotnet run

# Create distributable installers (see Build installers section above)
./build-macos.sh      # macOS DMG
.\build-windows.ps1   # Windows Setup.exe
./build-linux.sh      # Linux binary
```

## Beta Testing & Feedback

This is a beta release! If you encounter bugs or have feature suggestions:
- Open an issue on GitHub
- Describe your use case and workflow
- Include your OS and .NET version

## License

MIT License

---

Built with ❤️ for educators
