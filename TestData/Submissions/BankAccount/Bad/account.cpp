// BAD VERSION - no validation, no error handling, poor design
#include <iostream>
using namespace std;

int main() {
    double balance = 100;
    balance = balance - 150;  // Can go negative!
    cout << balance << endl;
    return 0;
}
