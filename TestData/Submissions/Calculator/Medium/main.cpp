// MEDIUM VERSION - has some organization but still mostly in main
#include <iostream>
using namespace std;

double calculate(double a, double b, char op) {
    if (op == '+') return a + b;
    else if (op == '-') return a - b;
    else if (op == '*') return a * b;
    else if (op == '/') {
        if (b != 0) return a / b;
        else {
            cout << "Error: Division by zero" << endl;
            return 0;
        }
    }
    return 0;
}

int main() {
    double num1, num2;
    char operation;

    cout << "Enter calculation (e.g., 5 + 3): ";
    cin >> num1 >> operation >> num2;

    double result = calculate(num1, num2, operation);
    cout << "Result: " << result << endl;

    return 0;
}
