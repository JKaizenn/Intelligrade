/**
 * Simple Calculator Program
 * Student: Jordan Smith
 * CSE 232 - Introduction to Programming
 *
 * A basic calculator that performs arithmetic operations
 */

#include <stdio.h>
#include <stdlib.h>

/**
 * Add two numbers
 */
double add(double a, double b) {
    return a + b;
}

/**
 * Subtract two numbers
 */
double subtract(double a, double b) {
    return a - b;
}

/**
 * Multiply two numbers
 */
double multiply(double a, double b) {
    return a * b;
}

/**
 * Divide two numbers
 */
double divide(double a, double b) {
    if (b == 0) {
        printf("Error: Division by zero!\n");
        return 0;
    }
    return a / b;
}

int main() {
    double num1, num2;
    char operation;

    printf("Simple Calculator\n");
    printf("Enter first number: ");
    scanf("%lf", &num1);

    printf("Enter operation (+, -, *, /): ");
    scanf(" %c", &operation);

    printf("Enter second number: ");
    scanf("%lf", &num2);

    double result;

    switch(operation) {
        case '+':
            result = add(num1, num2);
            printf("%.2f + %.2f = %.2f\n", num1, num2, result);
            break;
        case '-':
            result = subtract(num1, num2);
            printf("%.2f - %.2f = %.2f\n", num1, num2, result);
            break;
        case '*':
            result = multiply(num1, num2);
            printf("%.2f * %.2f = %.2f\n", num1, num2, result);
            break;
        case '/':
            result = divide(num1, num2);
            if (num2 != 0) {
                printf("%.2f / %.2f = %.2f\n", num1, num2, result);
            }
            break;
        default:
            printf("Invalid operation!\n");
            return 1;
    }

    return 0;
}
