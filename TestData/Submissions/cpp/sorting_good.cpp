/**
 * Sorting Algorithms Implementation
 * Student: Alex Johnson
 * CSE 232 - Data Structures
 *
 * Time Complexity Analysis:
 * - Bubble Sort: O(n²) average and worst case, O(n) best case
 * - Quick Sort: O(n log n) average, O(n²) worst case
 * - Merge Sort: O(n log n) all cases
 */

#include <iostream>
#include <vector>
#include <algorithm>

/**
 * Bubble Sort - Simple comparison-based sorting
 * Optimized with early termination if array is sorted
 */
template<typename T>
void bubbleSort(std::vector<T>& arr) {
    size_t n = arr.size();
    for (size_t i = 0; i < n - 1; i++) {
        bool swapped = false;
        for (size_t j = 0; j < n - i - 1; j++) {
            if (arr[j] > arr[j + 1]) {
                std::swap(arr[j], arr[j + 1]);
                swapped = true;
            }
        }
        // Early termination if no swaps occurred
        if (!swapped) break;
    }
}

/**
 * Partition function for Quick Sort
 */
template<typename T>
size_t partition(std::vector<T>& arr, size_t low, size_t high) {
    T pivot = arr[high];
    size_t i = low;

    for (size_t j = low; j < high; j++) {
        if (arr[j] < pivot) {
            std::swap(arr[i], arr[j]);
            i++;
        }
    }
    std::swap(arr[i], arr[high]);
    return i;
}

/**
 * Quick Sort - Divide and conquer sorting algorithm
 */
template<typename T>
void quickSortHelper(std::vector<T>& arr, size_t low, size_t high) {
    if (low < high) {
        size_t pi = partition(arr, low, high);
        if (pi > 0) quickSortHelper(arr, low, pi - 1);
        quickSortHelper(arr, pi + 1, high);
    }
}

template<typename T>
void quickSort(std::vector<T>& arr) {
    if (!arr.empty()) {
        quickSortHelper(arr, 0, arr.size() - 1);
    }
}

/**
 * Merge function for Merge Sort
 */
template<typename T>
void merge(std::vector<T>& arr, size_t left, size_t mid, size_t right) {
    std::vector<T> leftArr(arr.begin() + left, arr.begin() + mid + 1);
    std::vector<T> rightArr(arr.begin() + mid + 1, arr.begin() + right + 1);

    size_t i = 0, j = 0, k = left;

    while (i < leftArr.size() && j < rightArr.size()) {
        if (leftArr[i] <= rightArr[j]) {
            arr[k++] = leftArr[i++];
        } else {
            arr[k++] = rightArr[j++];
        }
    }

    while (i < leftArr.size()) arr[k++] = leftArr[i++];
    while (j < rightArr.size()) arr[k++] = rightArr[j++];
}

/**
 * Merge Sort - Stable divide and conquer algorithm
 */
template<typename T>
void mergeSortHelper(std::vector<T>& arr, size_t left, size_t right) {
    if (left < right) {
        size_t mid = left + (right - left) / 2;
        mergeSortHelper(arr, left, mid);
        mergeSortHelper(arr, mid + 1, right);
        merge(arr, left, mid, right);
    }
}

template<typename T>
void mergeSort(std::vector<T>& arr) {
    if (!arr.empty()) {
        mergeSortHelper(arr, 0, arr.size() - 1);
    }
}

/**
 * Utility function to print a vector
 */
template<typename T>
void printVector(const std::vector<T>& arr, const std::string& label) {
    std::cout << label << ": [";
    for (size_t i = 0; i < arr.size(); i++) {
        std::cout << arr[i];
        if (i < arr.size() - 1) std::cout << ", ";
    }
    std::cout << "]" << std::endl;
}

/**
 * Test function to verify sorting correctness
 */
template<typename T>
bool isSorted(const std::vector<T>& arr) {
    for (size_t i = 1; i < arr.size(); i++) {
        if (arr[i - 1] > arr[i]) return false;
    }
    return true;
}

int main() {
    // Test data
    std::vector<int> original = {64, 34, 25, 12, 22, 11, 90, 88, 45, 50};

    // Test Bubble Sort
    std::vector<int> arr1 = original;
    printVector(arr1, "Original");
    bubbleSort(arr1);
    printVector(arr1, "Bubble Sort");
    std::cout << "Is sorted: " << (isSorted(arr1) ? "Yes" : "No") << "\n\n";

    // Test Quick Sort
    std::vector<int> arr2 = original;
    quickSort(arr2);
    printVector(arr2, "Quick Sort");
    std::cout << "Is sorted: " << (isSorted(arr2) ? "Yes" : "No") << "\n\n";

    // Test Merge Sort
    std::vector<int> arr3 = original;
    mergeSort(arr3);
    printVector(arr3, "Merge Sort");
    std::cout << "Is sorted: " << (isSorted(arr3) ? "Yes" : "No") << "\n\n";

    // Edge case tests
    std::vector<int> empty;
    bubbleSort(empty);
    std::cout << "Empty array test: " << (isSorted(empty) ? "Pass" : "Fail") << std::endl;

    std::vector<int> single = {42};
    quickSort(single);
    std::cout << "Single element test: " << (isSorted(single) ? "Pass" : "Fail") << std::endl;

    std::vector<int> sorted = {1, 2, 3, 4, 5};
    mergeSort(sorted);
    std::cout << "Already sorted test: " << (isSorted(sorted) ? "Pass" : "Fail") << std::endl;

    return 0;
}
