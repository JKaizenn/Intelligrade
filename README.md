# IntelliGrade 🤖📚

A cross-platform desktop application for automated grading of programming assignments. Built with C# and Avalonia UI, IntelliGrade streamlines the grading workflow with intelligent code analysis, multi-language support, and customizable rubrics.

## ✨ Features

### 🖥️ Modern Desktop Experience
- **Cross-Platform** - Runs natively on Windows, macOS, and Linux
- **Avalonia UI** - Beautiful, responsive interface with MVVM architecture
- **Drag & Drop** - Simply drop assignment files to grade them
- **Real-time Feedback** - Watch as code is analyzed and graded
- **Dark/Light Theme** - Comfortable viewing in any environment

### 🌐 Multi-Language Support
- **Python** (.py) - CSE 111 Programming with Functions
- **C#** (.cs) - CSE 210 Programming with Classes  
- **JavaScript** (.js) - Web development assignments
- **HTML/CSS** (.html, .css) - Frontend projects
- **Java, C++, C, PHP, Ruby, Go, Rust** - Additional language support
- **Automatic Detection** - IntelliGrade identifies the language automatically

### 🤖 AI-Powered Analysis
- **Local AI Processing** - Uses Ollama for privacy-friendly analysis
- **Rubric-Based Grading** - Strict adherence to assignment criteria
- **Creativity Detection** - Recognizes and credits additional features
- **Structured Feedback** - Complete/Developing/Missing format with point breakdowns
- **Detailed Reports** - Export grading results with full analysis

### 📁 Intelligent File Handling
- **Automatic File Detection** - Finds source files and outputs
- **Assignment-Specific Data** - Automatically handles required test files
- **Smart Display** - Syntax highlighting for code review
- **Safe Execution** - Sandboxed code execution with timeout protection
- **Batch Processing** - Grade multiple submissions at once

### 📋 Course Management
- **Multi-Course Support** - CSE 111, CSE 210, and extensible to any course
- **Week-Based Organization** - Structured rubrics by course and assignment
- **Template System** - Easy addition of new courses and assignments
- **Grade Export** - Export to CSV, JSON, or integrate with LMS

## 🚀 Quick Start

### Prerequisites
- **.NET 8.0 SDK** or later
- **Ollama** - For AI analysis (optional but recommended)
- **Platform-specific compilers** - For grading non-C# assignments:
  - Python 3 for Python assignments
  - JDK for Java assignments
  - C++ compiler for C++ assignments

## 🖥️ Installation by Operating System

### 🍎 macOS Installation

1. **Install .NET SDK**
```bash
# Using Homebrew (recommended)
brew install --cask dotnet

# OR download from https://dotnet.microsoft.com/download
```

2. **Install Ollama (Optional)**
```bash
# Using Homebrew
brew install ollama

# Start Ollama and install AI model
ollama pull llama3.1:8b
```

3. **Clone and Build IntelliGrade**
```bash
git clone https://github.com/JKaizenn/Intelligrade.git
cd Intelligrade
dotnet restore
dotnet build
dotnet run
```

### 🐧 Linux Installation

1. **Install .NET SDK**
```bash
# Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0

# Arch Linux
sudo pacman -S dotnet-sdk

# Fedora
sudo dnf install dotnet-sdk-8.0
```

2. **Install Ollama (Optional)**
```bash
curl -fsSL https://ollama.ai/install.sh | sh
ollama pull llama3.1:8b
```

3. **Clone and Build IntelliGrade**
```bash
git clone https://github.com/JKaizenn/Intelligrade.git
cd Intelligrade
dotnet restore
dotnet build
dotnet run
```

### 🪟 Windows Installation

1. **Install .NET SDK**
- Download and install from: https://dotnet.microsoft.com/download
- Verify installation: `dotnet --version`

2. **Install Ollama (Optional)**
- Download from: https://ollama.ai/download
- Install and run: `ollama pull llama3.1:8b`

3. **Clone and Build IntelliGrade**
```powershell
git clone https://github.com/JKaizenn/Intelligrade.git
cd Intelligrade
dotnet restore
dotnet build
dotnet run
```

### 🔧 Verification Steps (All Platforms)

After installation, verify everything works:

```bash
# Check .NET SDK
dotnet --version

# Check Ollama (if installed)
ollama --version
ollama list  # Should show llama3.1:8b

# Run IntelliGrade
cd Intelligrade
dotnet run
```

## 💻 Usage

