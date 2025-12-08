// MEDIUM VERSION - has class but all in one file, basic validation
#include <iostream>
#include <string>
using namespace std;

class BankAccount {
private:
    string owner;
    double balance;

public:
    BankAccount(string name, double initialBalance) {
        owner = name;
        balance = initialBalance;
    }

    void deposit(double amount) {
        if (amount > 0) {
            balance += amount;
            cout << "Deposited: $" << amount << endl;
        }
    }

    void withdraw(double amount) {
        if (amount > 0 && amount <= balance) {
            balance -= amount;
            cout << "Withdrew: $" << amount << endl;
        } else {
            cout << "Insufficient funds" << endl;
        }
    }

    void display() {
        cout << "Account Owner: " << owner << endl;
        cout << "Balance: $" << balance << endl;
    }
};

int main() {
    BankAccount account("John Doe", 1000.0);
    account.display();
    account.deposit(500);
    account.withdraw(200);
    account.display();
    return 0;
}
