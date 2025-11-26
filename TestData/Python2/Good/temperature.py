"""
Temperature Converter
CSE 110 - Introduction to Programming

A menu-driven program that converts temperatures between
Celsius, Fahrenheit, and Kelvin scales.

Author: Good Student
Date: 2025-01-15
"""

# Constants
ABSOLUTE_ZERO_CELSIUS = -273.15
ABSOLUTE_ZERO_FAHRENHEIT = -459.67


def celsius_to_fahrenheit(celsius: float) -> float:
    """
    Convert Celsius to Fahrenheit.
    
    Formula: F = (C × 9/5) + 32
    
    Args:
        celsius: Temperature in Celsius
        
    Returns:
        Temperature in Fahrenheit
    """
    return (celsius * 9/5) + 32


def fahrenheit_to_celsius(fahrenheit: float) -> float:
    """
    Convert Fahrenheit to Celsius.
    
    Formula: C = (F - 32) × 5/9
    
    Args:
        fahrenheit: Temperature in Fahrenheit
        
    Returns:
        Temperature in Celsius
    """
    return (fahrenheit - 32) * 5/9


def celsius_to_kelvin(celsius: float) -> float:
    """
    Convert Celsius to Kelvin.
    
    Formula: K = C + 273.15
    
    Args:
        celsius: Temperature in Celsius
        
    Returns:
        Temperature in Kelvin
    """
    return celsius + 273.15


def kelvin_to_celsius(kelvin: float) -> float:
    """
    Convert Kelvin to Celsius.
    
    Formula: C = K - 273.15
    
    Args:
        kelvin: Temperature in Kelvin
        
    Returns:
        Temperature in Celsius
    """
    return kelvin - 273.15


def display_menu() -> None:
    """Display the conversion menu options."""
    print("\n╔════════════════════════════════════╗")
    print("║     Temperature Converter          ║")
    print("╠════════════════════════════════════╣")
    print("║  1. Celsius to Fahrenheit          ║")
    print("║  2. Fahrenheit to Celsius          ║")
    print("║  3. Celsius to Kelvin              ║")
    print("║  4. Kelvin to Celsius              ║")
    print("║  5. Exit                           ║")
    print("╚════════════════════════════════════╝")


def get_temperature(unit: str, min_value: float) -> float:
    """
    Get a valid temperature from the user.
    
    Args:
        unit: The temperature unit name for the prompt
        min_value: The minimum allowed value (absolute zero)
        
    Returns:
        A valid temperature value
        
    Raises:
        ValueError: If input cannot be converted to float
    """
    while True:
        try:
            temp = float(input(f"Enter temperature in {unit}: "))
            if temp < min_value:
                print(f"Error: Temperature cannot be below {min_value:.2f}° {unit}")
                continue
            return temp
        except ValueError:
            print("Error: Please enter a valid number")


def format_result(value: float, from_unit: str, to_unit: str, result: float) -> str:
    """Format the conversion result for display."""
    return f"\n✓ {value:.2f}° {from_unit} = {result:.2f}° {to_unit}"


def main():
    """Main program loop."""
    print("Welcome to the Temperature Converter!")
    
    while True:
        display_menu()
        
        try:
            choice = input("\nSelect an option (1-5): ").strip()
        except EOFError:
            break
            
        if choice == "1":
            temp = get_temperature("Celsius", ABSOLUTE_ZERO_CELSIUS)
            result = celsius_to_fahrenheit(temp)
            print(format_result(temp, "C", "F", result))
            
        elif choice == "2":
            temp = get_temperature("Fahrenheit", ABSOLUTE_ZERO_FAHRENHEIT)
            result = fahrenheit_to_celsius(temp)
            print(format_result(temp, "F", "C", result))
            
        elif choice == "3":
            temp = get_temperature("Celsius", ABSOLUTE_ZERO_CELSIUS)
            result = celsius_to_kelvin(temp)
            print(format_result(temp, "C", "K", result))
            
        elif choice == "4":
            temp = get_temperature("Kelvin", 0)
            result = kelvin_to_celsius(temp)
            print(format_result(temp, "K", "C", result))
            
        elif choice == "5":
            print("\nThank you for using Temperature Converter. Goodbye!")
            break
            
        else:
            print("\n⚠ Invalid option. Please enter 1-5.")


if __name__ == "__main__":
    main()
