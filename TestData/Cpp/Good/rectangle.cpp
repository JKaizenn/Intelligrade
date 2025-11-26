/**
 * Rectangle Class Implementation
 * CSE 232 - Object-Oriented Programming
 * 
 * Demonstrates encapsulation, constructors, and input validation
 * in a simple geometry class.
 * 
 * @author Good Student
 * @date 2025-01-15
 */

#include <iostream>
#include <stdexcept>
#include <iomanip>

/**
 * Represents a rectangle with width and height dimensions.
 * Provides methods for calculating area and perimeter.
 */
class Rectangle {
private:
    double width_;
    double height_;

    /**
     * Validates that a dimension value is positive.
     * @param value The value to validate
     * @param name The name of the dimension for error messages
     * @throws std::invalid_argument if value is not positive
     */
    void validateDimension(double value, const std::string& name) const {
        if (value <= 0) {
            throw std::invalid_argument(name + " must be positive");
        }
    }

public:
    /**
     * Constructs a rectangle with specified dimensions.
     * @param width The width of the rectangle (must be positive)
     * @param height The height of the rectangle (must be positive)
     * @throws std::invalid_argument if dimensions are not positive
     */
    Rectangle(double width, double height) {
        validateDimension(width, "Width");
        validateDimension(height, "Height");
        width_ = width;
        height_ = height;
    }

    /**
     * Default constructor creates a 1x1 rectangle.
     */
    Rectangle() : width_(1.0), height_(1.0) {}

    // Getters
    double getWidth() const { return width_; }
    double getHeight() const { return height_; }

    // Setters with validation
    void setWidth(double width) {
        validateDimension(width, "Width");
        width_ = width;
    }

    void setHeight(double height) {
        validateDimension(height, "Height");
        height_ = height;
    }

    /**
     * Calculates the area of the rectangle.
     * @return The area (width * height)
     */
    double area() const {
        return width_ * height_;
    }

    /**
     * Calculates the perimeter of the rectangle.
     * @return The perimeter (2 * width + 2 * height)
     */
    double perimeter() const {
        return 2 * (width_ + height_);
    }

    /**
     * Determines if this is a square (width equals height).
     * @return true if width equals height, false otherwise
     */
    bool isSquare() const {
        return width_ == height_;
    }

    /**
     * Prints rectangle information to the console.
     */
    void display() const {
        std::cout << std::fixed << std::setprecision(2);
        std::cout << "Rectangle: " << width_ << " x " << height_ << std::endl;
        std::cout << "  Area: " << area() << std::endl;
        std::cout << "  Perimeter: " << perimeter() << std::endl;
        std::cout << "  Is Square: " << (isSquare() ? "Yes" : "No") << std::endl;
    }
};

int main() {
    std::cout << "Rectangle Class Demo" << std::endl;
    std::cout << "====================" << std::endl << std::endl;

    // Create rectangles
    Rectangle rect1(5.0, 3.0);
    Rectangle rect2(4.0, 4.0);
    Rectangle rect3;  // Default 1x1

    std::cout << "Rectangle 1:" << std::endl;
    rect1.display();

    std::cout << std::endl << "Rectangle 2 (Square):" << std::endl;
    rect2.display();

    std::cout << std::endl << "Rectangle 3 (Default):" << std::endl;
    rect3.display();

    // Test setter validation
    std::cout << std::endl << "Testing validation:" << std::endl;
    try {
        rect1.setWidth(-5);
    } catch (const std::invalid_argument& e) {
        std::cout << "Caught expected error: " << e.what() << std::endl;
    }

    return 0;
}
