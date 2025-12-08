// BAD VERSION - no error handling, allows division by zero, poor structure
import java.util.Scanner;

public class Calculator {
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        System.out.print("Enter first number: ");
        double a = sc.nextDouble();
        System.out.print("Enter second number: ");
        double b = sc.nextDouble();
        System.out.print("Enter operation (+,-,*,/): ");
        String op = sc.next();
        
        // No validation, just do the operation
        if (op.equals("+")) System.out.println(a + b);
        if (op.equals("-")) System.out.println(a - b);
        if (op.equals("*")) System.out.println(a * b);
        if (op.equals("/")) System.out.println(a / b);  // Division by zero!
    }
}
