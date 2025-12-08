# MEDIUM VERSION - has functions and file I/O, but minimal error handling
def add_task(tasks, task):
    tasks.append(task)
    print(f"Added: {task}")

def display_tasks(tasks):
    if not tasks:
        print("No tasks!")
    else:
        for i, task in enumerate(tasks, 1):
            print(f"{i}. {task}")

def save_tasks(tasks, filename):
    with open(filename, 'w') as f:
        for task in tasks:
            f.write(task + '\n')

def load_tasks(filename):
    tasks = []
    try:
        with open(filename, 'r') as f:
            tasks = [line.strip() for line in f]
    except FileNotFoundError:
        pass
    return tasks

# Main program
tasks = load_tasks('tasks.txt')
add_task(tasks, "Buy groceries")
add_task(tasks, "Finish homework")
display_tasks(tasks)
save_tasks(tasks, 'tasks.txt')
