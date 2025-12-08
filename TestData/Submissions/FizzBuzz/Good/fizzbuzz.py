# FizzBuzz - GOOD VERSION
# Prints numbers from 1-100 with special rules:
# - "Fizz" for multiples of 3
# - "Buzz" for multiples of 5
# - "FizzBuzz" for multiples of both (15)

def fizzbuzz(n):
    """
    Generate FizzBuzz sequence from 1 to n.

    Args:
        n: The upper limit (inclusive)
    """
    for number in range(1, n + 1):
        output = ""

        # Check divisibility by 3
        if number % 3 == 0:
            output += "Fizz"

        # Check divisibility by 5
        if number % 5 == 0:
            output += "Buzz"

        # If not divisible by 3 or 5, print the number
        if not output:
            output = str(number)

        print(output)

if __name__ == "__main__":
    fizzbuzz(100)
