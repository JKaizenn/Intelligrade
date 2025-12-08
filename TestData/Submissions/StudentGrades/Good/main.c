#include <stdio.h>
#include "grades.h"

#define MAX_STUDENTS 10

/**
 * Main program - demonstrates grade statistics and sorting
 */
int main() {
    int grades[MAX_STUDENTS] = {85, 92, 78, 95, 88, 73, 90, 84, 79, 91};
    int count = MAX_STUDENTS;

    // Display original grades
    printf("=== Student Grade Analysis ===\n\n");
    printf("Original grades (%d students):\n", count);
    printGrades(grades, count);

    // Calculate and display statistics
    printf("\nStatistics:\n");
    printf("Average: %.2f\n", calculateAverage(grades, count));
    printf("Highest: %d\n", findMax(grades, count));
    printf("Lowest: %d\n", findMin(grades, count));

    // Sort and display sorted grades
    bubbleSort(grades, count);
    printf("\nSorted grades (ascending order):\n");
    printGrades(grades, count);

    return 0;
}
