// Binary Search Tree Implementation
// Student: John Doe
// Course: CSE 232

#include <iostream>
#include <memory>
#include <queue>

template<typename T>
class BinarySearchTree {
private:
    struct Node {
        T data;
        std::unique_ptr<Node> left;
        std::unique_ptr<Node> right;

        explicit Node(const T& value) : data(value), left(nullptr), right(nullptr) {}
    };

    std::unique_ptr<Node> root;
    size_t size_;

    // Helper function for recursive insertion
    void insertHelper(std::unique_ptr<Node>& node, const T& value) {
        if (!node) {
            node = std::make_unique<Node>(value);
            size_++;
            return;
        }

        if (value < node->data) {
            insertHelper(node->left, value);
        } else if (value > node->data) {
            insertHelper(node->right, value);
        }
    }

    // Helper function for in-order traversal
    void inorderHelper(const std::unique_ptr<Node>& node) const {
        if (node) {
            inorderHelper(node->left);
            std::cout << node->data << " ";
            inorderHelper(node->right);
        }
    }

    // Helper function for finding minimum value
    const Node* findMin(const std::unique_ptr<Node>& node) const {
        if (!node) return nullptr;
        if (!node->left) return node.get();
        return findMin(node->left);
    }

    // Helper function for deletion
    std::unique_ptr<Node> deleteHelper(std::unique_ptr<Node> node, const T& value) {
        if (!node) return nullptr;

        if (value < node->data) {
            node->left = deleteHelper(std::move(node->left), value);
        } else if (value > node->data) {
            node->right = deleteHelper(std::move(node->right), value);
        } else {
            // Node to be deleted found
            if (!node->left) {
                size_--;
                return std::move(node->right);
            } else if (!node->right) {
                size_--;
                return std::move(node->left);
            }

            // Node has two children
            const Node* minNode = findMin(node->right);
            node->data = minNode->data;
            node->right = deleteHelper(std::move(node->right), node->data);
        }
        return node;
    }

public:
    BinarySearchTree() : root(nullptr), size_(0) {}

    void insert(const T& value) {
        insertHelper(root, value);
    }

    void remove(const T& value) {
        root = deleteHelper(std::move(root), value);
    }

    bool contains(const T& value) const {
        const Node* current = root.get();
        while (current) {
            if (value == current->data) return true;
            current = (value < current->data) ? current->left.get() : current->right.get();
        }
        return false;
    }

    void inorderTraversal() const {
        inorderHelper(root);
        std::cout << std::endl;
    }

    size_t size() const {
        return size_;
    }

    bool empty() const {
        return size_ == 0;
    }
};

int main() {
    BinarySearchTree<int> bst;

    // Test insertions
    bst.insert(50);
    bst.insert(30);
    bst.insert(70);
    bst.insert(20);
    bst.insert(40);
    bst.insert(60);
    bst.insert(80);

    std::cout << "In-order traversal: ";
    bst.inorderTraversal();

    std::cout << "Size: " << bst.size() << std::endl;
    std::cout << "Contains 40: " << (bst.contains(40) ? "Yes" : "No") << std::endl;
    std::cout << "Contains 100: " << (bst.contains(100) ? "Yes" : "No") << std::endl;

    bst.remove(30);
    std::cout << "After removing 30: ";
    bst.inorderTraversal();

    return 0;
}
