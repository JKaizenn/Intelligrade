// Rectangle class

#include <iostream>

class Rectangle {
private:
    double width;
    double height;

public:
    Rectangle(double w, double h) {
        width = w;
        height = h;
    }

    double getWidth() { return width; }
    double getHeight() { return height; }

    void setWidth(double w) { width = w; }
    void setHeight(double h) { height = h; }

    double area() {
        return width * height;
    }

    double perimeter() {
        return 2 * width + 2 * height;
    }
};

int main() {
    Rectangle r(5, 3);

    std::cout << "Width: " << r.getWidth() << std::endl;
    std::cout << "Height: " << r.getHeight() << std::endl;
    std::cout << "Area: " << r.area() << std::endl;
    std::cout << "Perimeter: " << r.perimeter() << std::endl;

    r.setWidth(10);
    std::cout << "New area: " << r.area() << std::endl;

    return 0;
}
