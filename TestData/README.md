# IntelliGrade Test Suite

This test suite contains rubrics and sample code files for testing IntelliGrade's grading and AI analysis features.

## Directory Structure

```
TestSuite/
├── Rubrics/                    # JSON rubric files
│   ├── FizzBuzz.json          # Python - CSE 110
│   ├── TemperatureConverter.json  # Python - CSE 110
│   ├── ArrayStats.json        # JavaScript - WDD 230
│   ├── Rectangle.json         # C++ - CSE 232
│   ├── BankAccount.json       # Java - CSE 210
│   └── TodoList.json          # C# - CSE 210
│
├── Python/                     # FizzBuzz Assignment
│   ├── Good/fizzbuzz.py       # Expected: 95-100 points
│   ├── Medium/fizzbuzz.py     # Expected: 65-80 points
│   └── Bad/fizzbuzz.py        # Expected: 20-40 points
│
├── Python2/                    # Temperature Converter
│   ├── Good/temperature.py    # Expected: 95-100 points
│   ├── Medium/temperature.py  # Expected: 60-75 points
│   └── Bad/temperature.py     # Expected: 10-25 points
│
├── JavaScript/                 # Array Statistics
│   ├── Good/arrayStats.js     # Expected: 95-100 points
│   ├── Medium/arrayStats.js   # Expected: 55-70 points
│   └── Bad/arrayStats.js      # Expected: 15-35 points
│
├── Cpp/                        # Rectangle Class
│   ├── Good/rectangle.cpp     # Expected: 95-100 points
│   ├── Medium/rectangle.cpp   # Expected: 60-75 points
│   └── Bad/rectangle.cpp      # Expected: 20-35 points
│
├── Java/                       # Bank Account Class
│   ├── Good/BankAccount.java  # Expected: 95-100 points
│   ├── Medium/BankAccount.java # Expected: 60-75 points
│   └── Bad/BankAccount.java   # Expected: 15-30 points
│
└── CSharp/                     # Todo List Application
    ├── Good/TodoList.cs       # Expected: 95-100 points
    ├── Medium/TodoList.cs     # Expected: 55-70 points
    └── Bad/TodoList.cs        # Expected: 10-25 points
```

## Solution Quality Levels

### Good Solutions (95-100 points)
- Complete functionality meeting all requirements
- Excellent code structure and organization
- Comprehensive error handling and input validation
- Full documentation (docstrings, XML comments, JSDoc)
- Follows language conventions and best practices
- Clean, readable formatting

### Medium Solutions (55-75 points)
- Core functionality works
- Basic structure but missing some organization
- Limited or no error handling
- Minimal documentation
- Some style inconsistencies
- Works but not optimal

### Bad Solutions (10-35 points)
- Has bugs affecting core functionality
- Poor or no structure (no functions/classes where needed)
- No error handling (crashes on invalid input)
- No documentation
- Very poor naming and formatting
- May have logic errors or incorrect implementations

## Using the Test Suite

### 1. Copy Rubrics to IntelliGrade
Copy the JSON rubrics to your IntelliGrade rubrics directory:
```
~/bin/rubrics/{Course}/{Language}/
```

For example:
- `FizzBuzz.json` → `~/bin/rubrics/CSE_110/python/FizzBuzz.json`
- `Rectangle.json` → `~/bin/rubrics/CSE_232/cpp/Rectangle.json`

### 2. Test Each Quality Level
1. Open IntelliGrade
2. Select a test directory (e.g., `TestSuite/Python/Good/`)
3. Select the appropriate rubric
4. Run the program to verify it executes
5. Run AI analysis
6. Verify AI suggestions are reasonable for the quality level

### 3. Expected AI Behavior

| Quality | AI Should Recognize |
|---------|---------------------|
| **Good** | High scores (35-40/40 on functionality), praise for documentation, clean code, good structure |
| **Medium** | Mid-range scores, note missing documentation, suggest improvements for error handling |
| **Bad** | Low scores, identify bugs, flag poor naming, mention missing validation, note style issues |

## Common Issues to Test

### Python Files
- **Good**: Check AI recognizes docstrings, type hints, proper structure
- **Medium**: Check AI notes missing docstrings, suggests improvements
- **Bad**: Check AI identifies logic bugs (FizzBuzz order), poor naming

### JavaScript Files
- **Good**: Check AI recognizes ES6+ features, JSDoc, error handling
- **Medium**: Check AI suggests using modern array methods
- **Bad**: Check AI identifies off-by-one error, var usage, poor naming

### C++ Files
- **Good**: Check AI recognizes encapsulation, validation, documentation
- **Medium**: Check AI notes missing validation, const correctness
- **Bad**: Check AI identifies calculation bugs, public members, no validation

### Java Files
- **Good**: Check AI recognizes proper OOP, JavaDoc, validation
- **Medium**: Check AI notes missing validation for negative deposits
- **Bad**: Check AI identifies public fields, overdraft bug, no encapsulation

### C# Files
- **Good**: Check AI recognizes proper class design, XML docs, LINQ usage
- **Medium**: Check AI notes missing input validation, basic structure
- **Bad**: Check AI identifies array vs List issue, null bugs, crashes

## Rubric Criteria Summary

| Assignment | Criteria |
|------------|----------|
| FizzBuzz | Output correctness, Code structure, Style, Documentation |
| Temperature | Conversions, UI, Validation, Organization |
| Array Stats | Functionality, Modern JS, Error handling, Code quality |
| Rectangle | Class structure, Methods, Validation, Code quality |
| Bank Account | Class design, Core methods, Validation, Code quality |
| Todo List | Core functionality, OOP design, UI, Code quality |

## AI Confidence Testing

For each solution, note the AI confidence level:
- **Good solutions**: Should show HIGH confidence
- **Medium solutions**: Should show MEDIUM confidence  
- **Bad solutions**: Should show LOW to MEDIUM confidence (may be uncertain due to bugs)

## Extending the Test Suite

To add more test cases:

1. Create a new rubric JSON in `Rubrics/`
2. Create `Good/`, `Medium/`, `Bad/` subdirectories
3. Implement three versions of the solution
4. Document expected scores and issues in this README

## Notes

- All code files are self-contained and can be run independently
- Java files must be named to match their public class
- C# files can be run with `dotnet run` or compiled with `csc`
- C++ files require a C++ compiler (g++, clang++)
- JavaScript files can be run with Node.js
