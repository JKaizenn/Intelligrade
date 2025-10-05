# IntelliGrade

<img width="1346" height="847" alt="Screenshot 2025-10-27 at 11 27 21 AM" src="https://github.com/user-attachments/assets/776719d8-8a9e-459d-a755-37aa08ccd548" />


AI-powered desktop application for automated grading of programming assignments.

## Requirements

- **.NET 9.0** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Ollama** (optional) - [Download](https://ollama.com/) - For AI grading features
  - Install Ollama and run: `ollama pull llama3.2:1b`

## Quick Start

### Running from Source
```bash
cd src/IntelliGrade.App
dotnet run
```

### Building for Distribution

**macOS:**
```bash
./build-macos.sh
open IntelliGrade.app
```

**Windows:**
```powershell
.\build-windows.ps1
.\dist\windows\IntelliGrade.App.exe
```

**Linux:**
```bash
./build-linux.sh
./dist/linux/IntelliGrade.App
```

## Features

- **Multi-Language Support** - Python, C++, C#, Java, JavaScript, and more
- **AI Grading** - Automated rubric-based grading with Ollama
- **Course Management** - Organize assignments by course and language
- **Custom Rubrics** - JSON-based rubric system
- **Export Results** - Save grades to JSON or CSV
- **Dark/Light Theme** - Automatically matches system theme

## Usage

1. **Select Directory** - Choose folder containing student submissions
2. **Detect Languages** - Automatically identifies programming languages used
3. **Select Source File** - Pick the submission to grade
4. **Choose Rubric** - Select the appropriate assignment rubric
5. **Run Program** - Execute the code to see output
6. **Analyze with AI** - Get automated feedback based on rubric
7. **Export** - Save grading results

## Project Structure

```
Intelligrade/
├── src/IntelliGrade.App/     # Main application
│   ├── Models/                # Data models
│   ├── ViewModels/            # MVVM view models
│   ├── Views/                 # UI views
│   ├── Services/              # Business logic
│   └── Styles/                # UI themes
├── TestData/                  # Example data
│   ├── Rubrics/               # Grading rubrics
│   └── Submissions/           # Sample assignments
├── build-macos.sh             # macOS build script
├── build-windows.ps1          # Windows build script
└── build-linux.sh             # Linux build script
```

## Rubric Format

Rubrics are stored in `TestData/Rubrics/{Course}/{Language}/` as JSON files:

```json
{
  "assignmentName": "Assignment Name",
  "totalPoints": 100,
  "criteria": [
    {
      "name": "Functionality",
      "points": 40,
      "description": "Program runs without errors"
    }
  ]
}
```

## Grading Scale

Standard BYU-Idaho grading scale:
- **93-100%** → A
- **90-92.9%** → A-
- **87-89.9%** → B+
- **83-86.9%** → B
- **80-82.9%** → B-
- **77-79.9%** → C+
- **73-76.9%** → C
- **70-72.9%** → C-
- **67-69.9%** → D+
- **63-66.9%** → D
- **60-62.9%** → D-
- **Below 60%** → F

## License

MIT License - See LICENSE file for details
