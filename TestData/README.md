# IntelliGrade Test Data

This directory contains sample rubrics and student code submissions for testing the IntelliGrade application.

## Directory Structure

```
TestData/
├── Rubrics/
│   ├── CSE232/
│   │   └── cpp/
│   │       ├── W06_BinaryTree.json
│   │       └── Sorting.json
│   └── CSE310/
│       └── cpp/
│           └── LinkedList.json
└── Submissions/
    ├── CSE232/
    │   └── cpp/
    │       ├── binary_tree_good.cpp
    │       ├── binary_tree_average.cpp
    │       ├── hash_table_good.cpp
    │       ├── sorting_good.cpp
    │       └── stack_queue.cs
    └── CSE310/
        ├── cpp/
        │   └── linked_list_excellent.cpp
        └── python/
            └── binary_search_tree.py
```

## Rubrics Format

Rubrics are stored in JSON format with the following structure:

```json
{
  "course": "CSE 232",
  "assignment": "W06 Lab: Binary Tree",
  "totalPoints": 115,
  "criteria": [
    {
      "name": "Code Quality",
      "maxPoints": 35,
      "ratings": [
        {
          "points": 35,
          "description": "The most elegant solution..."
        }
      ]
    }
  ]
}
```

## Organization Rules

1. **Rubrics**: `Rubrics/{CourseName}/{Language}/{AssignmentName}.json`
   - Example: `Rubrics/CSE232/cpp/W06_BinaryTree.json`

2. **Submissions**: `Submissions/{CourseName}/{Language}/{filename}.{ext}`
   - Example: `Submissions/CSE232/cpp/binary_tree_good.cpp`

3. **Languages**: Use lowercase for language folder names
   - `cpp` for C++
   - `python` for Python
   - `csharp` for C#
   - `java` for Java

## Sample Submissions

### CSE232 (C++)
- `binary_tree_good.cpp` - Excellent implementation with smart pointers
- `binary_tree_average.cpp` - Has memory leaks (for testing)
- `hash_table_good.cpp` - Hash table with resizing
- `sorting_good.cpp` - Three sorting algorithms with analysis
- `stack_queue.cs` - C# generic stack/queue (note: in cpp folder for testing)

### CSE310
- `linked_list_excellent.cpp` - Complete linked list with documentation
- `binary_search_tree.py` - Python BST with type hints
