# sorting algorithms

def sort(arr):
    # bubble sort i think
    for i in range(len(arr)):
        for j in range(len(arr)-1):
            if arr[j] > arr[j+1]:
                arr[j], arr[j+1] = arr[j+1], arr[j]
    return arr

# test
x = [5,2,8,1,9]
print(sort(x))
