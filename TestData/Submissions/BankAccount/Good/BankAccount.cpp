#include "BankAccount.h"
#include <iostream>
#include <iomanip>

int BankAccount::nextAccountNumber = 1000;

BankAccount::BankAccount(const std::string& owner, double initialDeposit)
    : ownerName(owner), balance(0.0) {

    // Generate unique account number
    accountNumber = "ACC" + std::to_string(nextAccountNumber++);

    // Validate and set initial deposit
    if (initialDeposit > 0) {
        balance = initialDeposit;
    } else {
        std::cerr << "Warning: Initial deposit must be positive. Starting with $0.\n";
    }
}

bool BankAccount::deposit(double amount) {
    if (amount <= 0) {
        std::cerr << "Error: Deposit amount must be positive.\n";
        return false;
    }

    balance += amount;
    std::cout << "Successfully deposited $" << std::fixed
              << std::setprecision(2) << amount << "\n";
    return true;
}

bool BankAccount::withdraw(double amount) {
    if (amount <= 0) {
        std::cerr << "Error: Withdrawal amount must be positive.\n";
        return false;
    }

    if (amount > balance) {
        std::cerr << "Error: Insufficient funds. Current balance: $"
                  << std::fixed << std::setprecision(2) << balance << "\n";
        return false;
    }

    balance -= amount;
    std::cout << "Successfully withdrew $" << std::fixed
              << std::setprecision(2) << amount << "\n";
    return true;
}

double BankAccount::getBalance() const {
    return balance;
}

std::string BankAccount::getOwnerName() const {
    return ownerName;
}

std::string BankAccount::getAccountNumber() const {
    return accountNumber;
}

void BankAccount::displayInfo() const {
    std::cout << "\n=== Account Information ===\n";
    std::cout << "Account Number: " << accountNumber << "\n";
    std::cout << "Owner: " << ownerName << "\n";
    std::cout << "Balance: $" << std::fixed << std::setprecision(2) << balance << "\n";
    std::cout << "===========================\n\n";
}
