#include <iostream>
#include <iomanip>
#include "Calculator.h"

using namespace std;

/**
 * Displays the menu of operations
 */
void displayMenu() {
    cout << "\n=== Calculator ===\n";
    cout << "1. Add\n";
    cout << "2. Subtract\n";
    cout << "3. Multiply\n";
    cout << "4. Divide\n";
    cout << "5. Exit\n";
    cout << "Enter choice: ";
}

/**
 * Main program - calculator with menu interface
 */
int main() {
    Calculator calc;
    int choice;
    double num1, num2, result;

    cout << fixed << setprecision(2);

    do {
        displayMenu();
        cin >> choice;

        if (choice >= 1 && choice <= 4) {
            cout << "Enter first number: ";
            cin >> num1;
            cout << "Enter second number: ";
            cin >> num2;

            try {
                switch (choice) {
                    case 1:
                        result = calc.add(num1, num2);
                        cout << "Result: " << num1 << " + " << num2 << " = " << result << endl;
                        break;
                    case 2:
                        result = calc.subtract(num1, num2);
                        cout << "Result: " << num1 << " - " << num2 << " = " << result << endl;
                        break;
                    case 3:
                        result = calc.multiply(num1, num2);
                        cout << "Result: " << num1 << " * " << num2 << " = " << result << endl;
                        break;
                    case 4:
                        result = calc.divide(num1, num2);
                        cout << "Result: " << num1 << " / " << num2 << " = " << result << endl;
                        break;
                }
            } catch (const exception& e) {
                cout << "Error: " << e.what() << endl;
            }
        }
    } while (choice != 5);

    cout << "Goodbye!\n";
    return 0;
}
