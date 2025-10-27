"""
Binary Search Tree Implementation
Student: Michael Chen
CSE 310 - Advanced Data Structures

A complete BST implementation with insertion, deletion, and traversal methods.
Includes balance checking and visualization capabilities.
"""

from typing import Optional, List, Callable
from dataclasses import dataclass


@dataclass
class Node:
    """Represents a node in the binary search tree."""
    value: int
    left: Optional['Node'] = None
    right: Optional['Node'] = None

    def __str__(self) -> str:
        return str(self.value)


class BinarySearchTree:
    """
    A binary search tree with comprehensive functionality.

    Supports insertion, deletion, search, and multiple traversal methods.
    Includes utilities for checking tree properties like balance and height.
    """

    def __init__(self):
        """Initializes an empty binary search tree."""
        self.root: Optional[Node] = None
        self.size: int = 0

    def insert(self, value: int) -> bool:
        """
        Inserts a value into the tree.

        Args:
            value: The integer value to insert

        Returns:
            True if inserted successfully, False if value already exists
        """
        if self.root is None:
            self.root = Node(value)
            self.size += 1
            return True

        return self._insert_recursive(self.root, value)

    def _insert_recursive(self, node: Node, value: int) -> bool:
        """Helper method for recursive insertion."""
        if value == node.value:
            return False  # Duplicate values not allowed

        if value < node.value:
            if node.left is None:
                node.left = Node(value)
                self.size += 1
                return True
            return self._insert_recursive(node.left, value)
        else:
            if node.right is None:
                node.right = Node(value)
                self.size += 1
                return True
            return self._insert_recursive(node.right, value)

    def search(self, value: int) -> bool:
        """
        Searches for a value in the tree.

        Args:
            value: The value to search for

        Returns:
            True if found, False otherwise
        """
        return self._search_recursive(self.root, value)

    def _search_recursive(self, node: Optional[Node], value: int) -> bool:
        """Helper method for recursive search."""
        if node is None:
            return False

        if value == node.value:
            return True
        elif value < node.value:
            return self._search_recursive(node.left, value)
        else:
            return self._search_recursive(node.right, value)

    def delete(self, value: int) -> bool:
        """
        Deletes a value from the tree.

        Args:
            value: The value to delete

        Returns:
            True if deleted successfully, False if not found
        """
        self.root, deleted = self._delete_recursive(self.root, value)
        if deleted:
            self.size -= 1
        return deleted

    def _delete_recursive(self, node: Optional[Node], value: int) -> tuple[Optional[Node], bool]:
        """Helper method for recursive deletion."""
        if node is None:
            return None, False

        if value < node.value:
            node.left, deleted = self._delete_recursive(node.left, value)
            return node, deleted
        elif value > node.value:
            node.right, deleted = self._delete_recursive(node.right, value)
            return node, deleted
        else:
            # Node to delete found
            # Case 1: No children
            if node.left is None and node.right is None:
                return None, True

            # Case 2: One child
            if node.left is None:
                return node.right, True
            if node.right is None:
                return node.left, True

            # Case 3: Two children
            # Find inorder successor (minimum in right subtree)
            successor = self._find_min(node.right)
            node.value = successor.value
            node.right, _ = self._delete_recursive(node.right, successor.value)
            return node, True

    def _find_min(self, node: Node) -> Node:
        """Finds the minimum value node in a subtree."""
        while node.left is not None:
            node = node.left
        return node

    def inorder_traversal(self) -> List[int]:
        """Returns list of values in inorder (sorted) sequence."""
        result = []
        self._inorder_recursive(self.root, result)
        return result

    def _inorder_recursive(self, node: Optional[Node], result: List[int]) -> None:
        """Helper for inorder traversal."""
        if node is not None:
            self._inorder_recursive(node.left, result)
            result.append(node.value)
            self._inorder_recursive(node.right, result)

    def preorder_traversal(self) -> List[int]:
        """Returns list of values in preorder sequence."""
        result = []
        self._preorder_recursive(self.root, result)
        return result

    def _preorder_recursive(self, node: Optional[Node], result: List[int]) -> None:
        """Helper for preorder traversal."""
        if node is not None:
            result.append(node.value)
            self._preorder_recursive(node.left, result)
            self._preorder_recursive(node.right, result)

    def postorder_traversal(self) -> List[int]:
        """Returns list of values in postorder sequence."""
        result = []
        self._postorder_recursive(self.root, result)
        return result

    def _postorder_recursive(self, node: Optional[Node], result: List[int]) -> None:
        """Helper for postorder traversal."""
        if node is not None:
            self._postorder_recursive(node.left, result)
            self._postorder_recursive(node.right, result)
            result.append(node.value)

    def height(self) -> int:
        """Returns the height of the tree."""
        return self._height_recursive(self.root)

    def _height_recursive(self, node: Optional[Node]) -> int:
        """Helper for calculating height."""
        if node is None:
            return 0
        left_height = self._height_recursive(node.left)
        right_height = self._height_recursive(node.right)
        return 1 + max(left_height, right_height)

    def is_balanced(self) -> bool:
        """Checks if the tree is height-balanced."""
        return self._is_balanced_recursive(self.root)[0]

    def _is_balanced_recursive(self, node: Optional[Node]) -> tuple[bool, int]:
        """Helper for checking balance. Returns (is_balanced, height)."""
        if node is None:
            return True, 0

        left_balanced, left_height = self._is_balanced_recursive(node.left)
        right_balanced, right_height = self._is_balanced_recursive(node.right)

        balanced = (left_balanced and right_balanced and
                   abs(left_height - right_height) <= 1)
        height = 1 + max(left_height, right_height)

        return balanced, height

    def __len__(self) -> int:
        """Returns the number of nodes in the tree."""
        return self.size

    def __str__(self) -> str:
        """String representation of the tree (inorder)."""
        return str(self.inorder_traversal())


def main():
    """Test the binary search tree implementation."""
    bst = BinarySearchTree()

    # Test insertions
    values = [50, 30, 70, 20, 40, 60, 80, 10, 25, 35, 65]
    print("Inserting values:", values)
    for value in values:
        bst.insert(value)

    print(f"\nTree size: {len(bst)}")
    print(f"Tree height: {bst.height()}")
    print(f"Is balanced: {bst.is_balanced()}")

    # Test traversals
    print(f"\nInorder (sorted): {bst.inorder_traversal()}")
    print(f"Preorder: {bst.preorder_traversal()}")
    print(f"Postorder: {bst.postorder_traversal()}")

    # Test search
    print(f"\nSearch for 40: {bst.search(40)}")
    print(f"Search for 100: {bst.search(100)}")

    # Test deletion
    print(f"\nDeleting 30...")
    bst.delete(30)
    print(f"Inorder after deletion: {bst.inorder_traversal()}")
    print(f"Tree size after deletion: {len(bst)}")

    # Test edge cases
    print("\n--- Edge Case Tests ---")
    empty_bst = BinarySearchTree()
    print(f"Empty tree size: {len(empty_bst)}")
    print(f"Empty tree height: {empty_bst.height()}")
    print(f"Search in empty tree: {empty_bst.search(1)}")


if __name__ == "__main__":
    main()
