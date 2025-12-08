# IntelliGrade Test Suite

This directory contains test programs of varying quality levels to validate the IntelliGrade grading system.

## Test Structure

Each assignment has three quality levels:
- **Bad**: Minimal implementation, missing key requirements, poor code quality
- **Medium**: Meets basic requirements but lacks polish, documentation, or error handling
- **Good**: Complete implementation with proper structure, documentation, and error handling

## Assignments

### 1. FizzBuzz
**Languages**: Python, Java
**Rubric**: TestData/Rubrics/FizzBuzz.json (100 points total)

Tests basic programming logic and control flow.

| Version | Expected Grade Range | Key Issues/Features |
|---------|---------------------|---------------------|
| Bad (Python) | 20-40% | Wrong logic order, prints "Fizz" and "Buzz" separately instead of "FizzBuzz" |
| Medium (Python) | 60-75% | Works correctly but redundant %15 check, minimal comments |
| Good (Python) | 90-100% | Clean function-based design, proper documentation, efficient logic |
| Bad (Java) | 25-40% | Messy one-liner with ternary operators, no structure |
| Medium (Java) | 65-75% | Correct logic but minimal comments, basic structure |
| Good (Java) | 90-100% | Constants, helper methods, comprehensive documentation |

**Files**:
- FizzBuzz/Bad/fizzbuzz.py
- FizzBuzz/Medium/fizzbuzz.py
- FizzBuzz/Good/fizzbuzz.py
- FizzBuzz/Bad/FizzBuzz.java
- FizzBuzz/Medium/FizzBuzz.java
- FizzBuzz/Good/FizzBuzz.java

---

### 2. Calculator
**Languages**: C++ (multi-file), Java (multi-file)
**Rubric**: TestData/Rubrics/Calculator.json (100 points total)

Tests object-oriented programming, multi-file projects, and error handling.

| Version | Expected Grade Range | Key Issues/Features |
|---------|---------------------|---------------------|
| Bad (C++) | 15-30% | Single file, no validation, allows division by zero |
| Medium (C++) | 55-70% | Has functions but single file, basic validation |
| Good (C++) | 90-100% | Proper OOP with .h/.cpp separation, full error handling, menu system |
| Good (Java) | 90-100% | Clean OOP design, multi-file, exception handling |

**Files**:
- Calculator/Bad/main.cpp (single file)
- Calculator/Medium/main.cpp (single file)
- Calculator/Good/Calculator.h, Calculator.cpp, main.cpp (multi-file)
- Calculator/Good/Calculator.java, CalculatorMain.java (multi-file Java)

**Multi-file Compilation Tests**: The Good versions test the multi-file compilation feature by splitting the program into separate header/source/main files.

---

### 3. StudentGrades
**Languages**: C (multi-file)
**Rubric**: TestData/Rubrics/StudentGrades.json (100 points total)

Tests array handling, sorting algorithms, and statistics calculations.

| Version | Expected Grade Range | Key Issues/Features |
|---------|---------------------|---------------------|
| Bad (C) | 20-35% | Hardcoded array, no functions, prints unsorted |
| Medium (C) | 60-75% | Has functions but single file, basic bubble sort |
| Good (C) | 90-100% | Multi-file design with .h/.c separation, selection sort, full stats |

**Files**:
- StudentGrades/Bad/grades.c (single file)
- StudentGrades/Medium/grades.c (single file)
- StudentGrades/Good/grades.h, grades.c, main.c (multi-file)

**Multi-file Compilation Tests**: The Good version tests C multi-file compilation with proper header/source separation.

---

### 4. BankAccount
**Languages**: C++ (multi-file)
**Rubric**: TestData/Rubrics/BankAccount.json (100 points total)

Tests object-oriented design, encapsulation, and data validation.

| Version | Expected Grade Range | Key Issues/Features |
|---------|---------------------|---------------------|
| Bad (C++) | 10-25% | No validation, allows negative balance, no OOP |
| Medium (C++) | 55-70% | Basic class with validation but single file |
| Good (C++) | 90-100% | Full OOP with .h/.cpp separation, comprehensive validation and documentation |

**Files**:
- BankAccount/Bad/account.cpp (single file, no OOP)
- BankAccount/Medium/account.cpp (single file with class)
- BankAccount/Good/BankAccount.h, BankAccount.cpp, main.cpp (multi-file OOP)

**Multi-file Compilation Tests**: The Good version tests C++ class separation across header and implementation files.

---

### 5. TodoList
**Languages**: Python
**Rubric**: TestData/Rubrics/TodoList.json (100 points total)

Tests file I/O, user interaction, and program structure.

| Version | Expected Grade Range | Key Issues/Features |
|---------|---------------------|---------------------|
| Bad (Python) | 15-30% | No file I/O, no functions, hardcoded tasks |
| Medium (Python) | 60-75% | Has functions and file I/O but minimal error handling |
| Good (Python) | 90-100% | Class-based design, comprehensive error handling, menu system, full documentation |

**Files**:
- TodoList/Bad/todo.py
- TodoList/Medium/todo.py
- TodoList/Good/todo.py

---

## Testing the Grading System

### Expected Behavior

1. **Language Detection**: Each program should be correctly identified by its file extension
2. **Compilation**: Multi-file projects (Calculator, StudentGrades, BankAccount Good versions) should compile all source files together
3. **Execution**: Programs should run successfully (note: interactive programs may require input handling)
4. **AI Grading**: AI should identify quality differences and assign appropriate grades based on rubrics

### Validation Steps

1. Load each rubric from TestData/Rubrics/
2. Grade each program version (Bad/Medium/Good)
3. Verify grade ranges match expected ranges in tables above
4. Check that multi-file projects compile successfully
5. Verify AI feedback identifies specific issues (e.g., "no error handling", "missing documentation")

### Known Limitations

- **Interactive Programs**: Some programs (Calculator Good, TodoList Good) have interactive menus that may not run fully in automated testing
- **File I/O**: TodoList programs create/modify files in their directory
- **Platform Dependencies**: C/C++ compilation requires g++/gcc, Java requires JDK

## Directory Structure

```
TestData/
├── Rubrics/
│   ├── FizzBuzz.json
│   ├── Calculator.json
│   ├── StudentGrades.json
│   ├── BankAccount.json
│   └── TodoList.json
└── Submissions/
    ├── FizzBuzz/
    │   ├── Bad/
    │   ├── Medium/
    │   └── Good/
    ├── Calculator/
    │   ├── Bad/
    │   ├── Medium/
    │   └── Good/
    ├── StudentGrades/
    │   ├── Bad/
    │   ├── Medium/
    │   └── Good/
    ├── BankAccount/
    │   ├── Bad/
    │   ├── Medium/
    │   └── Good/
    └── TodoList/
        ├── Bad/
        ├── Medium/
        └── Good/
```

## Quality Criteria Summary

### Bad Programs (15-40% expected)
- Minimal or incorrect functionality
- No error handling or validation
- Poor or no code structure
- Missing documentation
- May have critical bugs

### Medium Programs (55-75% expected)
- Basic functionality works
- Minimal error handling
- Some structure (functions or basic OOP)
- Limited documentation
- Single-file implementation

### Good Programs (90-100% expected)
- Complete, correct functionality
- Comprehensive error handling
- Proper structure (OOP, multi-file for compiled languages)
- Full documentation (comments, docstrings, XML docs)
- Professional code quality
- User-friendly interface where applicable
