/**
 * Array Methods Implementation
 * Student: Alex Rivera
 * CSE 232 - Data Structures
 *
 * Implementation of common array manipulation methods
 * using functional programming concepts.
 */

/**
 * Finds the maximum value in an array
 * @param {number[]} arr - Array of numbers
 * @returns {number} Maximum value or -Infinity for empty array
 */
function findMax(arr) {
    if (arr.length === 0) return -Infinity;
    return arr.reduce((max, current) => Math.max(max, current), arr[0]);
}

/**
 * Filters array to only include even numbers
 * @param {number[]} arr - Array of numbers
 * @returns {number[]} Array containing only even numbers
 */
function filterEvens(arr) {
    return arr.filter(num => num % 2 === 0);
}

/**
 * Maps array values by squaring each element
 * @param {number[]} arr - Array of numbers
 * @returns {number[]} Array with squared values
 */
function squareAll(arr) {
    return arr.map(num => num ** 2);
}

/**
 * Removes duplicates from array
 * @param {any[]} arr - Array with possible duplicates
 * @returns {any[]} Array with unique values
 */
function removeDuplicates(arr) {
    return [...new Set(arr)];
}

/**
 * Flattens a nested array by one level
 * @param {any[][]} arr - Nested array
 * @returns {any[]} Flattened array
 */
function flatten(arr) {
    return arr.flat();
}

// Test cases
const numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
const nested = [[1, 2], [3, 4], [5, 6]];
const withDupes = [1, 2, 2, 3, 3, 3, 4, 4, 4, 4];

console.log('Max value:', findMax(numbers));
console.log('Even numbers:', filterEvens(numbers));
console.log('Squared:', squareAll([1, 2, 3, 4, 5]));
console.log('Unique values:', removeDuplicates(withDupes));
console.log('Flattened:', flatten(nested));

module.exports = { findMax, filterEvens, squareAll, removeDuplicates, flatten };
