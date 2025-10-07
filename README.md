# IntelliGrade

AI-powered desktop application for automated grading of programming assignments.

## Why IntelliGrade?

Grading programming assignments is time-consuming and repetitive. IntelliGrade streamlines the process by:

- ⚡ **Saving Time** - Automated code execution and AI analysis reduces grading time by 70%
- 🎯 **Consistent Grading** - Rubric-based evaluation ensures fair, standardized assessment
- 🔒 **Privacy-Focused** - Run AI locally with Ollama or use your own API keys
- 📈 **Detailed Feedback** - Students get comprehensive, actionable feedback
- 🌍 **Cross-Platform** - Works seamlessly on macOS, Windows, and Linux

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

### 🖥️ Cross-Platform Desktop Application
- **Native Performance** - Built with Avalonia UI for macOS, Windows, and Linux
- **Modern Interface** - Clean, intuitive design with Calibri typography
- **Dark/Light Theme** - Automatically detects and matches your system theme
- **Responsive Layout** - Optimized for various screen sizes

### 🌐 Multi-Language Programming Support
- **Automatic Language Detection** - Scans directories and identifies programming languages
- **Wide Language Coverage**:
  - Python (.py)
  - C++ (.cpp, .h)
  - C# (.cs)
  - Java (.java)
  - JavaScript (.js)
  - C (.c)
  - PHP (.php)
  - Ruby (.rb)
  - Go (.go)
  - Rust (.rs)
- **Smart File Management** - Detects source files, test files, and program outputs

### 🤖 AI-Powered Grading
- **Multiple AI Providers**:
  - **Ollama** - Local AI processing (privacy-friendly, free)
  - **OpenAI** - GPT models with custom API key
  - **Anthropic** - Claude models with custom API key
- **Rubric-Based Analysis** - AI strictly follows assignment criteria
- **Detailed Feedback** - Complete/Developing/Missing format with explanations
- **API Key Validation** - Ensures correct format before saving

### 📝 Rubric & Course Management
- **JSON-Based Rubrics** - Easy to create and modify
- **Rubric Import** - Import from JSON, text, or markdown files
- **Course Organization** - Organize by course code and programming language
- **Assignment Templates** - Reusable grading criteria
- **Multi-Course Support** - Handle multiple classes simultaneously

### 🔄 Code Execution & Testing
- **Safe Execution** - Run student code with timeout protection
- **Real-Time Output** - View program output as it executes
- **Output File Detection** - Automatically displays .txt files created by programs
- **Error Handling** - Captures and displays compilation/runtime errors
- **Multiple Test Cases** - Run code against different inputs

### 📊 Grading & Assessment
- **Student Information** - Track student names and IDs
- **Numeric Grading** - Enter grades from 0-100
- **Letter Grade Calculation** - Automatic conversion using BYU-Idaho scale
- **Instructor Feedback** - Add custom comments and notes
- **Grade History** - Track multiple submissions per student

### 🗂️ File & Repository Management
- **Directory Browser** - Navigate and select submission folders
- **Git Integration** - Clone repositories directly from GitHub/GitLab
- **File Cleanup** - Remove compiled binaries and temporary files
- **Batch Processing** - Grade multiple submissions efficiently

### 💾 Export & Reporting
- **CSV Export** - Compatible with spreadsheet software
- **JSON Export** - Structured data for custom processing
- **Gradebook Integration** - Format compatible with LMS systems
- **Detailed Reports** - Include code, output, AI analysis, and grades

### ⚙️ Customization & Settings
- **API Configuration** - Manage AI service credentials
- **Custom Endpoints** - Configure Ollama server URL
- **Model Selection** - Choose specific AI models
- **Theme Preferences** - Override system theme if desired
- **Keyboard Shortcuts** - Efficient navigation and workflow

## Usage Workflow

### Initial Setup
1. **Launch IntelliGrade** - Start the application
2. **Configure API Settings** (optional) - Click Settings icon to add API keys for OpenAI or Anthropic
3. **Create or Import Rubrics** - Set up grading criteria for your assignments

### Grading Process
1. **Select Working Directory**
   - Click "Select Directory" to choose folder with student submissions
   - Or use "Clone Git Repo" to pull directly from GitHub

2. **Detect Languages**
   - Click "Detect Languages" to scan the directory
   - Application identifies all programming languages present
   - Select the appropriate language from dropdown

3. **Choose Files**
   - **Source File** - Select the student's code to grade
   - **Course** - Choose the course from your managed courses
   - **Assignment** - Select the assignment/rubric to use

4. **Review Code**
   - View student's source code in the "Source Code" tab
   - Check the "Rubric" tab to review grading criteria

5. **Execute & Test**
   - Click "Run Program" to execute the code
   - View output in the "Output" tab
   - Program automatically detects and displays .txt output files
   - Check for compilation errors or runtime issues

6. **AI Analysis**
   - Click "Analyze with AI" to get automated feedback
   - AI evaluates code against rubric criteria
   - View detailed analysis in "AI Analysis" tab
   - Get point-by-point assessment with explanations

7. **Grade & Provide Feedback**
   - Go to "Grading" tab
   - Enter student name and ID
   - Input grade (0-100) - letter grade calculated automatically
   - Add instructor feedback and comments
   - Review AI suggestions and adjust as needed

8. **Export Results**
   - Export individual grades to JSON
   - Batch export to CSV for gradebook import
   - Include all analysis and feedback in reports

### Maintenance
- **Manage Courses** - Add, edit, or remove course configurations
- **Import Rubrics** - Add new assignment rubrics
- **Cleanup Files** - Remove compiled binaries and temp files between submissions

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

## Supported File Types

### Source Code Files
- `.py` - Python
- `.cpp`, `.cc`, `.cxx`, `.hpp`, `.h` - C++
- `.c`, `.h` - C
- `.cs` - C#
- `.java` - Java
- `.js` - JavaScript
- `.php` - PHP
- `.rb` - Ruby
- `.go` - Go
- `.rs` - Rust

### Rubric Files
- `.json` - Structured JSON rubrics (recommended)
- `.txt` - Plain text rubrics
- `.md` - Markdown rubrics

### Output Files
- `.txt` - Text output files (automatically detected and displayed)
- Standard output/error streams

## Troubleshooting

### AI Not Working
- **Ollama**: Make sure Ollama is running (`ollama serve`) and the model is pulled
- **OpenAI/Anthropic**: Verify your API key is correct and has valid format
- Check API settings in Settings dialog

### Code Won't Execute
- Ensure the programming language compiler/interpreter is installed
- Check file permissions
- Verify the selected source file is correct

### Rubrics Not Loading
- Check rubric file format (JSON syntax)
- Ensure rubrics are in correct directory: `TestData/Rubrics/{Course}/{Language}/`
- Verify file has proper extension

### Build Script Issues
- **macOS**: If you get permission errors, run `chmod +x build-macos.sh`
- **Windows**: Run PowerShell as Administrator if needed
- Ensure .NET 9.0 SDK is installed

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

MIT License - See LICENSE file for details

---

**Built with ❤️ for educators and students**
