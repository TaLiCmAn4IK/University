#include <iostream>
#include <string>
#include <vector>
#include <queue>
#include <map>
#include <algorithm>

using namespace std;

struct HuffmanNode {
    char character;
    int frequency;
    HuffmanNode* left;
    HuffmanNode* right;

    HuffmanNode(char ch, int freq) {
        character = ch;
        frequency = freq;
        left = right = nullptr;
    }

    HuffmanNode(int freq, HuffmanNode* l, HuffmanNode* r) {
        character = '\0';
        frequency = freq;
        left = l;
        right = r;
    }
};

struct Compare {
    bool operator()(HuffmanNode* a, HuffmanNode* b) {
        return a->frequency > b->frequency;
    }
};

class HuffmanCoding {
private:
    HuffmanNode* root;
    map<char, string> huffmanCodes;
    map<char, int> frequencyTable;

    void buildCodes(HuffmanNode* node, string code) {
        if (node == nullptr) return;

        if (node->character != '\0') {
            huffmanCodes[node->character] = code;
        }

        buildCodes(node->left, code + "0");
        buildCodes(node->right, code + "1");
    }

    void deleteTree(HuffmanNode* node) {
        if (node == nullptr) return;
        deleteTree(node->left);
        deleteTree(node->right);
        delete node;
    }

public:
    HuffmanCoding() : root(nullptr) {}

    ~HuffmanCoding() {
        deleteTree(root);
    }

    void buildFrequencyTable(const string& text) {
        frequencyTable.clear();
        for (char ch : text) {
            frequencyTable[ch]++;
        }
    }

    void buildHuffmanTree() {
        priority_queue<HuffmanNode*, vector<HuffmanNode*>, Compare> pq;

        for (auto& pair : frequencyTable) {
            pq.push(new HuffmanNode(pair.first, pair.second));
        }

        while (pq.size() > 1) {
            HuffmanNode* left = pq.top(); pq.pop();
            HuffmanNode* right = pq.top(); pq.pop();

            HuffmanNode* parent = new HuffmanNode(
                left->frequency + right->frequency, left, right);
            pq.push(parent);
        }

        if (!pq.empty()) {
            root = pq.top();
        }

        huffmanCodes.clear();
        if (root != nullptr) {
            buildCodes(root, "");
        }
    }

    string encode(const string& text) {
        string encodedText = "";
        for (char ch : text) {
            encodedText += huffmanCodes[ch];
        }
        return encodedText;
    }

    const map<char, int>& getFrequencyTable() const {
        return frequencyTable;
    }

    const map<char, string>& getHuffmanCodes() const {
        return huffmanCodes;
    }
};

void printFrequencyTable(const map<char, int>& freqTable, int totalChars) {
    cout << "\n=== Таблица встречаемости символов ===" << endl;
    cout << "Символ | Количество | Процент" << endl;
    cout << "--------------------------------" << endl;

    for (auto& pair : freqTable) {
        char ch = pair.first;
        int count = pair.second;
        double percentage = (count * 100.0) / totalChars;

        if (ch == ' ') {
            cout << "[пробел]";
        }
        else if (ch == '\n') {
            cout << "[\\n]   ";
        }
        else if (ch == '\t') {
            cout << "[\\t]   ";
        }
        else {
            cout << "'" << ch << "'    ";
        }

        cout << " | " << count << "        | " << percentage << "%" << endl;
    }
}

void printCodeTable(const map<char, string>& codeTable) {
    cout << "\n=== Таблица соответствия символа и кодовой последовательности ===" << endl;
    cout << "Символ | Код" << endl;
    cout << "--------------" << endl;

    for (auto& pair : codeTable) {
        char ch = pair.first;
        string code = pair.second;

        if (ch == ' ') {
            cout << "[пробел]";
        }
        else if (ch == '\n') {
            cout << "[\\n]   ";
        }
        else if (ch == '\t') {
            cout << "[\\t]   ";
        }
        else {
            cout << "'" << ch << "'    ";
        }

        cout << " | " << code << endl;
    }
}

void printEncodedText(const string& encodedText) {
    cout << "\n=== Выходнвая последовательность ===" << endl;
    cout << encodedText << endl;

    cout << "\nДополнительная информация:" << endl;
    cout << "Длина исходного текста (в битах ASCII): " << encodedText.length() / 8 * 8 << " бит" << endl;
    cout << "Длина закодированного текста: " << encodedText.length() << " бит" << endl;
    double compressionRatio = (1.0 - (double)encodedText.length() / (encodedText.length() / 8 * 8)) * 100;
    cout << "Степень сжатия: " << compressionRatio << "%" << endl;
}

int main() {
    setlocale(LC_ALL, "Russian");

    HuffmanCoding huffman;
    string text;

    cout << "Введите текст для кодирования: ";
    getline(cin, text);

    if (text.empty()) {
        cout << "Ошибка: текст не может быть пустым!" << endl;
        return 1;
    }

    huffman.buildFrequencyTable(text);

    huffman.buildHuffmanTree();

    string encodedText = huffman.encode(text);

    printFrequencyTable(huffman.getFrequencyTable(), text.length());
    printCodeTable(huffman.getHuffmanCodes());
    printEncodedText(encodedText);

    return 0;
}