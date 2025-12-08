// MEDIUM VERSION - has functions but all in one file, basic sorting
#include <stdio.h>

#define MAX_STUDENTS 10

float calculateAverage(int grades[], int count) {
    int sum = 0;
    for (int i = 0; i < count; i++) {
        sum += grades[i];
    }
    return (float)sum / count;
}

int findMax(int grades[], int count) {
    int max = grades[0];
    for (int i = 1; i < count; i++) {
        if (grades[i] > max) max = grades[i];
    }
    return max;
}

int findMin(int grades[], int count) {
    int min = grades[0];
    for (int i = 1; i < count; i++) {
        if (grades[i] < min) min = grades[i];
    }
    return min;
}

void bubbleSort(int grades[], int count) {
    for (int i = 0; i < count - 1; i++) {
        for (int j = 0; j < count - i - 1; j++) {
            if (grades[j] > grades[j + 1]) {
                int temp = grades[j];
                grades[j] = grades[j + 1];
                grades[j + 1] = temp;
            }
        }
    }
}

int main() {
    int grades[MAX_STUDENTS] = {85, 92, 78, 95, 88, 73, 90, 84, 79, 91};
    int count = 10;

    printf("Original grades:\n");
    for (int i = 0; i < count; i++) {
        printf("%d ", grades[i]);
    }
    printf("\n");

    printf("Average: %.2f\n", calculateAverage(grades, count));
    printf("Highest: %d\n", findMax(grades, count));
    printf("Lowest: %d\n", findMin(grades, count));

    bubbleSort(grades, count);
    printf("\nSorted grades:\n");
    for (int i = 0; i < count; i++) {
        printf("%d ", grades[i]);
    }
    printf("\n");

    return 0;
}
