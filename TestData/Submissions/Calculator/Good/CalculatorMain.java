import java.util.Scanner;

/**
 * Main class for Calculator program
 */
public class CalculatorMain {
    public static void main(String[] args) {
        Calculator calc = new Calculator();
        Scanner scanner = new Scanner(System.in);
        int choice;

        do {
            displayMenu();
            choice = scanner.nextInt();

            if (choice >= 1 && choice <= 4) {
                System.out.print("Enter first number: ");
                double num1 = scanner.nextDouble();
                System.out.print("Enter second number: ");
                double num2 = scanner.nextDouble();

                try {
                    double result = 0;
                    switch (choice) {
                        case 1:
                            result = calc.add(num1, num2);
                            System.out.printf("Result: %.2f + %.2f = %.2f%n", num1, num2, result);
                            break;
                        case 2:
                            result = calc.subtract(num1, num2);
                            System.out.printf("Result: %.2f - %.2f = %.2f%n", num1, num2, result);
                            break;
                        case 3:
                            result = calc.multiply(num1, num2);
                            System.out.printf("Result: %.2f * %.2f = %.2f%n", num1, num2, result);
                            break;
                        case 4:
                            result = calc.divide(num1, num2);
                            System.out.printf("Result: %.2f / %.2f = %.2f%n", num1, num2, result);
                            break;
                    }
                } catch (ArithmeticException e) {
                    System.out.println("Error: " + e.getMessage());
                }
            }
        } while (choice != 5);

        System.out.println("Goodbye!");
        scanner.close();
    }

    private static void displayMenu() {
        System.out.println("\n=== Calculator ===");
        System.out.println("1. Add");
        System.out.println("2. Subtract");
        System.out.println("3. Multiply");
        System.out.println("4. Divide");
        System.out.println("5. Exit");
        System.out.print("Enter choice: ");
    }
}
