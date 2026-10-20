#include <iostream>
#include <clocale>
#include <iomanip>   // для setw
#include <vector>    // для std::vector

using namespace std;

struct Node {
    int key;
    Node* left;
    Node* right;
    Node(int k) : key(k), left(nullptr), right(nullptr) {}
};

// ---------- базовые операции ----------
Node* insert(Node* p1, int key) {
    if (!p1) return new Node(key);
    if (key < p1->key)      p1->left = insert(p1->left, key);
    else if (key > p1->key) p1->right = insert(p1->right, key);
    else  cout << "Ключ " << key << " уже существует в дереве!\n";
    return p1;
}

Node* search(Node* p1, int key) {
    if (!p1 || p1->key == key) return p1;
    return (key < p1->key) ? search(p1->left, key) : search(p1->right, key);
}

Node* findMin(Node* p1) {
    while (p1 && p1->left) p1 = p1->left;
    return p1;
}

Node* remove(Node* p1, int key) {
    if (!p1) return nullptr;
    if (key < p1->key)       p1->left = remove(p1->left, key);
    else if (key > p1->key)  p1->right = remove(p1->right, key);
    else {
        if (!p1->left) { Node* r = p1->right; delete p1; return r; }
        if (!p1->right) { Node* l = p1->left;  delete p1; return l; }
        Node* m = findMin(p1->right);
        p1->key = m->key;
        p1->right = remove(p1->right, m->key);
    }
    return p1;
}

void printTree(Node* root, int space = 0, int gap = 5) {
    if (root == nullptr) return;

    space += gap;

    printTree(root->right, space, gap);

    cout << '\n';
    for (int i = gap; i < space; ++i) cout << ' ';
    cout << root->key << '\n';

    printTree(root->left, space, gap);
}

void clearTree(Node*& p1) {
    if (!p1) return;
    clearTree(p1->left);
    clearTree(p1->right);
    delete p1;
    p1 = nullptr;
}

int countEvenNodes(Node* p1) {
    if (!p1) return 0;
    int cnt = (p1->key % 2 == 0);
    return cnt + countEvenNodes(p1->left) + countEvenNodes(p1->right);
}

Node* makeTree(Node* p1) {
    if (p1) {
        cout << "Дерево уже создано! Очистите его сначала.\n";
        return p1;
    }
    int n, key;
    cout << "Введите количество элементов для создания дерева: ";
    cin >> n;
    for (int i = 0; i < n; ++i) {
        cout << "Введите ключ для элемента " << i + 1 << ": ";
        cin >> key;
        p1 = insert(p1, key);
    }
    return p1;
}

// ---------- main ----------
int main() {
    setlocale(LC_ALL, "rus");
    Node* root = nullptr;
    int choice, key;

    do {
        cout << "\n1. Создать дерево\n"
            << "2. Добавить элемент\n"
            << "3. Поиск элемента\n"
            << "4. Удалить элемент\n"
            << "5. Вывести дерево\n"
            << "6. Подсчитать количество узлов с четными ключами\n"
            << "7. Очистить дерево\n"
            << "8. Выход\n"
            << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            root = makeTree(root);
            break;
        case 2:
            cout << "Введите ключ для добавления: ";
            cin >> key;
            root = insert(root, key);
            break;
        case 3:
            cout << "Введите ключ для поиска: ";
            cin >> key;
            cout << (search(root, key) ? "Элемент найден.\n"
                : "Элемент не найден.\n");
            break;
        case 4:
            cout << "Введите ключ для удаления: ";
            cin >> key;
            root = remove(root, key);
            cout << "Удаление выполнено.\n";
            break;
        case 5:
            if (!root) cout << "Дерево пустое.\n";
            else       printTree(root);
            break;
        case 6:
            cout << "Количество узлов с четными ключами: "
                << countEvenNodes(root) << '\n';
            break;
        case 7:
            clearTree(root);
            cout << "Дерево очищено.\n";
            break;
        case 8:
            clearTree(root);
            cout << "Выход.\n";
            break;
        default:
            cout << "Неверный выбор.\n";
        }
    } while (choice != 8);

    return 0;
}
