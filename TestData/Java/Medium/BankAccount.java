// Bank Account class

public class BankAccount {
    private String name;
    private double balance;
    
    public BankAccount(String name, double balance) {
        this.name = name;
        this.balance = balance;
    }
    
    public String getName() {
        return name;
    }
    
    public double getBalance() {
        return balance;
    }
    
    public void deposit(double amount) {
        balance = balance + amount;
        System.out.println("Deposited: " + amount);
    }
    
    public void withdraw(double amount) {
        if (amount <= balance) {
            balance = balance - amount;
            System.out.println("Withdrew: " + amount);
        } else {
            System.out.println("Not enough money");
        }
    }
    
    public static void main(String[] args) {
        BankAccount account = new BankAccount("John", 1000);
        
        System.out.println("Account: " + account.getName());
        System.out.println("Balance: " + account.getBalance());
        
        account.deposit(500);
        System.out.println("Balance: " + account.getBalance());
        
        account.withdraw(200);
        System.out.println("Balance: " + account.getBalance());
        
        account.withdraw(5000);
        System.out.println("Balance: " + account.getBalance());
    }
}
