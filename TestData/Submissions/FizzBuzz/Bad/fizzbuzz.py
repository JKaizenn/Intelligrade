# FizzBuzz - BAD VERSION
# Issues: Wrong range, incorrect logic order, no comments, poor structure

for i in range(1, 101):
    if i % 3 == 0:
        print("Fizz")
    if i % 5 == 0:
        print("Buzz")
    if i % 3 != 0 and i % 5 != 0:
        print(i)
