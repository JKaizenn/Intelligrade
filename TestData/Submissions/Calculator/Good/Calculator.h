#ifndef CALCULATOR_H
#define CALCULATOR_H

#include <stdexcept>

/**
 * Calculator class - performs basic arithmetic operations
 * with proper error handling
 */
class Calculator {
public:
    /**
     * Adds two numbers
     * @param a First number
     * @param b Second number
     * @return Sum of a and b
     */
    double add(double a, double b);

    /**
     * Subtracts two numbers
     * @param a First number
     * @param b Second number
     * @return Difference of a and b
     */
    double subtract(double a, double b);

    /**
     * Multiplies two numbers
     * @param a First number
     * @param b Second number
     * @return Product of a and b
     */
    double multiply(double a, double b);

    /**
     * Divides two numbers
     * @param a Numerator
     * @param b Denominator
     * @return Quotient of a divided by b
     * @throws std::invalid_argument if b is zero
     */
    double divide(double a, double b);
};

#endif
