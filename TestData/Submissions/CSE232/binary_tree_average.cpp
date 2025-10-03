// Binary Tree - Average Implementation
// Has some memory management issues and style problems

#include <iostream>

template<typename T>
class BST {
private:
    struct Node {
        T data;
        Node* left;
        Node* right;
        Node(T val) : data(val), left(NULL), right(NULL) {}
    };

    Node* root;

    void insert(Node*& node, T value) {
        if (node == NULL) {
            node = new Node(value);
            return;
        }
        if (value < node->data)
            insert(node->left, value);
        else
            insert(node->right, value);
    }

    void print(Node* node) {
        if (node != NULL) {
            print(node->left);
            std::cout << node->data << " ";
            print(node->right);
        }
    }

public:
    BST() { root = NULL; }

    // Missing destructor - memory leak!

    void add(T value) {
        insert(root, value);
    }

    void display() {
        print(root);
        std::cout << std::endl;
    }

    bool find(T value) {
        Node* current = root;
        while (current != NULL) {
            if (current->data == value) return true;
            if (value < current->data)
                current = current->left;
            else
                current = current->right;
        }
        return false;
    }

    // Remove function not implemented
};

int main() {
    BST<int> tree;
    tree.add(50);
    tree.add(30);
    tree.add(70);
    tree.add(20);
    tree.add(40);

    std::cout << "Tree contents: ";
    tree.display();

    std::cout << "Find 30: " << tree.find(30) << std::endl;

    // No cleanup - memory leak
    return 0;
}
