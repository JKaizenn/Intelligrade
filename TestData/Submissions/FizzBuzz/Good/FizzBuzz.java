/**
 * FizzBuzz - GOOD VERSION
 * Prints numbers from 1 to 100 with the following rules:
 * - Print "Fizz" for multiples of 3
 * - Print "Buzz" for multiples of 5
 * - Print "FizzBuzz" for multiples of both 3 and 5
 * - Print the number itself otherwise
 */
public class FizzBuzz {
    private static final int START = 1;
    private static final int END = 100;
    private static final int FIZZ_DIVISOR = 3;
    private static final int BUZZ_DIVISOR = 5;

    /**
     * Main method - entry point of the program
     */
    public static void main(String[] args) {
        printFizzBuzz(START, END);
    }

    /**
     * Prints the FizzBuzz sequence from start to end (inclusive)
     *
     * @param start The starting number
     * @param end The ending number
     */
    private static void printFizzBuzz(int start, int end) {
        for (int number = start; number <= end; number++) {
            System.out.println(getFizzBuzzValue(number));
        }
    }

    /**
     * Returns the FizzBuzz value for a given number
     *
     * @param number The number to evaluate
     * @return The FizzBuzz string or the number as a string
     */
    private static String getFizzBuzzValue(int number) {
        StringBuilder result = new StringBuilder();

        if (number % FIZZ_DIVISOR == 0) {
            result.append("Fizz");
        }

        if (number % BUZZ_DIVISOR == 0) {
            result.append("Buzz");
        }

        return result.length() > 0 ? result.toString() : String.valueOf(number);
    }
}
