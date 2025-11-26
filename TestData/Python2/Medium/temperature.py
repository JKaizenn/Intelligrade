# Temperature Converter

def c_to_f(c):
    return (c * 9/5) + 32

def f_to_c(f):
    return (f - 32) * 5/9

def c_to_k(c):
    return c + 273.15

def k_to_c(k):
    return k - 273.15

print("Temperature Converter")
print("1. Celsius to Fahrenheit")
print("2. Fahrenheit to Celsius")  
print("3. Celsius to Kelvin")
print("4. Kelvin to Celsius")
print("5. Exit")

while True:
    choice = input("Select option: ")
    
    if choice == "1":
        temp = float(input("Enter Celsius: "))
        print(f"Result: {c_to_f(temp)} F")
    elif choice == "2":
        temp = float(input("Enter Fahrenheit: "))
        print(f"Result: {f_to_c(temp)} C")
    elif choice == "3":
        temp = float(input("Enter Celsius: "))
        print(f"Result: {c_to_k(temp)} K")
    elif choice == "4":
        temp = float(input("Enter Kelvin: "))
        print(f"Result: {k_to_c(temp)} C")
    elif choice == "5":
        print("Goodbye")
        break
    else:
        print("Invalid choice")
