#include <iostream>
#include <string>
#include <vector>
#include <map>

using namespace std;

class LZW {
private:
    map<string, int> dict;

    void initDict() {
        dict.clear();
        for (int i = 0; i < 256; i++) {
            string s(1, char(i));
            dict[s] = i;
        }
    }

public:
    vector<int> encode(const string& text) {
        initDict();
        vector<int> result;

        string w = "";
        for (char c : text) {
            string wc = w + c;
            if (dict.count(wc)) {
                w = wc;
            }
            else {
                result.push_back(dict[w]);
                dict[wc] = dict.size();
                w = string(1, c);
            }
        }

        if (!w.empty()) {
            result.push_back(dict[w]);
        }

        return result;
    }

    string decode(const vector<int>& codes) {
        initDict();
        map<int, string> revDict;

        for (int i = 0; i < 256; i++) {
            revDict[i] = string(1, char(i));
        }

        string result = "";
        string prev = revDict[codes[0]];
        result += prev;

        for (size_t i = 1; i < codes.size(); i++) {
            string curr;
            if (revDict.count(codes[i])) {
                curr = revDict[codes[i]];
            }
            else {
                curr = prev + prev[0];
            }

            result += curr;
            revDict[revDict.size()] = prev + curr[0];
            prev = curr;
        }

        return result;
    }
};

// Функция для вывода таблицы встречаемости символов
void printFrequencyTable(const string& text) {
    cout << "\n=== ТАБЛИЦА ВСТРЕЧАЕМОСТИ СИМВОЛОВ ===" << endl;
    cout << "Символ | Количество | Процент" << endl;
    cout << "--------------------------------" << endl;

    map<char, int> freq;
    for (char ch : text) {
        freq[ch]++;
    }

    for (auto& pair : freq) {
        char ch = pair.first;
        int count = pair.second;
        double percentage = (count * 100.0) / text.length();

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

// Функция для вывода таблицы кодов LZW
void printCodeTable(const vector<int>& codes, const string& text) {
    cout << "\n=== ТАБЛИЦА КОДОВ LZW ===" << endl;
    cout << "Последовательность | Код" << endl;
    cout << "----------------------" << endl;

    // Воссоздаем словарь для отображения
    map<string, int> dict;
    for (int i = 0; i < 256; i++) {
        string s(1, char(i));
        dict[s] = i;
    }

    vector<pair<string, int>> sequences;
    string w = "";
    for (char c : text) {
        string wc = w + c;
        if (dict.count(wc)) {
            w = wc;
        }
        else {
            sequences.push_back({ w, dict[w] });
            dict[wc] = dict.size();
            w = string(1, c);
        }
    }
    if (!w.empty()) {
        sequences.push_back({ w, dict[w] });
    }

    for (auto& seq : sequences) {
        string display = seq.first;
        if (display == " ") display = "[пробел]";
        else if (display == "\n") display = "[\\n]";
        else if (display == "\t") display = "[\\t]";

        cout << "\"" << display << "\" | " << seq.second << endl;
    }
}

// Функция для вывода закодированной последовательности
void printEncodedText(const vector<int>& encoded) {
    cout << "\n=== ВЫХОДНАЯ (ЗАКОДИРОВАННАЯ) ПОСЛЕДОВАТЕЛЬНОСТЬ ===" << endl;
    for (int code : encoded) {
        cout << code << " ";
    }
    cout << endl;

    // Дополнительная информация
    cout << "\nДополнительная информация:" << endl;
    cout << "Длина исходного текста (в битах ASCII): " << encoded.size() * 8 << " бит" << endl;
    cout << "Длина закодированного текста: " << encoded.size() * 12 << " бит" << endl;
    double compressionRatio = (1.0 - (double)(encoded.size() * 12) / (encoded.size() * 8)) * 100;
    cout << "Степень сжатия: " << compressionRatio << "%" << endl;
}

int main() {
    setlocale(LC_ALL, "Russian");
    LZW lzw;

    cout << "=== LZW КОДИРОВАНИЕ ===" << endl;

    string text;
    cout << "Введите текст: ";
    getline(cin, text);

    if (text.empty()) {
        cout << "Ошибка: текст не может быть пустым!" << endl;
        return 1;
    }

    // Кодирование
    vector<int> coded = lzw.encode(text);

    // Вывод результатов в стиле лабораторной
    printFrequencyTable(text);
    printCodeTable(coded, text);
    printEncodedText(coded);

    // Декодирование и проверка
    string decoded = lzw.decode(coded);
    cout << "\n=== ПРОВЕРКА ДЕКОДИРОВАНИЯ ===" << endl;
    cout << "Исходный текст: " << text << endl;
    cout << "Декодированный текст: " << decoded << endl;

    if (text == decoded) {
        cout << "✅ Декодирование успешно!" << endl;
    }
    else {
        cout << "❌ Ошибка декодирования!" << endl;
    }

    return 0;
}