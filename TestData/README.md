# IntelliGrade Test Data

This directory contains test programs and rubrics for demonstrating and testing IntelliGrade's features across multiple programming languages.

## Directory Structure

```
TestData/
├── Rubrics/              # Rubrics organized by course and language
│   ├── CS101/            # Introductory programming course
│   │   ├── python/       # Python rubrics (FizzBuzz, TodoList)
│   │   ├── java/         # Java rubrics (FizzBuzz)
│   │   ├── javascript/   # JavaScript rubrics (HelloWorld)
│   │   ├── csharp/       # C# rubrics (HelloWorld)
│   │   ├── rust/         # Rust rubrics (HelloWorld)
│   │   ├── go/           # Go rubrics (HelloWorld)
│   │   └── ruby/         # Ruby rubrics (HelloWorld)
│   └── CS201/            # Advanced programming course
│       ├── cpp/          # C++ rubrics (Calculator, BankAccount)
│       ├── c/            # C rubrics (StudentGrades)
│       └── java/         # Java rubrics (Calculator)
└── Submissions/          # Sample student submissions
    ├── HelloWorld/       # Simple Hello World programs (all languages)
    │   ├── Good/         # High-quality submission
    │   ├── Medium/       # Average-quality submission
    │   └── Bad/          # Low-quality submission
    ├── FizzBuzz/         # FizzBuzz programs (Python, Java)
    ├── Calculator/       # Calculator programs (C++, Java)
    ├── TodoList/         # To-do list programs (Python)
    ├── BankAccount/      # Bank account programs (C++ multi-file)
    └── StudentGrades/    # Student grades calculator (C)
```

## Important Note

**Some test programs may require further testing and debugging.** If you encounter issues with any test program:

