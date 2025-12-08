// BAD VERSION - messy code, magic numbers, poor structure
public class FizzBuzz {
    public static void main(String[] args) {
        for (int x = 1; x < 101; x++) {
            if (x % 3 == 0) System.out.println("Fizz");
            if (x % 5 == 0) System.out.println("Buzz");
            if (x % 3 != 0 && x % 5 != 0) System.out.println(x);
        }
    }
}
