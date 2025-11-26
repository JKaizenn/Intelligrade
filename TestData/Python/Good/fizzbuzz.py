"""
FizzBuzz Implementation
CSE 110 - Introduction to Programming

This program prints numbers from 1 to 100, but:
- For multiples of 3, prints "Fizz" instead of the number
- For multiples of 5, prints "Buzz" instead of the number
- For multiples of both 3 and 5, prints "FizzBuzz"

Author: Good Student
Date: 2025-01-15
"""


def fizzbuzz(n: int) -> str:
    """
    Determine the FizzBuzz output for a given number.
    
    Args:
        n: The number to evaluate
        
    Returns:
        'FizzBuzz' if divisible by both 3 and 5,
        'Fizz' if divisible by 3 only,
        'Buzz' if divisible by 5 only,
        otherwise the number as a string
    """
    if n % 15 == 0:  # Check divisible by both first (3 * 5 = 15)
        return "FizzBuzz"
    elif n % 3 == 0:
        return "Fizz"
    elif n % 5 == 0:
        return "Buzz"
    else:
        return str(n)


def main():
    """Run FizzBuzz for numbers 1 through 100."""
    for number in range(1, 101):
        print(fizzbuzz(number))


if __name__ == "__main__":
    main()
