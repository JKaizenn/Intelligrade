/**
 * Array Statistics Calculator
 * WDD 230 - Web Frontend Development
 * 
 * This module provides utility functions for calculating
 * statistics on arrays of numbers.
 * 
 * @author Good Student
 * @version 1.0.0
 */

/**
 * Calculates the sum of all numbers in an array.
 * @param {number[]} numbers - Array of numbers to sum
 * @returns {number} The sum, or 0 for empty/invalid input
 */
const calculateSum = (numbers) => {
    if (!Array.isArray(numbers) || numbers.length === 0) {
        return 0;
    }
    return numbers.reduce((total, num) => total + num, 0);
};

/**
 * Calculates the average of all numbers in an array.
 * @param {number[]} numbers - Array of numbers
 * @returns {number} The average, or 0 for empty/invalid input
 */
const calculateAverage = (numbers) => {
    if (!Array.isArray(numbers) || numbers.length === 0) {
        return 0;
    }
    return calculateSum(numbers) / numbers.length;
};

/**
 * Finds the minimum value in an array.
 * @param {number[]} numbers - Array of numbers
 * @returns {number|null} The minimum value, or null for empty/invalid input
 */
const findMin = (numbers) => {
    if (!Array.isArray(numbers) || numbers.length === 0) {
        return null;
    }
    return Math.min(...numbers);
};

/**
 * Finds the maximum value in an array.
 * @param {number[]} numbers - Array of numbers
 * @returns {number|null} The maximum value, or null for empty/invalid input
 */
const findMax = (numbers) => {
    if (!Array.isArray(numbers) || numbers.length === 0) {
        return null;
    }
    return Math.max(...numbers);
};

/**
 * Filters an array to only include numbers above a threshold.
 * @param {number[]} numbers - Array of numbers to filter
 * @param {number} threshold - Minimum value (exclusive)
 * @returns {number[]} Filtered array of numbers above threshold
 */
const filterAboveThreshold = (numbers, threshold) => {
    if (!Array.isArray(numbers)) {
        return [];
    }
    return numbers.filter(num => num > threshold);
};

// Test the functions
const testData = [23, 45, 12, 67, 34, 89, 5, 42];

console.log("Array Statistics Calculator");
console.log("===========================");
console.log(`Test data: [${testData.join(", ")}]`);
console.log("");
console.log(`Sum: ${calculateSum(testData)}`);
console.log(`Average: ${calculateAverage(testData).toFixed(2)}`);
console.log(`Minimum: ${findMin(testData)}`);
console.log(`Maximum: ${findMax(testData)}`);
console.log(`Values above 30: [${filterAboveThreshold(testData, 30).join(", ")}]`);

// Test edge cases
console.log("");
console.log("Edge Case Tests:");
console.log(`Empty array sum: ${calculateSum([])}`);
console.log(`Empty array min: ${findMin([])}`);
console.log(`Single element average: ${calculateAverage([42])}`);
