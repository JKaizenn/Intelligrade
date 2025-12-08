// BAD VERSION - no functions, hardcoded size, no sorting, minimal functionality
#include <stdio.h>

int main() {
    int g[5] = {85, 92, 78, 95, 88};
    int sum = 0;
    for (int i = 0; i < 5; i++) {
        sum += g[i];
    }
    printf("Average: %d\n", sum / 5);
    return 0;
}
