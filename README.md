# IntelliGrade

<img width="1358" height="852" alt="IntelliGrade Screenshot" src="https://github.com/user-attachments/assets/02bb18fc-cb63-4c08-941b-dd9e9484805d" />

Desktop application for quickly running and grading student code with local AI analysis assisting. Currently in beta.

## Features

- Quick and easy interactive program execution with terminal support
- Local AI analysis assisting using Ollama models
- Multiple analysis modes (Quick, Balanced, Thorough)
- Flexible rubric system (JSON and markdown formats)
- Multi-language support (Python, C++, Java, JavaScript, C#, Rust, Go, and more)
- Grade export to CSV and JSON
- Cross-platform (macOS, Windows, Linux)
- Dark mode support

## Installation

### Beta Release

Download the installer for your platform from [Releases](https://github.com/JKaizenn/Intelligrade/releases):

- **macOS**: `IntelliGrade-v0.9.0-beta-macOS.dmg`
- **Windows**: `IntelliGrade-v0.9.0-beta-Windows-Setup.exe`

No .NET installation required. Installers are self-contained.

### AI Setup (Optional)

For AI grading features:
1. Install [Ollama](https://ollama.com/)
2. Pull a model: `ollama pull llama3.2:1b`
3. IntelliGrade will auto-detect running Ollama instances

## Usage

### Basic Workflow

1. **Select Directory** - Browse to student submission folder or clone from GitHub
2. **Detect Languages** - Scan directory for programming languages
3. **Configure** - Select language, source file, course, and rubric
4. **Run & Test** - Execute code interactively in the terminal
5. **AI Analysis** - Get automated feedback (requires Ollama)
6. **Grade** - Enter student info, score, and feedback
7. **Export** - Save grades to CSV or JSON

### Interface Overview

- **Header** - AI status indicator, feedback, settings, home, and theme toggle
- **Main Tabs** - Source code, program output, interactive terminal, rubric view
- **Sidebar** - Student info, grading controls, AI analysis results
- **Status Bar** - Operation progress and version info

### Analysis Modes

- **Quick** - Fast analysis for simple assignments
- **Balanced** - Standard thoroughness for most cases
- **Thorough** - Deep analysis with detailed feedback

## Rubric Configuration

Rubrics are stored in `~/bin/rubrics/{Course}/{Language}/`

### JSON Format

```json
{
  "course": "CSE 101",
  "assignment": "Lab 1",
  "totalPoints": 100,
  "criteria": [
    {
      "name": "Functionality",
      "maxPoints": 40,
      "description": "Program works as specified",
      "levels": [
        { "label": "Complete", "points": 40, "description": "Fully functional" },
        { "label": "Partial", "points": 20, "description": "Partially working" },
        { "label": "None", "points": 0, "description": "Does not work" }
      ]
    }
  ]
}
```

### Markdown Format

```markdown
# Lab 1 Rubric

## Functionality (40 points)
- Fully functional: 40 points
- Partially working: 20 points
- Does not work: 0 points

## Code Quality (30 points)
- Excellent: 30 points
- Good: 20 points
- Poor: 10 points
```

## Development

### Requirements

- [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)

### Build from Source

```bash
cd src/IntelliGrade
dotnet run
```

### Create Installers

```bash
# macOS
./scripts/build-macos.sh

# Windows
.\scripts\build-windows.ps1

# Linux
./scripts/build-linux.sh
```

## Feedback

Report bugs or suggest features by opening an issue on GitHub. Include your OS and details about the issue.

## License

MIT License

---

Built for educators
