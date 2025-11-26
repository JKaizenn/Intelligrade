using System;
using System.Collections.Generic;

// Todo item class
class TodoItem
{
    public string Text;
    public bool Done;
    
    public TodoItem(string text)
    {
        Text = text;
        Done = false;
    }
}

class Program
{
    static List<TodoItem> todos = new List<TodoItem>();
    
    static void Main()
    {
        Console.WriteLine("Todo List");
        
        while (true)
        {
            Console.WriteLine("\n1. Add");
            Console.WriteLine("2. List");
            Console.WriteLine("3. Complete");
            Console.WriteLine("4. Remove");
            Console.WriteLine("5. Exit");
            Console.Write("Choice: ");
            
            string choice = Console.ReadLine();
            
            if (choice == "1")
            {
                Console.Write("Enter todo: ");
                string text = Console.ReadLine();
                todos.Add(new TodoItem(text));
                Console.WriteLine("Added!");
            }
            else if (choice == "2")
            {
                Console.WriteLine("\nTodos:");
                for (int i = 0; i < todos.Count; i++)
                {
                    string status = todos[i].Done ? "[X]" : "[ ]";
                    Console.WriteLine($"{i + 1}. {status} {todos[i].Text}");
                }
            }
            else if (choice == "3")
            {
                Console.Write("Enter number to complete: ");
                int num = int.Parse(Console.ReadLine());
                todos[num - 1].Done = true;
                Console.WriteLine("Marked complete!");
            }
            else if (choice == "4")
            {
                Console.Write("Enter number to remove: ");
                int num = int.Parse(Console.ReadLine());
                todos.RemoveAt(num - 1);
                Console.WriteLine("Removed!");
            }
            else if (choice == "5")
            {
                break;
            }
        }
    }
}
