/**
 * Hash Table Implementation
 * Student: Sarah Martinez
 * CSE 232 - Data Structures
 *
 * A hash table using separate chaining for collision resolution.
 * Features dynamic resizing when load factor exceeds 0.75.
 */

#include <iostream>
#include <vector>
#include <list>
#include <string>
#include <stdexcept>

template<typename K, typename V>
class HashTable {
private:
    struct Entry {
        K key;
        V value;

        Entry(const K& k, const V& v) : key(k), value(v) {}
    };

    std::vector<std::list<Entry>> buckets;
    size_t num_elements;
    float max_load_factor;

    /**
     * Hash function using std::hash
     */
    size_t hash(const K& key) const {
        return std::hash<K>{}(key) % buckets.size();
    }

    /**
     * Resizes the hash table when load factor is exceeded
     */
    void resize() {
        size_t new_size = buckets.size() * 2;
        std::vector<std::list<Entry>> new_buckets(new_size);

        // Rehash all elements
        for (const auto& bucket : buckets) {
            for (const auto& entry : bucket) {
                size_t new_index = std::hash<K>{}(entry.key) % new_size;
                new_buckets[new_index].push_back(entry);
            }
        }

        buckets = std::move(new_buckets);
    }

public:
    /**
     * Constructor with initial capacity
     */
    explicit HashTable(size_t initial_capacity = 16)
        : buckets(initial_capacity), num_elements(0), max_load_factor(0.75f) {}

    /**
     * Inserts or updates a key-value pair
     */
    void insert(const K& key, const V& value) {
        // Check load factor
        if (load_factor() > max_load_factor) {
            resize();
        }

        size_t index = hash(key);

        // Check if key exists
        for (auto& entry : buckets[index]) {
            if (entry.key == key) {
                entry.value = value; // Update existing
                return;
            }
        }

        // Add new entry
        buckets[index].emplace_back(key, value);
        num_elements++;
    }

    /**
     * Retrieves value by key
     * @throws std::out_of_range if key not found
     */
    V& at(const K& key) {
        size_t index = hash(key);

        for (auto& entry : buckets[index]) {
            if (entry.key == key) {
                return entry.value;
            }
        }

        throw std::out_of_range("Key not found");
    }

    /**
     * Checks if key exists
     */
    bool contains(const K& key) const {
        size_t index = hash(key);

        for (const auto& entry : buckets[index]) {
            if (entry.key == key) {
                return true;
            }
        }

        return false;
    }

    /**
     * Removes a key-value pair
     * @return true if removed, false if not found
     */
    bool remove(const K& key) {
        size_t index = hash(key);

        for (auto it = buckets[index].begin(); it != buckets[index].end(); ++it) {
            if (it->key == key) {
                buckets[index].erase(it);
                num_elements--;
                return true;
            }
        }

        return false;
    }

    /**
     * Returns current load factor
     */
    float load_factor() const {
        return static_cast<float>(num_elements) / buckets.size();
    }

    /**
     * Returns number of elements
     */
    size_t size() const {
        return num_elements;
    }

    /**
     * Checks if hash table is empty
     */
    bool empty() const {
        return num_elements == 0;
    }

    /**
     * Prints hash table contents
     */
    void print() const {
        for (size_t i = 0; i < buckets.size(); i++) {
            if (!buckets[i].empty()) {
                std::cout << "Bucket " << i << ": ";
                for (const auto& entry : buckets[i]) {
                    std::cout << "[" << entry.key << ":" << entry.value << "] ";
                }
                std::cout << std::endl;
            }
        }
    }
};

int main() {
    HashTable<std::string, int> ages;

    // Test insertions
    ages.insert("Alice", 25);
    ages.insert("Bob", 30);
    ages.insert("Charlie", 35);
    ages.insert("Diana", 28);
    ages.insert("Eve", 32);

    std::cout << "Hash Table Contents:" << std::endl;
    ages.print();

    std::cout << "\nSize: " << ages.size() << std::endl;
    std::cout << "Load Factor: " << ages.load_factor() << std::endl;

    // Test retrieval
    std::cout << "\nAlice's age: " << ages.at("Alice") << std::endl;

    // Test contains
    std::cout << "Contains 'Bob': " << (ages.contains("Bob") ? "Yes" : "No") << std::endl;
    std::cout << "Contains 'Frank': " << (ages.contains("Frank") ? "Yes" : "No") << std::endl;

    // Test update
    ages.insert("Alice", 26);
    std::cout << "\nAlice's updated age: " << ages.at("Alice") << std::endl;

    // Test removal
    ages.remove("Charlie");
    std::cout << "\nAfter removing Charlie:" << std::endl;
    ages.print();

    // Test resizing with many insertions
    for (int i = 0; i < 20; i++) {
        ages.insert("Person" + std::to_string(i), 20 + i);
    }

    std::cout << "\nAfter adding 20 more entries:" << std::endl;
    std::cout << "Size: " << ages.size() << std::endl;
    std::cout << "Load Factor: " << ages.load_factor() << std::endl;

    return 0;
}
