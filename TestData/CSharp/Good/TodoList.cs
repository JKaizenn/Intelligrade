/// <summary>
/// Todo List Application
/// CSE 210 - Programming with Classes
/// 
/// A console-based todo list manager demonstrating
/// object-oriented design principles in C#.
/// 
/// Author: Good Student
/// Date: 2025-01-15
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a single todo item with a description and completion status.
/// </summary>
public class TodoItem
{
    /// <summary>
    /// Gets or sets the todo description.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Gets or sets whether the todo is completed.
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Gets the creation date of the todo.
    /// </summary>
    public DateTime CreatedAt { get; }
    
    /// <summary>
    /// Creates a new todo item with the specified description.
    /// </summary>
    /// <param name="description">The todo description</param>
    /// <exception cref="ArgumentException">If description is empty</exception>
    public TodoItem(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be empty", nameof(description));
        }
        
        Description = description.Trim();
        IsCompleted = false;
        CreatedAt = DateTime.Now;
    }
    
    /// <summary>
    /// Returns a formatted string representation of the todo.
    /// </summary>
    public override string ToString()
    {
        string status = IsCompleted ? "[✓]" : "[ ]";
        return $"{status} {Description}";
    }
}

/// <summary>
/// Manages a collection of todo items with add, remove, and complete operations.
/// </summary>
public class TodoList
{
    private readonly List<TodoItem> _items;
    
    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    public int Count => _items.Count;
    
    /// <summary>
    /// Gets the number of completed items.
    /// </summary>
    public int CompletedCount => _items.Count(item => item.IsCompleted);
    
    /// <summary>
    /// Gets the number of pending items.
    /// </summary>
    public int PendingCount => _items.Count(item => !item.IsCompleted);
    
    /// <summary>
    /// Creates a new empty todo list.
    /// </summary>
    public TodoList()
    {
        _items = new List<TodoItem>();
    }
    
    /// <summary>
    /// Adds a new todo item to the list.
    /// </summary>
    /// <param name="description">The todo description</param>
    /// <returns>The created todo item</returns>
    public TodoItem Add(string description)
    {
        var item = new TodoItem(description);
        _items.Add(item);
        return item;
    }
    
    /// <summary>
    /// Removes a todo item by its index (1-based).
    /// </summary>
    /// <param name="index">The 1-based index of the item to remove</param>
    /// <returns>True if removed successfully</returns>
    public bool RemoveAt(int index)
    {
        if (index < 1 || index > _items.Count)
        {
            return false;
        }
        
        _items.RemoveAt(index - 1);
        return true;
    }
    
    /// <summary>
    /// Toggles the completion status of an item by index (1-based).
    /// </summary>
    /// <param name="index">The 1-based index of the item</param>
    /// <returns>True if toggled successfully</returns>
    public bool ToggleComplete(int index)
    {
        if (index < 1 || index > _items.Count)
        {
            return false;
        }
        
        _items[index - 1].IsCompleted = !_items[index - 1].IsCompleted;
        return true;
    }
    
    /// <summary>
    /// Gets all todo items.
    /// </summary>
    public IEnumerable<TodoItem> GetAll() => _items.AsReadOnly();
    
    /// <summary>
    /// Displays all todos with their indices.
    /// </summary>
    public void Display()
    {
        if (_items.Count == 0)
        {
            Console.WriteLine("  No todos yet. Add one to get started!");
            return;
        }
        
        for (int i = 0; i < _items.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {_items[i]}");
        }
        
        Console.WriteLine();
        Console.WriteLine($"  Total: {Count} | Completed: {CompletedCount} | Pending: {PendingCount}");
    }
}

/// <summary>
/// Main program class with console UI.
/// </summary>
public class Program
{
    private static readonly TodoList _todoList = new TodoList();
    
    public static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════╗");
        Console.WriteLine("║       Todo List Application        ║");
        Console.WriteLine("╚════════════════════════════════════╝");
        
        bool running = true;
        
        while (running)
        {
            DisplayMenu();
            
            Console.Write("\nSelect option: ");
            string? input = Console.ReadLine();
            
            Console.WriteLine();
            
            switch (input?.Trim())
            {
                case "1":
                    AddTodo();
                    break;
                case "2":
                    ListTodos();
                    break;
                case "3":
                    ToggleTodo();
                    break;
                case "4":
                    RemoveTodo();
                    break;
                case "5":
                    running = false;
                    Console.WriteLine("Goodbye! Stay productive! ✨");
                    break;
                default:
                    Console.WriteLine("⚠ Invalid option. Please enter 1-5.");
                    break;
            }
        }
    }
    
    private static void DisplayMenu()
    {
        Console.WriteLine("\n┌────────────────────────────────────┐");
        Console.WriteLine("│  1. Add new todo                   │");
        Console.WriteLine("│  2. List all todos                 │");
        Console.WriteLine("│  3. Toggle complete                │");
        Console.WriteLine("│  4. Remove todo                    │");
        Console.WriteLine("│  5. Exit                           │");
        Console.WriteLine("└────────────────────────────────────┘");
    }
    
    private static void AddTodo()
    {
        Console.Write("Enter todo description: ");
        string? description = Console.ReadLine();
        
        try
        {
            var item = _todoList.Add(description ?? "");
            Console.WriteLine($"✓ Added: {item.Description}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"⚠ Error: {ex.Message}");
        }
    }
    
    private static void ListTodos()
    {
        Console.WriteLine("\n── Your Todos ──────────────────────");
        _todoList.Display();
    }
    
    private static void ToggleTodo()
    {
        ListTodos();
        
        if (_todoList.Count == 0) return;
        
        Console.Write("\nEnter todo number to toggle: ");
        
        if (int.TryParse(Console.ReadLine(), out int index))
        {
            if (_todoList.ToggleComplete(index))
            {
                Console.WriteLine("✓ Todo status updated!");
            }
            else
            {
                Console.WriteLine("⚠ Invalid todo number.");
            }
        }
        else
        {
            Console.WriteLine("⚠ Please enter a valid number.");
        }
    }
    
    private static void RemoveTodo()
    {
        ListTodos();
        
        if (_todoList.Count == 0) return;
        
        Console.Write("\nEnter todo number to remove: ");
        
        if (int.TryParse(Console.ReadLine(), out int index))
        {
            if (_todoList.RemoveAt(index))
            {
                Console.WriteLine("✓ Todo removed!");
            }
            else
            {
                Console.WriteLine("⚠ Invalid todo number.");
            }
        }
        else
        {
            Console.WriteLine("⚠ Please enter a valid number.");
        }
    }
}
