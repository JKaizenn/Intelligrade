// BAD VERSION - all in one file, no error handling, poor design
#include <iostream>
using namespace std;

int main() {
    int a, b;
    char op;
    cout << "Enter: ";
    cin >> a >> op >> b;

    if (op == '+') cout << a + b;
    if (op == '-') cout << a - b;
    if (op == '*') cout << a * b;
    if (op == '/') cout << a / b;  // No division by zero check!

    return 0;
}