### Basic Grading Workflow

1. **Launch IntelliGrade**
```bash
cd Intelligrade
dotnet run
```

2. **Select Course and Assignment**
- Choose from available courses (CSE 111, CSE 210, etc.)
- Select the assignment week

3. **Add Student Submissions**
- Drag and drop files into the application
- Or use the file browser to select submissions
- IntelliGrade will automatically detect the programming language

4. **Review and Grade**
- View source code with syntax highlighting
- See execution output and test results
- Review AI analysis against rubric
- Adjust scores if needed

5. **Export Results**
- Export grades to CSV for LMS import
- Generate detailed feedback reports
- Save grading session for later review

### Expected Interface
```
┌────────────────────────────────────────┐
│  IntelliGrade                    ⚙️ 🌙 │
├────────────────────────────────────────┤
│  Course: CSE 111    Week: 01           │
│  Assignment: Tire Volume Calculator    │
├────────────────────────────────────────┤
│  📂 Submissions (15)                   │
│    ✅ student1_tire_volume.py  95/100  │
│    ⏳ student2_tire_volume.py  ...     │
│    ❌ student3_tire_volume.py  ERROR   │
├────────────────────────────────────────┤
│  📊 Grade Distribution                 │
│  ████████████░░░░░ 85.3 average        │
└────────────────────────────────────────┘
```

## 📂 Project Structure

```
Intelligrade/
├── Assets/                      # UI resources (icons, images)
├── Models/                      # Data models
│   ├── Course.cs
│   ├── Assignment.cs
│   ├── Submission.cs
│   └── GradeResult.cs
├── ViewModels/                  # MVVM view models
│   ├── MainWindowViewModel.cs
│   ├── GradingViewModel.cs
│   └── CourseSelectionViewModel.cs
├── Views/                       # XAML views
│   ├── MainWindow.axaml
│   ├── GradingView.axaml
│   └── CourseSelectionView.axaml
├── Services/                    # Business logic
│   ├── CodeExecutionService.cs
│   ├── AIAnalysisService.cs
│   ├── RubricService.cs
│   └── LanguageDetectionService.cs
├── Rubrics/                     # Assignment rubrics
│   ├── cse111/
│   │   └── python/
│   │       ├── week01.txt
│   │       └── week02.txt
│   └── cse210/
│       └── csharp/
│           └── week01.txt
├── App.axaml
├── Program.cs
└── Intelligrade.csproj
```

## 🎯 Supported Assignment Types

### CSE 111 Week 01 - Tire Volume Calculator (Python)
- Validates mathematical calculations
- Checks file operations and date formatting
- Analyzes against comprehensive rubric

### CSE 111 Week 02 - Password Strength Checker (Python)
- Tests function definitions and implementations
- Validates password complexity algorithms
- Checks file handling and user interface

### CSE 210 - C# Assignments
- Compiles and runs C# programs
- Analyzes OOP principles and class structure
- Validates C# coding standards and best practices

### Custom Assignments
- Easily add new courses and assignments
- Define custom rubrics and test cases
- Support any programming language with a compiler/interpreter

## 🔧 Customization

### Adding New Courses

1. **Create Course Directory**
```bash
mkdir -p Rubrics/[course_code]/[language]/
```

2. **Add Rubric File**
```
Rubrics/cse120/python/week01.txt
```

3. **Define Rubric Format**
```
# Course: CSE 120, Assignment: Week 01, Language: Python

ASSIGNMENT: Hello World Program

RUBRIC CRITERIA:
1. Program Output (20 points)
   - Complete (20): Prints "Hello, World!" correctly
   - Developing (10): Prints with minor errors
   - Missing (0): No output or incorrect output

2. Code Structure (10 points)
   - Complete (10): Clean, well-commented code
   - Developing (5): Working but messy
   - Missing (0): No structure

TOTAL POINTS: 30
```

### Configuring AI Analysis

Edit `appsettings.json`:
```json
{
  "Ollama": {
    "Endpoint": "http://localhost:11434",
    "Model": "llama3.1:8b",
    "Timeout": 30
  },
  "Grading": {
    "MaxExecutionTime": 10,
    "EnableAIAnalysis": true,
    "StrictMode": false
  }
}
```

## 🛠️ Troubleshooting

### Common Issues

**"SDK not found" or build errors**
```bash
dotnet --version  # Verify .NET 8.0+
dotnet restore
dotnet build
```

