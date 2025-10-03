/**
 * Stack and Queue Implementations
 * Student: Emily Johnson
 * CSE 232 - Data Structures
 *
 * Generic stack and queue implementations using linked lists.
 * Includes comprehensive error handling and LINQ integration.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures
{
    /// <summary>
    /// Generic stack implementation using a singly linked list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack</typeparam>
    public class Stack<T> : IEnumerable<T>
    {
        private class Node
        {
            public T Data { get; set; }
            public Node? Next { get; set; }

            public Node(T data)
            {
                Data = data;
                Next = null;
            }
        }

        private Node? _top;
        private int _count;

        public int Count => _count;
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Pushes an item onto the stack.
        /// </summary>
        public void Push(T item)
        {
            var newNode = new Node(item)
            {
                Next = _top
            };
            _top = newNode;
            _count++;
        }

        /// <summary>
        /// Removes and returns the top item from the stack.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when stack is empty</exception>
        public T Pop()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Cannot pop from empty stack");

            var data = _top!.Data;
            _top = _top.Next;
            _count--;
            return data;
        }

        /// <summary>
        /// Returns the top item without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when stack is empty</exception>
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Cannot peek empty stack");

            return _top!.Data;
        }

        /// <summary>
        /// Clears all items from the stack.
        /// </summary>
        public void Clear()
        {
            _top = null;
            _count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = _top;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Generic queue implementation using a singly linked list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queue</typeparam>
    public class Queue<T> : IEnumerable<T>
    {
        private class Node
        {
            public T Data { get; set; }
            public Node? Next { get; set; }

            public Node(T data)
            {
                Data = data;
                Next = null;
            }
        }

        private Node? _front;
        private Node? _rear;
        private int _count;

        public int Count => _count;
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Adds an item to the rear of the queue.
        /// </summary>
        public void Enqueue(T item)
        {
            var newNode = new Node(item);

            if (IsEmpty)
            {
                _front = _rear = newNode;
            }
            else
            {
                _rear!.Next = newNode;
                _rear = newNode;
            }

            _count++;
        }

        /// <summary>
        /// Removes and returns the front item from the queue.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when queue is empty</exception>
        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Cannot dequeue from empty queue");

            var data = _front!.Data;
            _front = _front.Next;

            if (_front == null)
                _rear = null;

            _count--;
            return data;
        }

        /// <summary>
        /// Returns the front item without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when queue is empty</exception>
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Cannot peek empty queue");

            return _front!.Data;
        }

        /// <summary>
        /// Clears all items from the queue.
        /// </summary>
        public void Clear()
        {
            _front = _rear = null;
            _count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = _front;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Stack Tests ===");
            TestStack();

            Console.WriteLine("\n=== Queue Tests ===");
            TestQueue();

            Console.WriteLine("\n=== Edge Case Tests ===");
            TestEdgeCases();
        }

        static void TestStack()
        {
            var stack = new Stack<int>();

            // Test push
            Console.WriteLine("Pushing 10, 20, 30...");
            stack.Push(10);
            stack.Push(20);
            stack.Push(30);

            Console.WriteLine($"Stack count: {stack.Count}");
            Console.WriteLine($"Stack contents: {string.Join(", ", stack)}");

            // Test peek
            Console.WriteLine($"Peek: {stack.Peek()}");

            // Test pop
            Console.WriteLine($"Pop: {stack.Pop()}");
            Console.WriteLine($"Pop: {stack.Pop()}");
            Console.WriteLine($"Stack count after pops: {stack.Count}");

            // Test LINQ
            stack.Push(40);
            stack.Push(50);
            Console.WriteLine($"Max value in stack: {stack.Max()}");
            Console.WriteLine($"Sum of stack: {stack.Sum()}");

            // Test clear
            stack.Clear();
            Console.WriteLine($"Stack count after clear: {stack.Count}");
        }

        static void TestQueue()
        {
            var queue = new Queue<string>();

            // Test enqueue
            Console.WriteLine("Enqueueing Alice, Bob, Charlie...");
            queue.Enqueue("Alice");
            queue.Enqueue("Bob");
            queue.Enqueue("Charlie");

            Console.WriteLine($"Queue count: {queue.Count}");
            Console.WriteLine($"Queue contents: {string.Join(", ", queue)}");

            // Test peek
            Console.WriteLine($"Peek: {queue.Peek()}");

            // Test dequeue
            Console.WriteLine($"Dequeue: {queue.Dequeue()}");
            Console.WriteLine($"Dequeue: {queue.Dequeue()}");
            Console.WriteLine($"Queue count after dequeues: {queue.Count}");

            // Test LINQ
            queue.Enqueue("David");
            queue.Enqueue("Eve");
            Console.WriteLine($"Queue contains Eve: {queue.Contains("Eve")}");
            Console.WriteLine($"First person: {queue.First()}");

            // Test clear
            queue.Clear();
            Console.WriteLine($"Queue count after clear: {queue.Count}");
        }

        static void TestEdgeCases()
        {
            var stack = new Stack<int>();
            var queue = new Queue<int>();

            // Test empty conditions
            Console.WriteLine($"Empty stack: {stack.IsEmpty}");
            Console.WriteLine($"Empty queue: {queue.IsEmpty}");

            // Test exceptions
            try
            {
                stack.Pop();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Expected exception: {e.Message}");
            }

            try
            {
                queue.Dequeue();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Expected exception: {e.Message}");
            }

            // Test single element
            stack.Push(100);
            Console.WriteLine($"Single element stack peek: {stack.Peek()}");
            Console.WriteLine($"Single element stack pop: {stack.Pop()}");
            Console.WriteLine($"Stack empty after pop: {stack.IsEmpty}");
        }
    }
}
