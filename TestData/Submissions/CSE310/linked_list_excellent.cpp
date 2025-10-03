/**
 * Linked List Implementation - CSE 310
 * Student: Jane Smith
 *
 * A complete doubly-linked list with proper memory management
 * and comprehensive error handling.
 */

#include <iostream>
#include <stdexcept>

template<typename T>
class LinkedList {
private:
    struct Node {
        T data;
        Node* next;
        Node* prev;

        explicit Node(const T& value)
            : data(value), next(nullptr), prev(nullptr) {}
    };

    Node* head_;
    Node* tail_;
    size_t size_;

    /**
     * Validates an index is within bounds
     * @param index The index to validate
     * @throws std::out_of_range if index is invalid
     */
    void validateIndex(size_t index) const {
        if (index >= size_) {
            throw std::out_of_range("Index out of bounds");
        }
    }

public:
    /**
     * Constructs an empty linked list
     */
    LinkedList() : head_(nullptr), tail_(nullptr), size_(0) {}

    /**
     * Destructor - properly cleans up all nodes
     */
    ~LinkedList() {
        clear();
    }

    /**
     * Copy constructor - deep copy
     */
    LinkedList(const LinkedList& other) : head_(nullptr), tail_(nullptr), size_(0) {
        Node* current = other.head_;
        while (current) {
            pushBack(current->data);
            current = current->next;
        }
    }

    /**
     * Assignment operator - deep copy with self-assignment check
     */
    LinkedList& operator=(const LinkedList& other) {
        if (this != &other) {
            clear();
            Node* current = other.head_;
            while (current) {
                pushBack(current->data);
                current = current->next;
            }
        }
        return *this;
    }

    /**
     * Adds element to the front - O(1)
     */
    void pushFront(const T& value) {
        Node* newNode = new Node(value);
        if (empty()) {
            head_ = tail_ = newNode;
        } else {
            newNode->next = head_;
            head_->prev = newNode;
            head_ = newNode;
        }
        size_++;
    }

    /**
     * Adds element to the back - O(1)
     */
    void pushBack(const T& value) {
        Node* newNode = new Node(value);
        if (empty()) {
            head_ = tail_ = newNode;
        } else {
            tail_->next = newNode;
            newNode->prev = tail_;
            tail_ = newNode;
        }
        size_++;
    }

    /**
     * Removes element from the front - O(1)
     * @throws std::runtime_error if list is empty
     */
    void popFront() {
        if (empty()) {
            throw std::runtime_error("Cannot pop from empty list");
        }
        Node* temp = head_;
        head_ = head_->next;
        if (head_) {
            head_->prev = nullptr;
        } else {
            tail_ = nullptr;
        }
        delete temp;
        size_--;
    }

    /**
     * Removes element from the back - O(1)
     * @throws std::runtime_error if list is empty
     */
    void popBack() {
        if (empty()) {
            throw std::runtime_error("Cannot pop from empty list");
        }
        Node* temp = tail_;
        tail_ = tail_->prev;
        if (tail_) {
            tail_->next = nullptr;
        } else {
            head_ = nullptr;
        }
        delete temp;
        size_--;
    }

    /**
     * Accesses element at index - O(n)
     * @param index Position to access
     * @return Reference to element
     * @throws std::out_of_range if index invalid
     */
    T& at(size_t index) {
        validateIndex(index);
        Node* current = head_;
        for (size_t i = 0; i < index; i++) {
            current = current->next;
        }
        return current->data;
    }

    /**
     * Removes all elements and frees memory
     */
    void clear() {
        while (!empty()) {
            popFront();
        }
    }

    /**
     * @return Number of elements in the list
     */
    size_t size() const { return size_; }

    /**
     * @return True if list is empty
     */
    bool empty() const { return size_ == 0; }

    /**
     * Prints all elements
     */
    void print() const {
        Node* current = head_;
        std::cout << "[";
        while (current) {
            std::cout << current->data;
            if (current->next) std::cout << ", ";
            current = current->next;
        }
        std::cout << "]" << std::endl;
    }
};

int main() {
    try {
        LinkedList<int> list;

        // Test push operations
        list.pushBack(10);
        list.pushBack(20);
        list.pushFront(5);
        list.pushBack(30);

        std::cout << "List after pushes: ";
        list.print();
        std::cout << "Size: " << list.size() << std::endl;

        // Test access
        std::cout << "Element at index 2: " << list.at(2) << std::endl;

        // Test pop operations
        list.popFront();
        list.popBack();
        std::cout << "After pops: ";
        list.print();

        // Test copy constructor
        LinkedList<int> list2 = list;
        std::cout << "Copied list: ";
        list2.print();

        // Test clear
        list.clear();
        std::cout << "After clear, size: " << list.size() << std::endl;

    } catch (const std::exception& e) {
        std::cerr << "Error: " << e.what() << std::endl;
        return 1;
    }

    return 0;
}
