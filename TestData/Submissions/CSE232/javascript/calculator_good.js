/**
 * Simple Calculator Implementation
 * Student: Good Example
 */

class Calculator {
    constructor() {
        this.result = 0;
    }

    // Addition
    add(a, b) {
        return a + b;
    }

    // Subtraction
    subtract(a, b) {
        return a - b;
    }

    // Multiplication
    multiply(a, b) {
        return a * b;
    }

    // Division with error handling
    divide(a, b) {
        if (b === 0) {
            throw new Error("Cannot divide by zero");
        }
        return a / b;
    }

    // Power
    power(base, exponent) {
        return Math.pow(base, exponent);
    }

    // Square root
    sqrt(num) {
        if (num < 0) {
            throw new Error("Cannot calculate square root of negative number");
        }
        return Math.sqrt(num);
    }

    // Modulo
    modulo(a, b) {
        return a % b;
    }
}

// Test the calculator
function runTests() {
    const calc = new Calculator();

    console.log("Calculator Tests:");
    console.log("5 + 3 =", calc.add(5, 3));
    console.log("10 - 4 =", calc.subtract(10, 4));
    console.log("6 * 7 =", calc.multiply(6, 7));
    console.log("20 / 5 =", calc.divide(20, 5));
    console.log("2^8 =", calc.power(2, 8));
    console.log("√16 =", calc.sqrt(16));
    console.log("17 % 5 =", calc.modulo(17, 5));

    // Test error handling
    try {
        console.log("10 / 0 =", calc.divide(10, 0));
    } catch (e) {
        console.log("Error:", e.message);
    }
}

runTests();
