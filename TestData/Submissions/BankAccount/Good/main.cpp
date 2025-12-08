#include <iostream>
#include "BankAccount.h"

using namespace std;

/**
 * Main program - demonstrates BankAccount functionality
 */
int main() {
    // Create accounts
    BankAccount account1("Alice Johnson", 1000.0);
    BankAccount account2("Bob Smith", 500.0);

    // Display initial state
    account1.displayInfo();
    account2.displayInfo();

    // Perform transactions
    cout << "=== Performing Transactions ===\n\n";

    account1.deposit(250.0);
    account1.withdraw(100.0);
    account1.displayInfo();

    // Test error handling
    cout << "=== Testing Error Handling ===\n\n";
    account2.withdraw(1000.0);  // Should fail - insufficient funds
    account2.deposit(-50.0);    // Should fail - negative amount

    account2.displayInfo();

    return 0;
}