**"Ollama connection failed"**
```bash
ollama --version
ollama serve  # Start Ollama service
ollama pull llama3.1:8b
```

**Application won't start**
```bash
# Clear build cache
dotnet clean
dotnet build
dotnet run
```

**Cross-platform rendering issues**
- Update Avalonia packages: `dotnet add package Avalonia --version [latest]`
- Check graphics drivers are up to date

## 🔒 Privacy & Security

- **Local AI Processing** - All analysis happens locally via Ollama (no cloud required)
- **Sandboxed Execution** - Student code runs in isolated processes with timeouts
- **No Data Collection** - Zero telemetry, all data stays on your machine
- **Resource Limits** - Configurable memory and CPU limits for code execution
- **Secure File Handling** - Temporary files are cleaned up automatically

## 🤝 Contributing

IntelliGrade is open source and welcomes contributions! Here's how you can help:

### Development Setup
```bash
git clone https://github.com/JKaizenn/Intelligrade.git
cd Intelligrade
dotnet restore
dotnet build

# Run with hot reload for development
dotnet watch run
```

### Contribution Areas
- **Language Support** - Add execution engines for new languages
- **UI/UX Improvements** - Enhance the Avalonia interface
- **Rubric Templates** - Create standardized rubrics for common courses
- **AI Improvements** - Better prompts and analysis algorithms
- **Performance** - Optimize code execution and batch processing
- **Testing** - Add unit and integration tests

### Code Style
- Follow C# coding conventions
- Use MVVM pattern for UI code
- Add XML documentation for public APIs
- Write unit tests for new features

## 📊 Performance

- **Startup Time**: ~2-3 seconds (native app)
- **UI Responsiveness**: 60 FPS on modern hardware
- **AI Analysis**: ~10-20 seconds per assignment (with Ollama)
- **Batch Grading**: ~100 submissions in under 5 minutes
- **Memory Usage**: ~150MB base + AI model
- **Disk Space**: ~500MB (app + AI model)

## 💡 Development Philosophy

### 🤖 AI-Assisted Development
The original bash script version of IntelliGrade was developed through AI collaboration. This C# Avalonia rewrite represents a complete manual rebuild to:

- **Deepen Understanding** - Learn C# and Avalonia through hands-on development
- **Improve Architecture** - Apply proper software design patterns (MVVM)
- **Enhance Maintainability** - Type-safe, compiled code with better tooling
- **Expand Capabilities** - Modern UI, better performance, more features

### 🚀 Future Development Plans

**Planned Features:**
- 🌐 **Web Interface** - Browser-based grading dashboard (Blazor WebAssembly)
- 📱 **Mobile App** - iOS/Android version using Avalonia Mobile
- 🔄 **LMS Integration** - Direct Canvas/Blackboard gradebook sync
- 📊 **Analytics Dashboard** - Class performance insights and trends
- 🔍 **Plagiarism Detection** - Code similarity analysis
- 🎯 **Custom AI Models** - Fine-tuned models for specific programming courses
- 🔧 **Plugin System** - Community-contributed language support

**Alternative Implementations:**
- ⚡ **CLI Tool in Rust** - Fast, compiled binary for CI/CD integration
- 🐳 **Docker Container** - Portable grading environment
- ☁️ **Cloud Service** - Scalable grading API for institutions

## 🎓 Educational Impact

IntelliGrade is designed for:
- **Computer Science Courses** - Automated grading for programming assignments
- **Bootcamps** - Fast feedback for large cohorts
- **Online Learning** - Scalable assessment for MOOCs
- **Teaching Assistants** - Reduce repetitive grading work
- **Students** - Quick self-assessment before submission

## 📝 License

MIT License - See LICENSE file for details

This project is designed for educational use. Please ensure compliance with your institution's policies regarding automated grading tools.

## 🏆 Acknowledgments

- **Avalonia Team** - For the excellent cross-platform UI framework
- **Ollama** - For making local AI accessible
- **Original Contributors** - To the bash script version
- **BYU-Idaho CSE Department** - For the inspiration and use cases

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/JKaizenn/Intelligrade/issues)
- **Discussions**: [GitHub Discussions](https://github.com/JKaizenn/Intelligrade/discussions)
- **Documentation**: [Wiki](https://github.com/JKaizenn/Intelligrade/wiki)

---

**Made with ❤️ for educators who want to focus on teaching, not tedious grading.**

*Built with C# and Avalonia UI - Cross-platform, performant, and beautiful.*
