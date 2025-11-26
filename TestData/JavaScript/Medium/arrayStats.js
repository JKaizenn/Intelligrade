// Array statistics functions

function sum(arr) {
    let total = 0;
    for (let i = 0; i < arr.length; i++) {
        total += arr[i];
    }
    return total;
}

function average(arr) {
    return sum(arr) / arr.length;
}

function min(arr) {
    let smallest = arr[0];
    for (let i = 1; i < arr.length; i++) {
        if (arr[i] < smallest) {
            smallest = arr[i];
        }
    }
    return smallest;
}

function max(arr) {
    let largest = arr[0];
    for (let i = 1; i < arr.length; i++) {
        if (arr[i] > largest) {
            largest = arr[i];
        }
    }
    return largest;
}

function filterAbove(arr, threshold) {
    let result = [];
    for (let i = 0; i < arr.length; i++) {
        if (arr[i] > threshold) {
            result.push(arr[i]);
        }
    }
    return result;
}

// Test
let data = [23, 45, 12, 67, 34, 89, 5, 42];

console.log("Sum:", sum(data));
console.log("Average:", average(data));
console.log("Min:", min(data));
console.log("Max:", max(data));
console.log("Above 30:", filterAbove(data, 30));
