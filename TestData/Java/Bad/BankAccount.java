public class BankAccount {
public String n;  // public field - no encapsulation
public double b;

public BankAccount(String n,double b){
this.n=n;
this.b=b;
}

public void dep(double a){
b=b+a;  // no validation for negative amounts
}

public void wd(double a){
b=b-a;  // Bug: allows overdraft, no check
}

public static void main(String[] args){
BankAccount x=new BankAccount("Bob",100);
x.dep(-500);  // deposits negative (actually subtracts)
System.out.println(x.b);
x.wd(1000);  // withdraws more than balance
System.out.println(x.b);  // negative balance allowed
x.b=999999;  // can directly modify balance (no encapsulation)
System.out.println(x.b);
}
}
