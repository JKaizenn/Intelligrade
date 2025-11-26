/**
 * Bank Account Class
 * CSE 210 - Programming with Classes
 * 
 * Demonstrates encapsulation, constructors, and input validation
 * in a banking domain class.
 * 
 * @author Good Student
 * @version 1.0
 */

/**
 * Represents a bank account with deposit and withdrawal capabilities.
 * Enforces business rules such as no negative balances or deposits.
 */
public class BankAccount {
    
    /** The account holder's name */
    private String accountHolder;
    
    /** The unique account number */
    private String accountNumber;
    
    /** The current balance in dollars */
    private double balance;
    
    /** Counter for generating unique account numbers */
    private static int nextAccountNumber = 1000;
    
    /**
     * Creates a new bank account with an initial deposit.
     * 
     * @param accountHolder The name of the account holder
     * @param initialDeposit The initial deposit amount (must be non-negative)
     * @throws IllegalArgumentException if holder name is empty or deposit is negative
     */
    public BankAccount(String accountHolder, double initialDeposit) {
        if (accountHolder == null || accountHolder.trim().isEmpty()) {
            throw new IllegalArgumentException("Account holder name cannot be empty");
        }
        if (initialDeposit < 0) {
            throw new IllegalArgumentException("Initial deposit cannot be negative");
        }
        
        this.accountHolder = accountHolder.trim();
        this.accountNumber = "ACC" + (nextAccountNumber++);
        this.balance = initialDeposit;
    }
    
    /**
     * Creates a new bank account with zero balance.
     * 
     * @param accountHolder The name of the account holder
     */
    public BankAccount(String accountHolder) {
        this(accountHolder, 0.0);
    }
    
    /**
     * Gets the account holder's name.
     * @return The account holder name
     */
    public String getAccountHolder() {
        return accountHolder;
    }
    
    /**
     * Gets the account number.
     * @return The unique account number
     */
    public String getAccountNumber() {
        return accountNumber;
    }
    
    /**
     * Gets the current balance.
     * @return The current balance in dollars
     */
    public double getBalance() {
        return balance;
    }
    
    /**
     * Deposits money into the account.
     * 
     * @param amount The amount to deposit (must be positive)
     * @return true if deposit was successful
     * @throws IllegalArgumentException if amount is not positive
     */
    public boolean deposit(double amount) {
        if (amount <= 0) {
            throw new IllegalArgumentException("Deposit amount must be positive");
        }
        
        balance += amount;
        System.out.printf("Deposited $%.2f. New balance: $%.2f%n", amount, balance);
        return true;
    }
    
    /**
     * Withdraws money from the account if sufficient funds exist.
     * 
     * @param amount The amount to withdraw (must be positive)
     * @return true if withdrawal was successful, false if insufficient funds
     * @throws IllegalArgumentException if amount is not positive
     */
    public boolean withdraw(double amount) {
        if (amount <= 0) {
            throw new IllegalArgumentException("Withdrawal amount must be positive");
        }
        
        if (amount > balance) {
            System.out.printf("Insufficient funds. Available: $%.2f, Requested: $%.2f%n", 
                            balance, amount);
            return false;
        }
        
        balance -= amount;
        System.out.printf("Withdrew $%.2f. New balance: $%.2f%n", amount, balance);
        return true;
    }
    
    /**
     * Transfers money to another account.
     * 
     * @param recipient The account to transfer to
     * @param amount The amount to transfer
     * @return true if transfer was successful
     */
    public boolean transfer(BankAccount recipient, double amount) {
        if (recipient == null) {
            throw new IllegalArgumentException("Recipient account cannot be null");
        }
        
        if (this.withdraw(amount)) {
            recipient.deposit(amount);
            System.out.printf("Transferred $%.2f to account %s%n", 
                            amount, recipient.getAccountNumber());
            return true;
        }
        return false;
    }
    
    /**
     * Returns a string representation of the account.
     * @return Account details as a formatted string
     */
    @Override
    public String toString() {
        return String.format("BankAccount[%s, Holder: %s, Balance: $%.2f]",
                           accountNumber, accountHolder, balance);
    }
    
    /**
     * Main method demonstrating bank account functionality.
     */
    public static void main(String[] args) {
        System.out.println("=== Bank Account Demo ===\n");
        
        // Create accounts
        BankAccount savings = new BankAccount("Alice Johnson", 1000.00);
        BankAccount checking = new BankAccount("Alice Johnson", 500.00);
        
        System.out.println("Created accounts:");
        System.out.println("  " + savings);
        System.out.println("  " + checking);
        
        // Perform transactions
        System.out.println("\n--- Transactions ---");
        savings.deposit(250.00);
        savings.withdraw(100.00);
        
        // Test insufficient funds
        System.out.println("\nAttempting large withdrawal:");
        savings.withdraw(5000.00);
        
        // Test transfer
        System.out.println("\nTransferring between accounts:");
        savings.transfer(checking, 200.00);
        
        // Final balances
        System.out.println("\n--- Final Balances ---");
        System.out.println("  " + savings);
        System.out.println("  " + checking);
        
        // Test validation
        System.out.println("\n--- Testing Validation ---");
        try {
            savings.deposit(-50);
        } catch (IllegalArgumentException e) {
            System.out.println("Caught expected error: " + e.getMessage());
        }
    }
}
