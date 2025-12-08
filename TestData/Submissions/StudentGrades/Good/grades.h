#ifndef GRADES_H
#define GRADES_H

/**
 * Calculates the average of an array of grades
 * @param grades Array of integer grades
 * @param count Number of grades in the array
 * @return Average as a float
 */
float calculateAverage(int grades[], int count);

/**
 * Finds the maximum grade in the array
 * @param grades Array of integer grades
 * @param count Number of grades in the array
 * @return Maximum grade
 */
int findMax(int grades[], int count);

/**
 * Finds the minimum grade in the array
 * @param grades Array of integer grades
 * @param count Number of grades in the array
 * @return Minimum grade
 */
int findMin(int grades[], int count);

/**
 * Sorts the grades array in ascending order using bubble sort
 * @param grades Array of integer grades (modified in place)
 * @param count Number of grades in the array
 */
void bubbleSort(int grades[], int count);

/**
 * Prints an array of grades
 * @param grades Array of integer grades
 * @param count Number of grades in the array
 */
void printGrades(int grades[], int count);

#endif
