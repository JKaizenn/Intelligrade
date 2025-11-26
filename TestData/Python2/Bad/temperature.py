x=input("enter temp:")
y=input("1=ctof 2=ftoc:")
if y=="1":
 print(x*9/5+32)  # Bug: x is string, not converted to float
if y=="2":
 print(x-32*5/9)  # Bug: wrong formula (missing parentheses) and x is string