- **Report bugs or issues**: Please open an issue on GitHub at [https://github.com/anthropics/intelligrade/issues](https://github.com/anthropics/intelligrade/issues)
- Include your operating system, the specific test program, and error messages
- Help us improve the test suite for all users!

## Using the Test Data

### Setting Up Rubrics

1. **Copy rubrics to your rubric directory:**
   ```bash
   cp -r TestData/Rubrics/* ~/bin/rubrics/
   ```

2. **Directory structure requirement:**
   - Rubrics must be organized as: `{Course}/{Language}/{Assignment}.json`
   - Example: `CS101/python/FizzBuzz.json`

### Testing Different Languages

The test data includes programs in 9+ languages:

| Language | Test Programs | Features |
|----------|---------------|----------|
| **Python** | FizzBuzz, TodoList | Interactive stdin, functions |
| **C++** | Calculator, BankAccount | Multi-file projects, classes, compilation |
| **Java** | Calculator, FizzBuzz | Compilation, OOP |
| **C** | StudentGrades | Header files, compilation |
| **JavaScript** | HelloWorld | Node.js execution |
| **C#** | HelloWorld | .NET compilation |
| **Rust** | HelloWorld | Rust compiler |
| **Go** | HelloWorld | Go execution |
| **Ruby** | HelloWorld | Ruby interpreter |

### Testing Quality Levels

Each assignment has three quality levels to test AI analysis:

- **Good**: High-quality code with documentation, proper structure, and best practices
- **Medium**: Working code with some documentation but room for improvement
- **Bad**: Minimal code, poor or no documentation, doesn't follow conventions

## Features Tested

### 1. Multi-Language Support
Test programs in Python, C++, Java, C, JavaScript, C#, Rust, Go, and Ruby.

### 2. Interactive Terminal
Programs that require stdin input:
- **TodoList** (Python): Menu-driven interactive program
- **BankAccount** (C++): Account operations with user prompts
- **StudentGrades** (C): Input student names and scores

Test these using the Interactive Terminal feature in IntelliGrade.

### 3. Multi-File Projects
The BankAccount C++ project demonstrates multi-file compilation:
```
BankAccount/Good/
├── BankAccount.h      # Class declaration
├── BankAccount.cpp    # Class implementation
└── main.cpp           # Main program
```

IntelliGrade automatically detects and compiles all necessary files.

### 4. Compilation Workflows
Test compile-then-run for:
- **C++**: g++ compiler
- **C**: gcc compiler
- **Java**: javac + java
- **C#**: dotnet/csc compiler
- **Rust**: rustc compiler
- **Go**: go run

### 5. AI Grading Modes
Test different analysis modes:
- **Quick**: Fast analysis with basic feedback
- **Balanced**: Moderate depth analysis (default)
- **Thorough**: Comprehensive detailed analysis

## Rubric Format

All rubrics follow the JSON format:

```json
{
  "name": "Assignment Name",
  "course": "CS101",
  "language": "Python",
  "totalPoints": 100,
  "criteria": [
    {
      "name": "Functionality",
      "maxPoints": 40,
      "description": "Program works correctly",
      "levels": [
        {
          "label": "Complete",
          "points": 40,
          "description": "All features work perfectly"
        },
        {
          "label": "Partial",
          "points": 20,
          "description": "Some features missing"
        },
        {
          "label": "Minimal",
          "points": 0,
          "description": "Does not work"
        }
      ]
    }
  ]
}
```

## Testing Workflow

1. **Select Working Directory**: Point IntelliGrade to a submission folder
   ```
   Example: TestData/Submissions/Calculator/Good
   ```

2. **Select Course, Language, and Assignment**:
   - Course: CS201
   - Language: cpp
   - Assignment: Calculator

3. **Run the Program**: Test execution in the Terminal tab
   - For compiled languages, IntelliGrade compiles automatically
   - For interpreted languages, runs directly

4. **Test Interactive Programs**: Use stdin in the Interactive Terminal tab
   - Type input when program prompts
   - Press Enter to send

5. **Get AI Analysis**: Use AI analysis to grade the submission
   - Choose analysis mode (Quick/Balanced/Thorough)
   - Review AI suggestions and scores

6. **Compare Quality Levels**: Test Good/Medium/Bad to see how AI adapts
   - Good: Should receive high scores, praise for quality
   - Medium: Should receive mid-range scores, suggestions for improvement
   - Bad: Should receive low scores, identification of issues

## Language-Specific Notes

### Python
- Runs directly with Python 3
- No compilation needed
- Test interactive programs in Terminal tab

### C++
- Uses g++ compiler
- Supports header files (.h) and implementation files (.cpp)
- Automatically includes all .cpp files in directory during compilation
- Creates executable in same directory

### Java
- Compiles all .java files in directory
- Main class should match primary filename
- Uses javac for compilation, java for execution

### C
- Uses gcc compiler
- Can have multiple .c and .h files
- Header includes are detected automatically

### C#
- Uses dotnet/csc compiler
- Requires .NET SDK installed
- Compiles to executable

### Rust
- Uses rustc compiler
- Compiles to native executable
- May take longer to compile (normal for Rust)

### Go
- Uses `go run` for execution
- Requires package main declaration
- Fast compilation and execution

### Ruby
- Runs directly with ruby interpreter
- No compilation needed
- Uses `puts` for output

### JavaScript/Node.js
- Runs with Node.js (node command)
- No compilation needed
- Uses console.log for output
- Requires Node.js installed

## Troubleshooting

### Program Won't Compile
- **C++/C**: Check for syntax errors, missing headers
- **Java**: Ensure class name matches filename
- **C#**: Verify .NET SDK is installed (`dotnet --version`)
- **Rust**: First compile may be slow; check error messages
- **Go**: Verify Go is installed (`go version`)

### Program Won't Run
- Check that the source file has the correct extension
- For compiled languages, check compilation output for errors
- Verify working directory is set correctly
- For scripting languages, verify interpreter is installed

### Rubric Not Found
- Ensure rubrics are in correct directory structure: `{Course}/{Language}/{Assignment}.json`
- Check that course name and language match exactly (case-sensitive)
- Verify rubric JSON is valid (use a JSON validator)

### AI Analysis Fails
- Verify Ollama is running: `ollama list`
- Check that the model is downloaded and available locally
- Ensure source code was loaded properly (check Source Code tab)
- Try a different analysis mode if one fails

### Interactive Programs Don't Accept Input
- Use the **Interactive Terminal** tab, not the Output tab
- Type input and press Enter to send
- Some programs may need specific input formats

## Creating Your Own Test Data

### Adding a New Assignment

1. **Create the rubric**:
   ```bash
   # Create rubric JSON file
   TestData/Rubrics/{Course}/{Language}/{Assignment}.json
   ```

2. **Create test submissions**:
   ```bash
   # Create submission folders
   mkdir -p TestData/Submissions/{Assignment}/{Good,Medium,Bad}
   # Add source files to each folder
   ```

3. **Test thoroughly**: Run all quality levels and verify:
   - Code compiles/runs correctly
   - AI analysis produces reasonable suggestions
   - Rubric criteria align with code quality

### Rubric Best Practices

- **Total Points**: Use 100 for easy percentage calculation
- **Criteria**: Include 3-5 criteria covering:
  - Functionality (35-40 points)
  - Code Quality/Structure (25-30 points)
  - Documentation (15-20 points)
  - Best Practices (10-15 points)
- **Levels**: Use 3-4 levels per criterion (Complete/Good/Partial/None)
- **Descriptions**: Be specific about requirements for each level

## Expected AI Behavior

| Quality | Expected AI Analysis |
|---------|---------------------|
| **Good** | High scores (90-100), recognizes documentation and best practices, praises clean code |
| **Medium** | Mid scores (60-80), notes missing documentation, suggests improvements |
| **Bad** | Low scores (0-50), identifies bugs and poor practices, flags missing error handling |

## Contributing

To expand the test suite:

1. Fork the repository
2. Add new rubrics and test programs
3. Test thoroughly with IntelliGrade
4. Document expected behavior
5. Submit a pull request
6. Report any issues via GitHub Issues

## Support

For questions, bugs, or feature requests:
- **GitHub Issues**: https://github.com/anthropics/intelligrade/issues
- **Documentation**: See main README.md in project root
- **Installation Help**: Check Installation section in main README

---

**Note**: This is test data for demonstration and development purposes. Some programs may exhibit issues or unexpected behavior. Please report any problems you encounter to help improve the test suite!

Built for testing IntelliGrade's grading capabilities across multiple languages and code quality levels.
