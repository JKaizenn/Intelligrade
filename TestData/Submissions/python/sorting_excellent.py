"""
Sorting Algorithms Implementation
Student: Excellent Example
This module implements multiple sorting algorithms with proper documentation
and error handling.
"""

def bubble_sort(arr):
    """
    Implements bubble sort algorithm.

    Args:
        arr: List of comparable elements

    Returns:
        Sorted list in ascending order

    Time Complexity: O(n²)
    Space Complexity: O(1)
    """
    if not arr:
        return []

    n = len(arr)
    arr_copy = arr.copy()  # Don't modify original

    for i in range(n):
        swapped = False
        for j in range(0, n - i - 1):
            if arr_copy[j] > arr_copy[j + 1]:
                arr_copy[j], arr_copy[j + 1] = arr_copy[j + 1], arr_copy[j]
                swapped = True

        # Optimization: if no swaps, array is sorted
        if not swapped:
            break

    return arr_copy


def quick_sort(arr):
    """
    Implements quick sort algorithm using Lomuto partition scheme.

    Args:
        arr: List of comparable elements

    Returns:
        Sorted list in ascending order

    Time Complexity: O(n log n) average, O(n²) worst
    Space Complexity: O(log n) due to recursion
    """
    if len(arr) <= 1:
        return arr

    pivot = arr[len(arr) // 2]
    left = [x for x in arr if x < pivot]
    middle = [x for x in arr if x == pivot]
    right = [x for x in arr if x > pivot]

    return quick_sort(left) + middle + quick_sort(right)


def merge_sort(arr):
    """
    Implements merge sort algorithm.

    Args:
        arr: List of comparable elements

    Returns:
        Sorted list in ascending order

    Time Complexity: O(n log n)
    Space Complexity: O(n)
    """
    if len(arr) <= 1:
        return arr

    mid = len(arr) // 2
    left = merge_sort(arr[:mid])
    right = merge_sort(arr[mid:])

    return merge(left, right)


def merge(left, right):
    """Helper function to merge two sorted arrays."""
    result = []
    i = j = 0

    while i < len(left) and j < len(right):
        if left[i] <= right[j]:
            result.append(left[i])
            i += 1
        else:
            result.append(right[j])
            j += 1

    result.extend(left[i:])
    result.extend(right[j:])

    return result


def main():
    """Test all sorting algorithms."""
    test_cases = [
        [64, 34, 25, 12, 22, 11, 90],
        [5, 2, 8, 1, 9],
        [],
        [1],
        [3, 3, 3, 3]
    ]

    for test in test_cases:
        print(f"\nOriginal: {test}")
        print(f"Bubble Sort: {bubble_sort(test)}")
        print(f"Quick Sort: {quick_sort(test)}")
        print(f"Merge Sort: {merge_sort(test)}")


if __name__ == "__main__":
    main()
