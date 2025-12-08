/**
 * Calculator class - performs basic arithmetic operations
 */
public class Calculator {
    /**
     * Adds two numbers
     */
    public double add(double a, double b) {
        return a + b;
    }

    /**
     * Subtracts two numbers
     */
    public double subtract(double a, double b) {
        return a - b;
    }

    /**
     * Multiplies two numbers
     */
    public double multiply(double a, double b) {
        return a * b;
    }

    /**
     * Divides two numbers
     * @throws ArithmeticException if divisor is zero
     */
    public double divide(double a, double b) {
        if (b == 0) {
            throw new ArithmeticException("Division by zero is not allowed");
        }
        return a / b;
    }
}
