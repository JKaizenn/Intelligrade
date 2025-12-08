#include <stdio.h>
#include "grades.h"

float calculateAverage(int grades[], int count) {
    if (count == 0) return 0.0f;

    int sum = 0;
    for (int i = 0; i < count; i++) {
        sum += grades[i];
    }
    return (float)sum / count;
}

int findMax(int grades[], int count) {
    if (count == 0) return 0;

    int max = grades[0];
    for (int i = 1; i < count; i++) {
        if (grades[i] > max) {
            max = grades[i];
        }
    }
    return max;
}

int findMin(int grades[], int count) {
    if (count == 0) return 0;

    int min = grades[0];
    for (int i = 1; i < count; i++) {
        if (grades[i] < min) {
            min = grades[i];
        }
    }
    return min;
}

void bubbleSort(int grades[], int count) {
    for (int i = 0; i < count - 1; i++) {
        for (int j = 0; j < count - i - 1; j++) {
            if (grades[j] > grades[j + 1]) {
                // Swap elements
                int temp = grades[j];
                grades[j] = grades[j + 1];
                grades[j + 1] = temp;
            }
        }
    }
}

void printGrades(int grades[], int count) {
    for (int i = 0; i < count; i++) {
        printf("%d ", grades[i]);
    }
    printf("\n");
}
