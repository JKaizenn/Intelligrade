# IntelliGrade

<img width="1346" height="847" alt="Screenshot 2025-10-27 at 11 27 21 AM" src="https://github.com/user-attachments/assets/776719d8-8a9e-459d-a755-37aa08ccd548" />

AI-powered desktop application for grading programming assignments.

## Features

- 🤖 **AI Grading** - Automated code analysis with Ollama, OpenAI, or Anthropic
- 🔄 **Code Execution** - Run and test student programs safely
- 📝 **Rubric Support** - JSON-based grading criteria
- 🌐 **Multi-Language** - Python, C++, Java, JavaScript, C#, and more
- 📊 **Export** - CSV and JSON export for gradebooks
- 🌍 **Cross-Platform** - macOS, Windows, Linux

## Quick Start

**Requirements:**
- [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Ollama](https://ollama.com/) (optional, for AI grading)

**Run from source:**
```bash
cd src/IntelliGrade.App
dotnet run
```

**Build for your platform:**
```bash
# macOS
./build-macos.sh

# Windows
.\build-windows.ps1

# Linux
./build-linux.sh
```

## Usage

1. **Select Directory** - Choose folder with student code
2. **Detect Languages** - Scan for programming languages
3. **Select Files** - Pick source file, course, and rubric
4. **Run Program** - Execute and view output
5. **Analyze with AI** - Get automated feedback
6. **Grade** - Enter score and feedback
7. **Export** - Save to CSV or JSON

## Rubric Format

Place rubrics in `~/bin/rubrics/{Course}/{Language}/assignment.json`:

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

## Development

```bash
# Run tests
dotnet test

# Build
dotnet build src/IntelliGrade.App/IntelliGrade.App.csproj
```

## License

MIT License

---

Built with ❤️ for educators
