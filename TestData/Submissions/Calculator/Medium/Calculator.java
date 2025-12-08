// MEDIUM VERSION - has basic validation but all in one file, minimal OOP
import java.util.Scanner;

public class Calculator {
    
    public static double calculate(double a, double b, String op) {
        switch(op) {
            case "+": return a + b;
            case "-": return a - b;
            case "*": return a * b;
            case "/": 
                if (b != 0) return a / b;
                else {
                    System.out.println("Error: Division by zero");
                    return 0;
                }
            default:
                System.out.println("Invalid operation");
                return 0;
        }
    }
    
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        System.out.print("Enter first number: ");
        double a = sc.nextDouble();
        System.out.print("Enter second number: ");
        double b = sc.nextDouble();
        System.out.print("Enter operation (+,-,*,/): ");
        String op = sc.next();
        
        double result = calculate(a, b, op);
        System.out.println("Result: " + result);
    }
}
