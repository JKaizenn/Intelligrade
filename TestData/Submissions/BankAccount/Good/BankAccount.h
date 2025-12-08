#ifndef BANKACCOUNT_H
#define BANKACCOUNT_H

#include <string>

/**
 * BankAccount class - represents a bank account with deposit/withdraw operations
 */
class BankAccount {
private:
    std::string accountNumber;
    std::string ownerName;
    double balance;

    static int nextAccountNumber;

public:
    /**
     * Constructor - creates a new bank account
     * @param owner Name of the account owner
     * @param initialDeposit Initial deposit amount (must be positive)
     */
    BankAccount(const std::string& owner, double initialDeposit);

    /**
     * Deposits money into the account
     * @param amount Amount to deposit (must be positive)
     * @return true if successful, false otherwise
     */
    bool deposit(double amount);

    /**
     * Withdraws money from the account
     * @param amount Amount to withdraw (must be positive and <= balance)
     * @return true if successful, false otherwise
     */
    bool withdraw(double amount);

    /**
     * Gets the current balance
     * @return Current balance
     */
    double getBalance() const;

    /**
     * Gets the account owner's name
     * @return Owner name
     */
    std::string getOwnerName() const;

    /**
     * Gets the account number
     * @return Account number
     */
    std::string getAccountNumber() const;

    /**
     * Displays account information
     */
    void displayInfo() const;
};

#endif
