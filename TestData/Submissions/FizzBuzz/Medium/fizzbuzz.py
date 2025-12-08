# FizzBuzz - MEDIUM VERSION
# Issues: Works but not optimal - checks 15 separately which is redundant

# Loop from 1 to 100
for num in range(1, 101):
    if num % 15 == 0:
        print("FizzBuzz")
    elif num % 3 == 0:
        print("Fizz")
    elif num % 5 == 0:
        print("Buzz")
    else:
        print(num)
