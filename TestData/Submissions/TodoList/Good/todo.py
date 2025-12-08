"""
TodoList Application - GOOD VERSION
A simple command-line todo list manager with file persistence.
Features: Add, remove, display tasks, and save/load from file.
"""

import os
import sys

class TodoList:
    """Manages a list of tasks with file persistence."""

    def __init__(self, filename='tasks.txt'):
        """
        Initialize the TodoList.

        Args:
            filename: Name of the file to store tasks
        """
        self.filename = filename
        self.tasks = []
        self.load_tasks()

    def add_task(self, task):
        """
        Add a new task to the list.

        Args:
            task: Task description (non-empty string)

        Returns:
            True if successful, False otherwise
        """
        if not task or not task.strip():
            print("Error: Task cannot be empty")
            return False

        self.tasks.append(task.strip())
        print(f"Added task: '{task.strip()}'")
        return True

    def remove_task(self, index):
        """
        Remove a task by index.

        Args:
            index: Task number (1-based)

        Returns:
            True if successful, False otherwise
        """
        if 1 <= index <= len(self.tasks):
            removed = self.tasks.pop(index - 1)
            print(f"Removed task: '{removed}'")
            return True
        else:
            print(f"Error: Invalid task number {index}")
            return False

    def display_tasks(self):
        """Display all tasks with numbering."""
        if not self.tasks:
            print("\nNo tasks in your list!")
        else:
            print(f"\n=== Your Tasks ({len(self.tasks)}) ===")
            for i, task in enumerate(self.tasks, 1):
                print(f"{i}. {task}")
            print()

    def save_tasks(self):
        """
        Save tasks to file.

        Returns:
            True if successful, False otherwise
        """
        try:
            with open(self.filename, 'w') as f:
                for task in self.tasks:
                    f.write(task + '\n')
            print(f"Tasks saved to '{self.filename}'")
            return True
        except IOError as e:
            print(f"Error saving tasks: {e}")
            return False

    def load_tasks(self):
        """
        Load tasks from file.

        Returns:
            True if successful, False if file doesn't exist
        """
        if not os.path.exists(self.filename):
            print(f"No existing task file found. Starting fresh.")
            return False

        try:
            with open(self.filename, 'r') as f:
                self.tasks = [line.strip() for line in f if line.strip()]
            print(f"Loaded {len(self.tasks)} tasks from '{self.filename}'")
            return True
        except IOError as e:
            print(f"Error loading tasks: {e}")
            return False

def display_menu():
    """Display the main menu."""
    print("\n=== Todo List Menu ===")
    print("1. Add task")
    print("2. Remove task")
    print("3. Display tasks")
    print("4. Save and exit")
    print("5. Exit without saving")

def main():
    """Main program loop."""
    todo = TodoList()
    todo.display_tasks()

    while True:
        display_menu()
        choice = input("Enter choice (1-5): ").strip()

        if choice == '1':
            task = input("Enter task description: ")
            todo.add_task(task)

        elif choice == '2':
            todo.display_tasks()
            try:
                index = int(input("Enter task number to remove: "))
                todo.remove_task(index)
            except ValueError:
                print("Error: Please enter a valid number")

        elif choice == '3':
            todo.display_tasks()

        elif choice == '4':
            todo.save_tasks()
            print("Goodbye!")
            break

        elif choice == '5':
            print("Exiting without saving. Goodbye!")
            break

        else:
            print("Invalid choice. Please enter 1-5.")

if __name__ == "__main__":
    main()
