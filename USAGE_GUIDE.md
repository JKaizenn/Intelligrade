# IntelliGrade Usage Guide

## Quick Start

### 1. Running the Application
```bash
cd /Users/jforbush/Dev/Intelligrade/Intelligrade
dotnet run --project src/IntelliGrade.App
```

### 2. Setting Up Test Files
Sample test files have been created at:
- `/Users/jforbush/Dev/Intelligrade/test_submissions/`

Sample rubrics have been created at:
- `~/bin/rubrics/CSE101/python/hello_world.txt`
- `~/bin/rubrics/CSE101/cpp/hello_world.txt`
- `~/bin/rubrics/CSE101/csharp/hello_world.txt`

## Using IntelliGrade

### Basic Workflow

1. **Select Working Directory**
   - Click "📁 Select Directory"
   - Navigate to `/Users/jforbush/Dev/Intelligrade/test_submissions`
   - Select the folder

2. **Detect Languages**
   - Click "🔍 Detect Languages"
   - Select the detected language (Python, C++, or C#)

3. **Select Course and Assignment**
   - Choose "CSE101" from the Course dropdown
   - Select source file from the Source File dropdown
   - Choose "hello_world" from the Assignment dropdown

4. **View Source Code**
   - Click the "📄 Source Code" tab to view the student's code

5. **Run the Program**
   - Click "▶️ Run Program"
   - View output in the "⚡ Program Output" tab

6. **View Rubric**
   - Click the "📋 Rubric" tab to see grading criteria

7. **Grade the Assignment**
   - Click the "✏️ Grading" tab
   - Enter Student Name and ID (optional)
   - Enter grade (0-100)
   - Letter grade calculates automatically using BYU-Idaho standard
   - Add instructor feedback

8. **Export Results**
   - Click "💾 Save as JSON" or "📊 Save as CSV"
   - Choose location and filename

### Optional: AI Analysis

If you have Ollama installed:
1. Install Ollama: https://ollama.ai
2. Pull the model: `ollama pull llama3.2:1b`
3. Click "🤖 Analyze with AI" to get automated feedback
4. View results in the "🤖 AI Analysis" tab

## BYU-Idaho Grading Scale

- A+: 97-100
- A: 93-96
- A-: 90-92
- B+: 87-89
- B: 83-86
- B-: 80-82
- C+: 77-79
- C: 73-76
- C-: 70-72
- D+: 67-69
- D: 63-66
- D-: 60-62
- F: Below 60

## Supported Languages

### Fully Tested
- **Python** (.py) - Uses `python3`
- **C++** (.cpp) - Compiles with `g++`, then runs
- **C#** (.cs) - Requires .NET SDK, uses `dotnet build`

### Also Supported
- JavaScript, Java, C, PHP, Ruby, Go, Rust

## Troubleshooting

### Program won't run
- Ensure you have the required compiler/interpreter installed
- For C++: Install g++ (`brew install gcc` on macOS)
- For Python: Python 3 should be pre-installed on macOS
- For C#: .NET SDK is required

### Rubrics not loading
- Check that `~/bin/rubrics` exists
- Verify the folder structure: `~/bin/rubrics/[Course]/[Language]/[Assignment].txt`

### AI not available
- Install Ollama: https://ollama.ai
- Run: `ollama pull llama3.2:1b`
- Ensure Ollama is running (it runs as a service after installation)
