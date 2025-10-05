/**
 * LinkedList Implementation
 * Student: Excellent Example
 *
 * A generic doubly-linked list implementation with comprehensive
 * error handling and documentation.
 */

public class LinkedList<T> {
    private Node<T> head;
    private Node<T> tail;
    private int size;

    /**
     * Inner class representing a node in the linked list.
     */
    private static class Node<T> {
        T data;
        Node<T> next;
        Node<T> prev;

        Node(T data) {
            this.data = data;
            this.next = null;
            this.prev = null;
        }
    }

    /**
     * Constructs an empty linked list.
     */
    public LinkedList() {
        this.head = null;
        this.tail = null;
        this.size = 0;
    }

    /**
     * Adds an element to the end of the list.
     *
     * @param data the element to add
     * @throws IllegalArgumentException if data is null
     */
    public void add(T data) {
        if (data == null) {
            throw new IllegalArgumentException("Cannot add null element");
        }

        Node<T> newNode = new Node<>(data);

        if (isEmpty()) {
            head = tail = newNode;
        } else {
            tail.next = newNode;
            newNode.prev = tail;
            tail = newNode;
        }

        size++;
    }

    /**
     * Inserts an element at the specified index.
     *
     * @param index the position to insert at
     * @param data the element to insert
     * @throws IndexOutOfBoundsException if index is invalid
     * @throws IllegalArgumentException if data is null
     */
    public void insert(int index, T data) {
        if (index < 0 || index > size) {
            throw new IndexOutOfBoundsException("Index: " + index + ", Size: " + size);
        }
        if (data == null) {
            throw new IllegalArgumentException("Cannot insert null element");
        }

        if (index == 0) {
            addFirst(data);
        } else if (index == size) {
            add(data);
        } else {
            Node<T> newNode = new Node<>(data);
            Node<T> current = getNodeAt(index);

            newNode.next = current;
            newNode.prev = current.prev;
            current.prev.next = newNode;
            current.prev = newNode;

            size++;
        }
    }

    /**
     * Removes and returns the element at the specified index.
     *
     * @param index the position to remove from
     * @return the removed element
     * @throws IndexOutOfBoundsException if index is invalid
     */
    public T remove(int index) {
        if (index < 0 || index >= size) {
            throw new IndexOutOfBoundsException("Index: " + index + ", Size: " + size);
        }

        Node<T> toRemove;

        if (index == 0) {
            toRemove = head;
            head = head.next;
            if (head != null) {
                head.prev = null;
            } else {
                tail = null;
            }
        } else if (index == size - 1) {
            toRemove = tail;
            tail = tail.prev;
            tail.next = null;
        } else {
            toRemove = getNodeAt(index);
            toRemove.prev.next = toRemove.next;
            toRemove.next.prev = toRemove.prev;
        }

        size--;
        return toRemove.data;
    }

    /**
     * Returns the element at the specified index.
     *
     * @param index the position to retrieve from
     * @return the element at the specified position
     * @throws IndexOutOfBoundsException if index is invalid
     */
    public T get(int index) {
        if (index < 0 || index >= size) {
            throw new IndexOutOfBoundsException("Index: " + index + ", Size: " + size);
        }
        return getNodeAt(index).data;
    }

    /**
     * Returns the number of elements in the list.
     *
     * @return the size of the list
     */
    public int size() {
        return size;
    }

    /**
     * Checks if the list is empty.
     *
     * @return true if the list contains no elements
     */
    public boolean isEmpty() {
        return size == 0;
    }

    /**
     * Removes all elements from the list.
     */
    public void clear() {
        head = tail = null;
        size = 0;
    }

    /**
     * Adds an element to the beginning of the list.
     *
     * @param data the element to add
     */
    private void addFirst(T data) {
        Node<T> newNode = new Node<>(data);

        if (isEmpty()) {
            head = tail = newNode;
        } else {
            newNode.next = head;
            head.prev = newNode;
            head = newNode;
        }

        size++;
    }

    /**
     * Helper method to get the node at a specified index.
     *
     * @param index the position of the node
     * @return the node at the specified position
     */
    private Node<T> getNodeAt(int index) {
        Node<T> current;

        // Optimize by starting from the closest end
        if (index < size / 2) {
            current = head;
            for (int i = 0; i < index; i++) {
                current = current.next;
            }
        } else {
            current = tail;
            for (int i = size - 1; i > index; i--) {
                current = current.prev;
            }
        }

        return current;
    }

    /**
     * Returns a string representation of the list.
     *
     * @return a string containing all elements in the list
     */
    @Override
    public String toString() {
        if (isEmpty()) {
            return "[]";
        }

        StringBuilder sb = new StringBuilder("[");
        Node<T> current = head;

        while (current != null) {
            sb.append(current.data);
            if (current.next != null) {
                sb.append(", ");
            }
            current = current.next;
        }

        sb.append("]");
        return sb.toString();
    }

    /**
     * Test the LinkedList implementation.
     */
    public static void main(String[] args) {
        LinkedList<Integer> list = new LinkedList<>();

        // Test add
        list.add(1);
        list.add(2);
        list.add(3);
        System.out.println("After adding 1, 2, 3: " + list);

        // Test insert
        list.insert(1, 10);
        System.out.println("After inserting 10 at index 1: " + list);

        // Test get
        System.out.println("Element at index 2: " + list.get(2));

        // Test remove
        list.remove(1);
        System.out.println("After removing element at index 1: " + list);

        // Test size
        System.out.println("Size: " + list.size());

        // Test clear
        list.clear();
        System.out.println("After clear: " + list + ", isEmpty: " + list.isEmpty());
    }
}
